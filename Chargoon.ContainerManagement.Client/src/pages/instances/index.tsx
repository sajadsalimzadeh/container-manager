import React, { useEffect, useState } from 'react';
import { RouteComponentProps } from "react-router";
import { Loading } from '../../components/loading';
import { InstanceGetDto, TemplateCommandColor, TemplateCommandExecDto, TemplateCommandGetDto, TemplateGetDto } from '../../models';
import { Instance_GetAllOwn, Instance_StartOwn, Instance_StopOwn, Instance_ChangeOwnTemplate, Template_GetAll, Instance_RunCommand, Docker_GetCommandLog, Instance_Signal } from '../../services';
import { store as notify } from 'react-notifications-component';
import { notificationOptions } from '../../notification';
import { Modal, Col, Row, Select, Form, Button, Tooltip, Dropdown, Menu, Input, Spin } from 'antd';
import useInterval from '@use-it/interval';
import { useForm } from 'antd/lib/form/Form';
import useForceUpdate from 'use-force-update';

declare type Modals = '' | 'change-template' | 'help' | 'command-detail';

interface Props extends RouteComponentProps<{}> {

}

export default () => {

    const [isloading, setIsloading] = useState(false);
    const [modal, setModal] = useState<Modals>('');
    const [items, setItems] = useState<InstanceGetDto[]>([]);
    const [templates, setTemplates] = useState<TemplateGetDto[]>([]);
    const [selectedInstance, setSelectedInstance] = useState<InstanceGetDto>();
    const [selectedCommand, setSelectedCommand] = useState<TemplateCommandExecDto>();
    const [changeTemplateForm] = useForm();

    const forceUpdate = useForceUpdate();

    useInterval(() => {
        loadSignal();
    }, 1000);

    const load = () => {
        setIsloading(true);
        Instance_GetAllOwn().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Fetch items failed' });
            }
        }).finally(() => setIsloading(false))
    }

    const startInstance = (instance: InstanceGetDto) => {
        instance.isStarting = true;
        forceUpdate();
        notify.addNotification({ ...notificationOptions, type: 'default', message: 'Starting Instance please wait' });
        Instance_StartOwn(instance.id).then(res => {
            if (res.data.success) {
                instance.isStarting = false;
                forceUpdate();
                notify.addNotification({ ...notificationOptions, type: 'success', message: 'Starting instance is in progress please wait' });
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Starting instance failed' })
            }
        });
    }

    const stopInstance = (instance: InstanceGetDto) => {
        instance.isStopping = true;
        forceUpdate();
        Instance_StopOwn(instance.id).then(res => {
            if (res.data.success) {
                notify.addNotification({ ...notificationOptions, type: 'success', message: 'Stopping instance is in progress please wait' });
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Stopping instance failed' });
            }
        });
    }

    const runCommand = (instance: InstanceGetDto, templateCommand: TemplateCommandGetDto) => {
        templateCommand.isRunning = true;
        forceUpdate();
        Instance_RunCommand(instance.id, templateCommand.id).then(res => {
            if (res.data.success) {
                notify.addNotification({ ...notificationOptions, type: 'success', message: 'Command is running inside container please wait until done' });
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Command running failed' });
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
        Template_GetAll().then(res => {
            if (res.data.success) {
                setTemplates(res.data.data);
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Fetch templates failed' })
            }
        }).finally(() => {

        })
    }

    const submitChangeTemplate = (values: { templateId: number }) => {
        if (selectedInstance) {
            Instance_ChangeOwnTemplate(selectedInstance.id, { templateId: values.templateId }).then(res => {
                if (res.data.success) {
                    load();
                    setModal('');
                    notify.addNotification({ ...notificationOptions, type: 'success', message: 'Change template done successfully' })
                } else {
                    notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Change template failed' })
                }
            }).finally(() => {

            })
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

    const showCommandDetail = (templateCommand: TemplateCommandGetDto, command: TemplateCommandExecDto) => {
        command.templateCommand = templateCommand;
        setSelectedCommand({ ...command, isLoadingLogs: true });
        setModal('command-detail');
        Docker_GetCommandLog(command.commandId).then(res => {
            if (res.data.success) {
                command.logs = res.data.data;
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Fetch command log failed' })
            }
            setSelectedCommand({ ...command, isLoadingLogs: false });
        })
    }

    const loadSignal = () => {
        Instance_Signal().then(res => {
            if (res.data.success) {
                for (let i = 0; i < res.data.data.length; i++) {
                    const signal = res.data.data[i];
                    const instance = items.find(x => x.id == signal.instanceId);
                    if (instance) {
                        instance.services = signal.services;
                        instance.commadns = signal.templateCommandExecs;
                    }
                }
                forceUpdate();
            }
        })
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
        if (!instance.template?.dockerCompose?.services) return [];
        return Object.keys(instance.template.dockerCompose.services);
    }

    const isInstanceServiceRunning = (instance: InstanceGetDto, serviceName: string): number => {
        if (!instance.services) return 0;
        const searchValue = (instance.name + "_" + serviceName).toLowerCase();
        return instance.services.findIndex(x => (x.spec ? x.spec.name.toLowerCase().indexOf(searchValue) > -1 : false)) > -1 ? 1 : -1;
    }

    const isInstanceAllServiceStarted = (instance: InstanceGetDto): boolean => {
        return getInstanceAllService(instance).findIndex((service) => {
            return isInstanceServiceRunning(instance, service) !== 1;
        }) < 0;
    }

    const isInstanceAllServiceStoped = (instance: InstanceGetDto): boolean => {
        return getInstanceAllService(instance).findIndex((service) => {
            return isInstanceServiceRunning(instance, service) === 1;
        }) < 0;
    }

    const isCommandRunning = (instance: InstanceGetDto, templateCommand: TemplateCommandGetDto) => {
        if (!instance.commadns) return false;
        return instance.commadns.findIndex(x => x.templateCommandId === templateCommand.id && x.inspect?.running) > -1;
    }

    const getReplacedDockerCompose = (instance: InstanceGetDto): any => {
        if (instance.template) {
            let dockerComposeJson = JSON.stringify(instance.template.dockerCompose);
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
        let isChange = false;
        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            if (item.isStarting && isInstanceAllServiceStarted(item)) {
                item.isStarting = false;
                isChange = true;
            }

            if (item.isStopping && isInstanceAllServiceStoped(item)) {
                item.isStopping = false;
                isChange = true;
            }
        }
        if (isChange) forceUpdate();
    }, [items])

    const baseUrl = window.location.protocol + '//' + window.location.hostname;

    return <div className="page-instances">
        {isloading ? <Loading /> : null}
        <div className="wrapper">
            <div className="title-bar">
                <h4>Instances</h4>
                <button className="btn btn-primary" onClick={load}>Reload</button>
                <button className="btn btn-info" onClick={() => setModal('help')}>Help</button>
            </div>
            <table className="custom-table">
                <colgroup>
                    <col width="150px" />
                    <col width="180px" />
                    <col />
                    <col width="100px" />
                    <col width="250px" />
                    <col width="320px" />
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
                            <button className={"btn btn-" + (instance.template ? "dark" : "danger")} onClick={() => changeTemplate(instance)}>{(instance.template ? instance.template.name : 'No Template Selected')}</button>
                        </td>
                        <td>
                            <div className="ports">
                                {instance.services?.map((service, i) => {
                                    let dockerComposeService: any;
                                    try {
                                        const dockerCompose = getReplacedDockerCompose(instance);
                                        const serviceNames = Object.keys(dockerCompose.services);
                                        for (let k = 0; k < serviceNames.length; k++) {
                                            const serviceName = serviceNames[k];
                                            if (service.spec.name.toLowerCase().endsWith(serviceName.toLowerCase())) {
                                                dockerComposeService = dockerCompose.services[serviceName];
                                            }
                                        }
                                    } catch { }

                                    return <div key={i}>
                                        {service.endpoint?.ports?.map((port, j) => {
                                            let dockerComposeServicePort: any;
                                            if (dockerComposeService) {
                                                try {
                                                    dockerComposeServicePort = dockerComposeService.ports.find((x: any) => x.published == port.publishedPort);
                                                } catch {

                                                }
                                            }

                                            return <span key={j}>
                                                <a href={`${baseUrl}:${port.publishedPort}`} target="_blank" rel="noopener noreferrer">{port.publishedPort}</a>
                                                :
                                                {dockerComposeServicePort?.name ? dockerComposeServicePort.name : port.targetPort}
                                            </span>;
                                        })}
                                    </div>;
                                })}
                            </div>
                        </td>
                        <td>
                            <div className="services">
                                {getInstanceAllService(instance).map((service, i) => {
                                    const status = isInstanceServiceRunning(instance, service);
                                    return <Tooltip key={i} title={service} placement={'top'}>
                                        <span className={status === 0 ? '' : (status === 1 ? 'running' : 'shutdown')}></span>
                                    </Tooltip>
                                })}
                            </div>
                        </td>
                        <td>
                            <div className="commands">
                                {instance.commadns?.map((tce, i) => {
                                    const tc = instance.template.commands.find(x => x.templateId === tce.templateCommandId);
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
                                {isInstanceAllServiceStarted(instance) || !isInstanceAllServiceStoped(instance) ? <button className="btn btn-danger" disabled={instance.isStopping} onClick={() => stopInstance(instance)}>{instance.isStopping ? 'Stopping...' : 'Stop'}</button> : null}
                                {!isInstanceAllServiceStarted(instance) || isInstanceAllServiceStoped(instance) ? <button className="btn btn-success" disabled={instance.isStarting} onClick={() => startInstance(instance)}>{instance.isStarting ? 'Starting...' : 'Start'}</button> : null}
                                <button className="btn btn-primary" onClick={() => showEnvironments(instance)}>ENV</button>
                                {instance.template?.commands?.length && isInstanceAllServiceStarted(instance) ? <Dropdown overlay={<Menu>
                                    {instance.template?.commands?.map(templateCommand => {
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
        <Modal title="Change template and run another network of containers" visible={modal === 'change-template'} onCancel={() => setModal('')} footer={null}>
            <Form form={changeTemplateForm} onFinish={submitChangeTemplate}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="templateId" label="Template">
                            <Select showSearch allowClear placeholder="Search & Choose Template">
                                {templates.map(template => <Select.Option key={template.id} value={template.id}>{template.name}</Select.Option>)}
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
        <Modal title="Help" visible={modal === 'help'} width="80vw" onCancel={() => setModal('')} footer={null}>
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
        {selectedCommand ? <Modal title="Command Detail" visible={modal === 'command-detail'} width="600px" onCancel={() => setModal('')} footer={null}>
            <Row>
                <Col xs={24}>
                    <Spin tip="Loading..." spinning={selectedCommand.isLoadingLogs}>
                        <Input.TextArea rows={10} value={selectedCommand.logs} />
                    </Spin>
                </Col>
            </Row>
        </Modal> : null}
    </div>;
}