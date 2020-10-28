import React, { useEffect, useState } from 'react';
import { Button, Checkbox, Col, Form, Input, Modal, Row } from 'antd';
import { store as notify } from 'react-notifications-component';
import { notificationOptions } from '../../notification';
import { Loading } from '../../components/loading';
import { BranchGetDto } from '../../models';
import { Branch_Add, Branch_Change, Branch_GetAll } from '../../services';
import { useForm } from 'antd/lib/form/Form';

declare type Modals = '' | 'form';


export default () => {

    const [id, setId] = useState<number>();
    const [modal, setModal] = useState<Modals>('');
    const [isloading, setIsloading] = useState(false);
    const [items, setItems] = useState<BranchGetDto[]>([]);

    const [form] = useForm();

    const load = () => {
        setIsloading(true);
        Branch_GetAll().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Fetch items failed' });
            }
        }).finally(() => setIsloading(false))
    }

    const submitForm = (values: any) => {
        try {
            console.log(values);

            values.dockerCompose = JSON.parse(values.dockerCompose);
        } catch {
            notify.addNotification({ ...notificationOptions, type: 'danger', message: 'Invalid docker compose' });
            return;
        }
        if (id) {
            Branch_Change(id, values).then(res => {
                if (res.data.success) {
                    if (res.data.success) {
                        load();
                        setModal('');
                        notify.addNotification({ ...notificationOptions, type: 'success', message: 'Add user successful' });
                    } else {
                        notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Add user failed' });
                    }
                }
            });
        } else {
            Branch_Add(values).then(res => {
                if (res.data.success) {
                    if (res.data.success) {
                        load();
                        setModal('');
                        notify.addNotification({ ...notificationOptions, type: 'success', message: 'Add user successful' });
                    } else {
                        notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Add user failed' });
                    }
                }
            });
        }
    }

    useEffect(() => {
        load();
    }, []);


    return <div className="page-users">
        {isloading ? <Loading /> : null}
        <div className="wrapper">
            <div className="title-bar">
                <h4>Branches</h4>
                <button className="btn btn-primary" onClick={() => load()}>Reload</button>
                <button className="btn btn-success" onClick={() => { setId(undefined); setModal('form'); }}>Add New Branch</button>
            </div>
            <table className="custom-table">
                <colgroup>
                    <col width="70px" />
                    <col width="" />
                    <col width="150px" />
                    <col width="200px" />
                </colgroup>
                <thead>
                    <tr className="table-head">
                        <th>Id</th>
                        <th>Name</th>
                        <th>Is Build Enable</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(branch => <tr key={branch.id}>
                        <td>{branch.id}</td>
                        <td>{branch.name}</td>
                        <td>{branch.isBranchEnable}</td>
                        <td>
                            <button className="btn btn-warning" onClick={() => {
                                setId(branch.id);
                                form.setFieldsValue({...branch, dockerCompose: JSON.stringify(branch.dockerCompose) });
                                setModal('form');
                            }}>Edit</button>
                        </td>
                    </tr>)}
                </tbody>
            </table>
        </div>
        <Modal title="Create new user" visible={modal === 'form'} onCancel={() => setModal('')} footer={null}>
            <Form className="floating-label" form={form} onFinish={submitForm}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                            <Input placeholder="Name" />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="dockerCompose" label="Docker Compose" rules={[{ required: true }]}>
                            <Input.TextArea placeholder="Json" rows={7} />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="isBuildEnable" valuePropName="checked">
                            <Checkbox>Is Build Enable</Checkbox>
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