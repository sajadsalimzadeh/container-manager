import React, { useEffect, useState } from 'react';
import { InstanceGetDto, TemplateCommandColor, TemplateCommandExecDto, TemplateCommandGetDto, TemplateGetDto } from '../../models';
import { Instance_GetAllOwn, Instance_StartOwn, Instance_StopOwn, Instance_ChangeOwnTemplate, Template_GetAll, Instance_RunCommand, Docker_GetCommandLog, Instance_Signal } from '../../services';
import { Modal, Col, Row, Select, Form, Button, Tooltip, Dropdown, Menu, message } from 'antd';
import useInterval from '@use-it/interval';
import { useForm } from 'antd/lib/form/Form';
import useForceUpdate from 'use-force-update';
import FileSaver from 'file-saver';

declare type Modals = '' | 'change-template' | 'help';


export default () => {

    const [loading, setLoading] = useState(false);
    const [modal, setModal] = useState<Modals>('');
    const [items, setItems] = useState<InstanceGetDto[]>([]);
    const [templates, setTemplates] = useState<TemplateGetDto[]>([]);
    const [selectedInstance, setSelectedInstance] = useState<InstanceGetDto>();
    const [changeTemplateForm] = useForm();
    const [isFormLoading, setIsFormLoading] = useState(false);
    const [isStartInstance, setIsStartInstance] = useState(false);

    const [isOnScreen, setIsOnScreen] = useState(true);
    const [isSignalOnLoading, setIsSignalOnLoading] = useState(false);

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
        changeTemplateForm.setFieldsValue({ templateId: instance.templateId });
        setIsFormLoading(true);
        Template_GetAll().then(res => {
            if (res.data.success) {
                setTemplates(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch templates failed')
            }
        }).finally(() => setIsFormLoading(false))
    }

    const submitChangeTemplate = (values: { templateId: number }) => {
        if (selectedInstance) {
            setIsFormLoading(true);
            Instance_ChangeOwnTemplate(selectedInstance.id, { templateId: values.templateId }).then(res => {
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

    const getServiceState = (instance: InstanceGetDto, serviceName: string): number => {
        if (!instance.services || !serviceName) return 0;
        const searchValue = (instance.name + "_" + serviceName).toLowerCase();
        return instance.services.findIndex(x => (x.spec ? x.spec.name.toLowerCase().indexOf(searchValue) > -1 : false)) > -1 ? 1 : -1;
    }

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

    const isInstanceAllServiceStoped = (instance: InstanceGetDto): boolean => {
        return getInstanceAllService(instance).findIndex((service) => {
            return getServiceState(instance, service) === 1;
        }) < 0;
    }

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

    const getReplacedDockerCompose = (instance: InstanceGetDto): any => {
        if (instance.template) {
            let dockerComposeJson = JSON.stringify(instance.template.dockerComposeObj);
            for (const key in instance.environments) {
                const value = instance.environments[key];
                while (dockerComposeJson.indexOf(`{${key}}`) > -1) dockerComposeJson = dockerComposeJson.replace(`{${key}}`, value);
            }
            return JSON.parse(dockerComposeJson);
        }
    }

    useEffect(() => {
        load();
    }, []);

    useEffect(() => {

    }, [items])

    const baseUrl = 'http://docker-srv';

    return <div className="page-instances" onMouseEnter={() => setIsOnScreen(true)} onMouseLeave={() => setIsOnScreen(false)}>
        <div className="wrapper">
            <div className="title-bar">
                <h4>Instances</h4>
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
                                {instance.services?.map((service, i) => {
                                    let dockerComposeService: any;
                                    let dockerComposeServiceName: any;
                                    try {
                                        const dockerCompose = getReplacedDockerCompose(instance);
                                        const serviceNames = Object.keys(dockerCompose.services);
                                        for (let k = 0; k < serviceNames.length; k++) {
                                            const serviceName = serviceNames[k];
                                            if (service.spec.name.toLowerCase().endsWith(serviceName.toLowerCase())) {
                                                dockerComposeService = dockerCompose.services[serviceName];
                                                dockerComposeServiceName = serviceName;
                                                break;
                                            }
                                        }
                                    } catch { }

                                    return <div className="service" key={i}>
                                        {service.endpoint?.ports?.map((port, j) => {
                                            // let dockerComposeServicePort: any;
                                            // if (dockerComposeService) {
                                            //     try {
                                            //         dockerComposeServicePort = dockerComposeService.ports.find((x: any) => x.published === port.publishedPort);
                                            //     } catch {

                                            //     }
                                            // }
                                            const containerState = (dockerComposeServiceName ? getContainerState(instance, dockerComposeServiceName) : -1);

                                            let portname = '';
                                            for (const key in instance.environments) {
                                                const value = instance.environments[key];
                                                if (value?.toString() === port?.publishedPort?.toString()) {
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
                                                    {containerState === 1 ?
                                                        <a href={`${baseUrl}:${port.publishedPort}`} target="_blank" rel="noopener noreferrer">{port.publishedPort}</a>
                                                        : port.publishedPort}
                                                    {/* :
                                                    {dockerComposeServicePort?.name ? dockerComposeServicePort.name : port.targetPort} */}
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
                                    const serviceState = getServiceState(instance, service);
                                    const containerState = getContainerState(instance, service);
                                    return <Tooltip key={i} title={<>
                                        <div>Name : {service}</div>
                                        <div>Service State: {(serviceState === 1 ? 'Running' : (serviceState === 0 ? 'NA' : 'Down'))}</div>
                                        <div>Container State : {(containerState === 1 ? 'Running' : (containerState === 0 ? 'NA' : 'Down'))}</div>
                                    </>} placement={'top'}>
                                        <span className={
                                            (containerState === 1 ? ' container-running' : '') +
                                            (serviceState === 1 ? ' service-running' : '') +
                                            (serviceState === 0 || containerState === 0 ? '' : ' shutdown')
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
                                {isInstanceAllContainerStarted(instance) || !isInstanceAllServiceStoped(instance) ? <button className="btn btn-danger" disabled={instance.isStopping} onClick={() => stopInstance(instance)}>{instance.isStopping ? 'Stopping...' : 'Stop'}</button> : null}
                                {!isInstanceAllContainerStarted(instance) || isInstanceAllContainerStoped(instance) ? <button className="btn btn-success" disabled={instance.isStarting} onClick={() => startInstance(instance)}>{instance.isStarting ? 'Starting...' : 'Start'}</button> : null}

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
        <Modal title="Change template and run another network of containers" visible={modal === 'change-template'} onCancel={() => setModal('')} footer={null} maskClosable={false}>
            <Form className={(isFormLoading ? ' loading' : '')} form={changeTemplateForm} onFinish={submitChangeTemplate}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="templateId" label="Template">
                            <Select showSearch allowClear placeholder="Search & Choose Template"
                                filterOption={(input, option) => {
                                    if (typeof option?.children === 'string') return option?.children.toLowerCase().indexOf(input.toLowerCase()) >= 0;
                                    return true;
                                }}>
                                {templates?.filter(x => x.isActive).map(template => <Select.Option key={template.id} value={template.id}>{template.name}</Select.Option>)}
                            </Select>
                        </Form.Item>
                    </Col>
                </Row>
                <Row>
                    <Col xs={12} className="p-2">
                        <Button type="primary" htmlType="submit" block>Save</Button>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Button type="primary" htmlType="button" block onClick={() => setModal('')} danger>Close</Button>
                    </Col>
                </Row>
            </Form>
        </Modal>
        <Modal title="Help" visible={modal === 'help'} width="80vw" onCancel={() => setModal('')} footer={null} maskClosable={false}>
            <div className="alert alert-warning rtl" role="alert">
                <h4 className="alert-heading">راهنمای استفاده از سرویس</h4>
                <p>برای استفاده از سرویس مدیریت کانتینر های داکر موارد زیر را مطالعه فرمایید</p>
                <hr />
                <ul>
                    <li>ستون File Manager برای مدیریت فایل های درون کانتینر میباشد و در صورتی که در دسترس نبود میتوانید در منوی More / Run File Manager را اجرا نمایید.</li>
                    <li>برای استفاده از دیتابیس مقدار docker-srv, port را در Sql Server Management Studio وارد نمایید.</li>
                    <li>توسعه دهندگان بک اند در صورتی که IIS فایل ها را لاک کرده بود میتوانید از امکان Start , Stop App Pools استفاده نمایید.</li>
                    <li>درصورتی که میخواهید تغییرات جدید بر روی دیتابیس اعمال گردد از گزینه ی More / Update Database استفاده نمایید. ستون DB State نمایشگر وضعیت دیتابیس میباشد. (این دستور حداقل 15 دقیقه زمان نیاز خواهد داشت)</li>
                    <li>
                        <span>کانتینر ها میتوانند به حالات مختلفی اجرا گردند که به شرح  زیر میباشد</span>
                        <ul>
                            <li>db : فقط دیتابیس که شامل دیتا های قدیم در وضعیت آخرین بروزرسانی</li>
                            <li>db-isolate : فقط دیتابیس به صورت خام و بدون دیتا های گذشته در وضعیت تاریخ Image</li>
                            <li>full : هم دیدگاه هم دیتابیس در وضعیت آخرین بروزرسانی دیتابیس</li>
                            <li>full-isolate : هم دیدگاه هم دیتابیس در وضعیت تاریخ Image</li>
                        </ul>
                    </li>
                    <li>برای تغییر تاریخ و برنچ خود ابتدا کانتینر مربوطه را Stop کرده سپس با زدن روی دکمه ستون Image و انتخاب یکی از Image ها و Save Change میتوانید Image خود را به تاریخ و برنچ مربوطه تغییر دهید و مجددا کانتینر را Start نمایید</li>
                </ul>
            </div>
        </Modal>
    </div>;
}