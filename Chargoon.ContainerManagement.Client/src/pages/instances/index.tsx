import React, { useEffect, useState } from 'react';
import { InstanceGetDto, CustomeNightlyBuildLog, TemplateCommandColor, TemplateCommandExecDto, TemplateCommandGetDto, TemplateGetDto, ContainerListResponse, UserGetDto } from '../../models';
import { Instance_GetAllOwn, Instance_StartOwn, Instance_StopOwn, Instance_ChangeOwnTemplate, Template_GetAll, Instance_RunCommand, Docker_GetCommandLog, Instance_Signal, Custome_GetNightlyBuildLogs, Custome_GetNightlyBuildLogDownloadPath, User_GetOwn } from '../../services';
import { Modal, Col, Row, Form, Button, Tooltip, Dropdown, Menu, message } from 'antd';
import useInterval from '@use-it/interval';
import useForceUpdate from 'use-force-update';
import FileSaver from 'file-saver';
import Cascader, { CascaderOptionType } from 'antd/lib/cascader';

declare type Modals = '' | 'change-template' | 'help';


export default () => {

    const [loading, setLoading] = useState(false);
    const [modal, setModal] = useState<Modals>('');
    const [items, setItems] = useState<InstanceGetDto[]>([]);
    const [templates, setTemplates] = useState<TemplateGetDto[]>([]);
    const [selectedInstance, setSelectedInstance] = useState<InstanceGetDto>();
    const [isFormLoading, setIsFormLoading] = useState(false);
    const [isStartInstance, setIsStartInstance] = useState(false);

    const [isOnScreen, setIsOnScreen] = useState(true);
    const [isSignalOnLoading, setIsSignalOnLoading] = useState(false);

    const [selectedTemplate, setSelectedTemplate] = useState<TemplateGetDto>();
    const [nightlyBuildLogs, setNightlyBuildLogs] = useState<CustomeNightlyBuildLog[]>([]);

    const [user, setUser] = useState<UserGetDto>();

    const forceUpdate = useForceUpdate();

    useInterval(() => {
        loadSignal();
    }, 1000);

    const load = () => {
        setLoading(true);
        Instance_GetAllOwn().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch items failed');
            }
        }).finally(() => setLoading(false))
    }

    const startInstance = (instance: InstanceGetDto) => {
        setIsStartInstance(true);
        instance.isStarting = true;
        forceUpdate();
        message.info('Starting Instance please wait');
        Instance_StartOwn(instance.id).then(res => {
            if (res.data.success) {
                instance.isStarting = false;
                forceUpdate();
                message.success('Starting instance is in progress please wait');
            } else {
                message.error(res.data.message ?? 'Starting instance failed')
            }
        }).finally(() => {
            instance.isStarting = false;
            forceUpdate();
        });
    }

    const stopInstance = (instance: InstanceGetDto) => {
        instance.isStopping = true;
        forceUpdate();
        Instance_StopOwn(instance.id).then(res => {
            if (res.data.success) {
                message.success('Stopping instance is in progress please wait');
            } else {
                message.error(res.data.message ?? 'Stopping instance failed');
            }
        }).finally(() => {
            instance.isStopping = false;
            forceUpdate();
        });
    }

    const runCommand = (instance: InstanceGetDto, templateCommand: TemplateCommandGetDto) => {
        templateCommand.isRunning = true;
        forceUpdate();
        Instance_RunCommand(instance.id, templateCommand.id).then(res => {
            if (res.data.success) {
                message.success('Command is running inside container please wait until done');
            } else {
                message.error(res.data.message ?? 'Command running failed');
            }
        }).finally(() => {
            templateCommand.isRunning = false;
            forceUpdate();
        })
    }

    const changeTemplate = (instance: InstanceGetDto) => {
        setSelectedInstance(instance);
        setModal('change-template');
        setSelectedTemplate(instance.template);
        setIsFormLoading(true);
        Template_GetAll().then(res => {
            if (res.data.success) {
                setTemplates(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch templates failed')
            }
        }).finally(() => setIsFormLoading(false))
    }

    const submitChangeTemplate = () => {
        if (selectedInstance) {
            setIsFormLoading(true);
            Instance_ChangeOwnTemplate(selectedInstance.id, { templateId: selectedTemplate?.id }).then(res => {
                if (res.data.success) {
                    load();
                    setModal('');
                    message.success('Change template done successfully')
                } else {
                    message.error(res.data.message ?? 'Change template failed')
                }
            }).finally(() => setIsFormLoading(false));
        }
    }

    const showEnvironments = (instance: InstanceGetDto) => {
        const result = [];
        for (const key in instance.environments) {
            const value = instance.environments[key];
            result.push(`${key}=${value}`);
        }
        alert(result.join('\n'));
    }

    const showDescription = (instance: InstanceGetDto) => {
        alert(instance.template.description);
    }

    const showCommandDetail = (templateCommand: TemplateCommandGetDto, command: TemplateCommandExecDto) => {
        command.templateCommand = templateCommand;
        Docker_GetCommandLog(command.commandId).then(res => {
            if (res.data.success) {
                command.logs = res.data.data;
                FileSaver.saveAs(new Blob([res.data.data]), `${templateCommand.serviceName}-${templateCommand.name}-logs.txt`);
            } else {
                message.error(res.data.message ?? 'Fetch command log failed')
            }
        })
    }

    const loadSignal = () => {
        if (isOnScreen && !isSignalOnLoading) {
            setIsSignalOnLoading(true);
            Instance_Signal().then(res => {
                if (res.data.success) {
                    for (let i = 0; i < res.data.data.length; i++) {
                        const signal = res.data.data[i];
                        const instance = items.find(x => x.id === signal.instanceId);
                        if (instance) {
                            instance.services = signal.services;
                            instance.commands = signal.templateCommandExecs;
                            instance.containers = signal.containers;

                            if (isStartInstance) isStartupCommandsRuns(instance);
                        }
                    }
                    forceUpdate();
                }
            }).finally(() => setIsSignalOnLoading(false));
        }
    }

    const isStartupCommandsRuns = (instance: InstanceGetDto) => {
        if (isInstanceAllContainerStarted(instance) && instance.commands) {
            setIsStartInstance(false);
            const startups = instance.template.commands.filter(x => x.runOnStartup);
            for (let i = 0; i < startups.length; i++) {
                const startup = startups[i];
                if (instance.commands.findIndex(x => x.templateCommandId === startup.id) < 0) {
                    instance.commands.push({
                        commandId: '',
                        templateCommand: startup,
                        templateCommandId: startup.id,
                    });
                    runCommand(instance, startup);
                }
            }
        }
    }

    const getCommandCssClass = (command: TemplateCommandGetDto) => {
        switch (command.color) {
            case TemplateCommandColor.None: return 'default';
            case TemplateCommandColor.Black: return 'dark';
            case TemplateCommandColor.Blue: return 'primary';
            case TemplateCommandColor.Green: return 'success';
            case TemplateCommandColor.Red: return 'danger';
            case TemplateCommandColor.Yellow: return 'warning';
        }
    }

    const getInstanceAllService = (instance: InstanceGetDto): string[] => {
        if (!instance.template?.dockerComposeObj?.services) return [];
        return Object.keys(instance.template.dockerComposeObj.services);
    }

    const getContainerByServiceName = (instance: InstanceGetDto, serviceName: string): ContainerListResponse | undefined => {
        if (!instance?.containers?.length) return;
        return instance.containers.find(x => x.names.findIndex(y => y.indexOf(serviceName) > -1) > -1);
    }

    // const getServiceState = (instance: InstanceGetDto, serviceName: string): number => {
    //     if (!instance.services || !serviceName) return 0;
    //     const searchValue = (instance.name + "_" + serviceName).toLowerCase();
    //     return instance.services.findIndex(x => (x.spec ? x.spec.name.toLowerCase().indexOf(searchValue) > -1 : false)) > -1 ? 1 : -1;
    // }

    const getContainerState = (instance: InstanceGetDto, containerName: string): number => {
        if (!instance.containers || !containerName) return 0;
        const searchValue = (instance.name + "_" + containerName).toLowerCase();
        return instance.containers.findIndex(x => x.names.findIndex(y => y.toLowerCase().indexOf(searchValue) > -1) > -1) > -1 ? 1 : -1;
    }

    // const isInstanceAllServiceStarted = (instance: InstanceGetDto): boolean => {
    //     return getInstanceAllService(instance).findIndex((service) => {
    //         return getServiceState(instance, service) !== 1;
    //     }) < 0;
    // }

    // const isInstanceAllServiceStoped = (instance: InstanceGetDto): boolean => {
    //     return getInstanceAllService(instance).findIndex((service) => {
    //         return getServiceState(instance, service) === 1;
    //     }) < 0;
    // }

    const isInstanceAllContainerStarted = (instance: InstanceGetDto): boolean => {
        return getInstanceAllService(instance).findIndex((service) => {
            return getContainerState(instance, service) !== 1;
        }) < 0;
    }

    const isInstanceAllContainerStoped = (instance: InstanceGetDto): boolean => {
        return getInstanceAllService(instance).findIndex((service) => {
            return getContainerState(instance, service) === 1;
        }) < 0;
    }

    const isCommandRunning = (instance: InstanceGetDto, templateCommand: TemplateCommandGetDto) => {
        if (!instance.commands) return false;
        return instance.commands.findIndex(x => x.templateCommandId === templateCommand.id && x.inspect?.running) > -1;
    }

    // const getReplacedDockerCompose = (instance: InstanceGetDto): any => {
    //     if (instance.template) {
    //         let dockerComposeJson = JSON.stringify(instance.template.dockerComposeObj);
    //         for (const key in instance.environments) {
    //             const value = instance.environments[key];
    //             while (dockerComposeJson.indexOf(`{${key}}`) > -1) dockerComposeJson = dockerComposeJson.replace(`{${key}}`, value);
    //         }
    //         return JSON.parse(dockerComposeJson);
    //     }
    // }

    const getTemplateCascaderOptions = (): CascaderOptionType[] => {
        const templateCascaderItems: CascaderOptionType[] = [];
        const wordRegex = new RegExp(/^\w*[.-]/);
        const filtered = templates.filter(x => x.isActive);
        for (let i = 0; i < filtered.length; i++) {
            const template = filtered[i];
            const wordMatches = wordRegex.exec(template.name);
            if (wordMatches?.length) {
                const match = wordMatches[0];
                const matchLabel = match.substr(0, match.length - 1);
                const item = templateCascaderItems.find(x => x.label === matchLabel);
                if (item) {
                    item.children?.push({
                        value: template.id.toString(),
                        label: template.name.replace(match, '')
                    })
                } else {
                    templateCascaderItems.push({
                        value: 'root-' + template.id,
                        label: matchLabel,
                        children: [
                            {
                                value: template.id.toString(),
                                label: template.name.replace(match, '')
                            }
                        ]
                    })
                }
            }
        }
        return templateCascaderItems;
    }

    const getTemplateCascaderOptionsValue = (items: CascaderOptionType[], value: string) => {
        let result: string[] = [];
        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            if (item.children) {
                const childResult = getTemplateCascaderOptionsValue(item.children, value);
                if (childResult.length) {
                    if (item.value) result.push(item.value.toString());
                    result = result.concat(childResult);
                    return result;
                }
            } else if (item.value === value) {
                result.push(item.value.toString());
            }
        }
        return result;
    }

    useEffect(() => {
        load();

        User_GetOwn().then(res => {
            setUser(res?.data?.data);
        });
    }, []);

    useEffect(() => {

    }, [items]);

    useEffect(() => {
        setNightlyBuildLogs([]);
        if (!selectedTemplate) return;
        const branchRegex = new RegExp(/(RC)|(Release)|(R[0-9]{2}\w[0-9]{2})/);
        const branchMatches = branchRegex.exec(selectedTemplate.name);
        const dateRegex = new RegExp(/[0-9]{4}.[0-9]{2}.[0-9]{2}/);
        const dateMatches = dateRegex.exec(selectedTemplate.name);
        if (branchMatches?.length && dateMatches?.length) {
            Custome_GetNightlyBuildLogs(branchMatches[0], dateMatches[0]).then(res => {
                if (res.data.success) {
                    setNightlyBuildLogs(res.data.data);
                }
            })
        }
    }, [selectedTemplate]);

    const templateCascaderItems = getTemplateCascaderOptions();
    let baseUrl = (user?.host ? user.host : window.location.hostname);
    if(baseUrl.length > 1) {
        if(baseUrl.indexOf('http') < 0) baseUrl = 'http://' + baseUrl;
    }

    return <div className="page-instances" onMouseEnter={() => setIsOnScreen(true)} onMouseLeave={() => setIsOnScreen(false)}>
        <div className="wrapper">
            <div className="title-bar">
                <h4>Instances (Host: {user?.host ? user.host : 'Self Hosted'})</h4>
                <button className="btn btn-primary" onClick={load}>Reload</button>
                <button className="btn btn-info" onClick={() => setModal('help')}>Help</button>
            </div>
            <table className={"custom-table" + (loading ? ' loading' : '')}>
                <colgroup>
                    <col width="150px" />
                    <col width="200px" />
                    <col />
                    <col width="100px" />
                    <col width="300px" />
                    <col width="400px" />
                </colgroup>
                <thead>
                    <tr className="table-head">
                        <th>Instance Name</th>
                        <th>Template</th>
                        <th>Ports</th>
                        <th>Services</th>
                        <th>Commands</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(instance => <tr key={instance.id}>
                        <td>{instance.name}</td>
                        <td>
                            <button className={"btn btn-" + (instance.template ? "dark" : "danger")} onClick={() => changeTemplate(instance)}>{(instance.template ? instance.template.name : 'Please Select Template (Click on me)')}</button>
                        </td>
                        <td>
                            <div className="ports">
                                {instance.containers?.sort((x1, x2) => (x1.names[0] > x2.names[0] ? 1 : (x1.names[0] < x2.names[0] ? -1 : 0))).map((container, i) => {

                                    return <div className="service" key={i}>
                                        {container?.ports?.sort((x1, x2) => x1.publicPort > x2.publicPort ? 1 : (x1.publicPort < x2.publicPort ? -1 : 0))?.map((port, j) => {

                                            let portname = '';
                                            for (const key in instance.environments) {
                                                const value = instance.environments[key];

                                                if (value?.toString() === port?.publicPort?.toString()) {
                                                    portname = key
                                                        .replace('EXPOSED_', '')
                                                        .replace('_EXPOSED', '')
                                                        .replace('EXPOSED', '')
                                                        .replace('_PORT', '')
                                                        .replace('PORT_', '')
                                                        .replace('PORT', '');
                                                }
                                            }

                                            return <span className="port" key={j} title={portname}>
                                                <span className="name">{portname} = </span>
                                                <span className="value">
                                                    <a href={`${baseUrl}:${port.publicPort}`} target="_blank" rel="noopener noreferrer">{port.publicPort}</a>
                                                </span>
                                            </span>;
                                        })}
                                    </div>;
                                })}
                            </div>
                        </td>
                        <td>
                            <div className="services">
                                {getInstanceAllService(instance).map((service, i) => {
                                    // const serviceState = getServiceState(instance, service);
                                    const container = getContainerByServiceName(instance, service);
                                    const containerState = getContainerState(instance, service);
                                    return <Tooltip key={i} title={<>
                                        <div>{service}</div>
                                        {container ? <>
                                            <div>Image : {container?.image}</div>
                                        </> : null}
                                        <div>State : {(containerState === 1 ? 'Running' : (containerState === 0 ? 'N/A' : 'Down'))}</div>
                                    </>} placement={'top'}>
                                        <span className={
                                            (containerState === 1 ? ' container-running' : '') +
                                            (containerState === 0 ? '' : ' shutdown')
                                        }></span>
                                    </Tooltip>
                                })}
                            </div>
                        </td>
                        <td>
                            <div className="commands">
                                {instance.commands?.map((tce, i) => {
                                    const tc = instance.template.commands.find(x => x.id === tce.templateCommandId);
                                    if (!tc || !tce.inspect) return null;
                                    return <button key={i} title={(tce.inspect.exitCode ? `exit code : ${tce.inspect.exitCode}` : '')} onClick={() => showCommandDetail(tc, tce)}>
                                        {tce.inspect.running ?
                                            <div className="spinner-border" role="status">
                                                <span className="sr-only">Loading...</span>
                                            </div> :
                                            (tce.inspect.exitCode === 0 ?
                                                <i className="fa fa-check text-success"> Success</i> :
                                                <i className="fa fa-times text-danger"> Error</i>)
                                        }
                                        <span>{tc ? `${tc.serviceName} - ${tc.name}` : tce.inspect?.execID}</span>
                                    </button>
                                })}
                            </div>
                        </td>
                        <td>
                            {instance.template ? <div className="actions">
                                {!isInstanceAllContainerStoped(instance) ? <button className="btn btn-danger" disabled={instance.isStopping} onClick={() => stopInstance(instance)}>{instance.isStopping ? 'Removing...' : 'Remove'}</button> : null}
                                {!isInstanceAllContainerStarted(instance) ? <button className="btn btn-success" disabled={instance.isStarting} onClick={() => startInstance(instance)}>{instance.isStarting ? 'Starting...' : 'Start'}</button> : null}
                                <button className="btn btn-primary" onClick={() => showEnvironments(instance)}>ENV</button>
                                {instance.template.description ? <button className="btn btn-info" onClick={() => showDescription(instance)}>Description</button> : null}
                                {instance.template.commands?.length && isInstanceAllContainerStarted(instance) ? <Dropdown overlay={<Menu>
                                    {instance.template.commands?.map(templateCommand => {
                                        const isRunning = templateCommand.isRunning || isCommandRunning(instance, templateCommand);
                                        return <Menu.Item key={templateCommand.id}>
                                            <button className={"btn btn-" + getCommandCssClass(templateCommand)} disabled={isRunning} onClick={() => runCommand(instance, templateCommand)}>{templateCommand.name}{isRunning ? ' ...' : ''}</button>
                                        </Menu.Item>;
                                    })}
                                </Menu>}>
                                    <button className="btn btn-dark dropdown-toggle" >Commands</button>
                                </Dropdown>
                                    : null}
                            </div> : null}
                        </td>
                    </tr>)}
                </tbody>
            </table>
        </div>
        <Modal className="change-template-modal" title="Change template and run another network of containers" visible={modal === 'change-template'} onCancel={() => setModal('')} footer={null} maskClosable={false}>
            <Form className={(isFormLoading ? ' loading' : '')} onFinish={submitChangeTemplate}>
                <Row>
                    <Col xs={24}>
                        <Cascader
                            options={templateCascaderItems}
                            showSearch
                            allowClear
                            placeholder="Search & Choose Template"
                            value={(selectedTemplate ? getTemplateCascaderOptionsValue(templateCascaderItems, selectedTemplate.id.toString()) : undefined)}
                            onChange={e => {
                                setSelectedTemplate(e.length === 2 ? templates.find(x => x.id === +e[1]) : undefined);
                            }} />
                    </Col>
                </Row>
                {selectedInstance ? <Row>
                    <Col xs={12} className="p-2">
                        <Button type="primary" htmlType="submit" block disabled={!isInstanceAllContainerStoped(selectedInstance)}>{isInstanceAllContainerStoped(selectedInstance) ? 'Save' : 'First Stop Containers'}</Button>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Button type="primary" htmlType="button" block onClick={() => setModal('')} danger>Close</Button>
                    </Col>
                </Row> : null}
                {nightlyBuildLogs?.length ? <div>
                    <h2>Nightly Build Logs</h2>
                    <ul>
                        {nightlyBuildLogs.map(x => <li>
                            <a href={Custome_GetNightlyBuildLogDownloadPath(x.branch, x.date, x.name)} target="_blank" rel="noopener noreferrer">{x.name}</a>
                        </li>)}
                    </ul>
                </div> : null}
            </Form>
        </Modal>
        <Modal title="Help" visible={modal === 'help'} width="80vw" onCancel={() => setModal('')} footer={null} maskClosable={false}>
            <div className="alert alert-warning rtl" role="alert">
                <h4 className="alert-heading">راهنمای استفاده از سرویس</h4>
                <p>برای استفاده از سرویس مدیریت کانتینر های داکر موارد زیر را مطالعه فرمایید</p>
                <hr />
                <ul>
                    <li>ستون Name :‌ به ازای هر کاربر تعدادی Instance (حداقل یکی) قرار داده می شود و در هر Instance تنها میتوان یک Template را استارت نمود و در صورت نیاز به نسخه دیگر ابتدا باید آن را استاپ کنید و با تغییر Template مجددا آن را استارت نمود</li>
                    <li>ستون Template : تعدادی الگو در سیستم تعریف شده که مهمترین آنها برنچ های مختلف دیدگاه بوده که با توجه به فایل های تولید شده توسط بیلد شبانه طی زمانبندی Image ها و Template ها ایجاد میگردد. نهایت عمر هر Image و Template یک هفته میباشد.</li>
                    <li>ستون Ports :  هر Template حاوی تعدادی Port میباشد که از بیرون از کانتینر قابل دسترس میباشد مثل :‌SQLSERVER , Didgah5 , Didgah4, ....</li>
                    <li>
                        <span>ستون Services : هر Template حاوی تعدادی سرویس میباشد که در 4 حالت در این داشبورد به نمایش در می آیند </span>
                        <ul>
                            <li>مشکی :‌ وضعیت نامشخص می باشد (هنوز اطلاعات از سرور دریافت نشده است)</li>
                            <li>قرمز :‌ سرویس و کانتینر موجود نمیباشد</li>
                            <li>زرد :‌ یا سرویس یا کانتینر در حالت فعال میباشد</li>
                            <li>سبز : ‌هم سرویس هم کانتینر فعال بوده و در این وضعیت قابل استفاده می باشد</li>
                        </ul>
                    </li>
                    <li>
                        <span>ستون Commands : در این ستون وضعیت کامند های اجرا شده نمایش داده شده و لاگ آنها با کلیک بر روی هر کدام قابل دانلود میباشد. هر کامند این ستون میتواند در سه حالت باشد :‌</span>
                        <ul>
                            <li>Running : در حال اجرا بود در برخی کامند ها مثل File Broser حالت Running یعنی قابل استفاده میباشد و نباید ژایان یابد چرا که به ژورت مشخصی گوش میدهد و کامند زنده خواهد ماند</li>
                            <li>Error : اجرا شده و همراه با خطا ژایان یافته است</li>
                            <li>Success : اجرا شده و بدون خطا ژایان یافته</li>
                        </ul>
                    </li>
                    <li>
                        <span>ستون Actions : مجموعه دستوراتی است که میتوان رو Instance انجام داد. تعدادی از آنها رو مورد بررسی قرار میدهیم :‌</span>
                        <ul>
                            <li>Start : درخواست ساخت سرویس و کانتینر را به سرور میفرستد که پس از ساخت سرویس ها ژاسخ مثبت داده خواهد شد و پس از چند ثانیه باید زرد شود و سرویس به حالت Running در بیاید و چند ثانیه بعد کانتینر ساخته شده و سرویس ها سبز میشود اگر کانتینر ساخته نشد به معنای آن است که ایمیج به درستی ساخته نشده است.</li>
                            <li>Stop : سرویس ها در حالت زرد و یا سبز باشد این دستور بعد از چند ثانیه سرویس و کانتینر را حذف خواهد کرد</li>
                            <li>Env : نمایش تنظیمات Template انتخاب شده</li>
                            <li>Commands : هر Template شامل تعدادی Command میباشد که درون کانتینر خود اجرا میگردد و نتیجه آن در ستون Command قابل مشاهده است</li>
                        </ul>
                    </li>
                </ul>
            </div>
        </Modal>
    </div>;
}