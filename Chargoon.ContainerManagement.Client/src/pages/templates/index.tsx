import React, { useEffect, useState } from 'react';
import { Button, Checkbox, Col, Form, Input, message, Modal, Popconfirm, Row } from 'antd';
import { TemplateGetDto } from '../../models';
import { Template_Add, Template_Change, Template_Dupplicate, Template_GetAll, Template_Remove } from '../../services';
import { useForm } from 'antd/lib/form/Form';

declare type Modals = '' | 'form';


export default () => {

    const [id, setId] = useState<number>();
    const [modal, setModal] = useState<Modals>('');
    const [loading, setLoading] = useState(false);
    const [items, setItems] = useState<TemplateGetDto[]>([]);
    const [searchValue, setSearchValue] = useState('');

    const [form] = useForm();

    const load = () => {
        setLoading(true);
        Template_GetAll().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch items failed');
            }
        }).finally(() => setLoading(false))
    }

    const submitForm = (values: any) => {
        try {
            const envs: any = {};
            const splits = values.environments.split('\n');
            for (let i = 0; i < splits.length; i++) {
                const element = splits[i];
                const itemSplit = element.split('=');
                if (itemSplit.length !== 2) continue;
                envs[itemSplit[0]] = itemSplit[1];
            }
            values.environments = envs;
        } catch {
            message.warn('environments invalid format');
            return;
        }

        if (id) {
            Template_Change(id, values).then(res => {
                if (res.data.success) {
                    load();
                    setModal('');
                    message.success('Edit template successful');
                } else {
                    message.error(res.data.message ?? 'Edit template failed');
                }
            });
        } else {
            Template_Add(values).then(res => {
                if (res.data.success) {
                    load();
                    setModal('');
                   message.success('Add template successful');
                } else {
                    message.error(res.data.message ?? 'Add template failed');
                }
            });
        }
    }

    const remove = (template: TemplateGetDto) => {
        setLoading(true);
        Template_Remove(template.id).then(res => {
            if (res.data.success) {
                load();
                message.success('Template delete successfully');
            } else {
                message.error(res.data.message);
            }
        }).finally(() => setLoading(false))
    }

    const dupplicate = (template: TemplateGetDto) => {
        setLoading(true);
        Template_Dupplicate(template.id).then(res => {
            if (res.data.success) {
                load();
                message.success('Template dupplicate successfully');
            } else {
                message.error(res.data.message);
            }
        }).finally(() => setLoading(false))
    }

    const showAdd = () => {
        setId(undefined);
        form.resetFields();
        setModal('form');
    }

    const showEdit = (template: TemplateGetDto) => {
        setId(template.id);
        let envText = '';
        for (const key in template.environments) {
            const value = template.environments[key];
            envText += key + '=' + value + '\n';
        }
        form.setFieldsValue({
            ...template,
            dockerCompose: template.dockerCompose,
            environments: envText,
        });
        setModal('form');
    }

    useEffect(() => {
        load();
    }, []);


    return <div className="page-templates">
        <div className="wrapper">
            <div className="title-bar">
                <h4>Templates</h4>
                <button className="btn btn-primary" onClick={() => load()}>Reload</button>
                <button className="btn btn-success" onClick={() => showAdd()}>Add New Template</button>
                <Input placeholder="Search by name ..." value={searchValue} onChange={e => setSearchValue(e.target.value)} />
            </div>
            <table className={"custom-table" + (loading ? ' loading' : '')}>
                <colgroup>
                    <col width="70px" />
                    <col width="200px" />
                    <col width="200px" />
                    <col width="" />
                    <col width="80px" />
                    <col width="270px" />
                </colgroup>
                <thead>
                    <tr className="table-head">
                        <th>Id</th>
                        <th>Name</th>
                        <th>Insert Cron</th>
                        <th></th>
                        <th>Is Active</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.filter(x => x.name.toLowerCase().indexOf(searchValue.toLowerCase()) > -1).map(template => <tr key={template.id}>
                        <td>{template.id}</td>
                        <td>{template.name}</td>
                        <td>{template.insertCron}</td>
                        <td></td>
                        <td><Checkbox disabled checked={template.isActive}></Checkbox></td>
                        <td>
                            <div className="actions">
                                <button className="btn btn-warning" onClick={() => showEdit(template)}>Edit</button>
                                <Popconfirm title="Are you sure?" onConfirm={() => dupplicate(template)}>
                                    <button className="btn btn-success">Dupplicate</button>
                                </Popconfirm>
                                <Popconfirm title="Are you sure?" onConfirm={() => remove(template)}>
                                    <button className="btn btn-danger">Remove</button>
                                </Popconfirm>
                            </div>
                        </td>
                    </tr>)}
                </tbody>
            </table>
        </div>
        <Modal title="Template Form" visible={modal === 'form'} width="80vw" onCancel={() => setModal('')} footer={null} maskClosable={false}>
            <Form className="floating-label" form={form} onFinish={submitForm}>
                <Row>
                    <Col xs={24} className="p-2">
                        <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                            <Input placeholder="Name" />
                        </Form.Item>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Form.Item name="dockerCompose" label="Docker Compose" rules={[{ required: true }]}>
                            <Input.TextArea placeholder="Docker Compose as yaml" rows={12} />
                        </Form.Item>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Form.Item name="environments" label="Environments" rules={[{ required: true }]}>
                            <Input.TextArea placeholder="Environments in multi lines..." rows={12} />
                        </Form.Item>
                    </Col>
                    <Col xs={24} className="p-2">
                        <Form.Item name="description" label="Description">
                            <Input.TextArea rows={7} />
                        </Form.Item>
                    </Col>
                    <Col xs={24} className="p-2">
                        <Form.Item name="beforeStartCommand" label="Before Start Command">
                            <Input />
                        </Form.Item>
                    </Col>
                    <Col xs={24} className="p-2">
                        <Form.Item name="afterStartCommand" label="After Start Command">
                            <Input />
                        </Form.Item>
                    </Col>
                    <Col xs={24} className="p-2">
                        <Form.Item name="beforeStopCommand" label="Before Stop Command">
                            <Input />
                        </Form.Item>
                    </Col>
                    <Col xs={24} className="p-2">
                        <Form.Item name="afterStopCommand" label="After Stop Command">
                            <Input />
                        </Form.Item>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Form.Item name="insertCron" label="Insert Cron">
                            <Input placeholder="0 6 * * *" />
                        </Form.Item>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Form.Item name="isActive" label="Insert Cron" valuePropName="checked">
                            <Checkbox>Is Active</Checkbox>
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
    </div>;
}