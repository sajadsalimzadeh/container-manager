import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, Modal, Row, Select } from 'antd';
import { RouteComponentProps, useHistory } from "react-router";
import { store as notify } from 'react-notifications-component';
import { notificationOptions } from '../../notification';
import { Loading } from '../../components/loading';
import { InstanceGetDto, UserGetDto } from '../../models';
import { Auth_ChangeUser, Auth_SetLoginInfo, Instance_Add, Instance_Remove, User_Add, User_GetAll } from '../../services';
import useForceUpdate from 'use-force-update';

declare type Modals = '' | 'add-user' | 'add-instance';

interface Props extends RouteComponentProps<{}> {

}

export default (props: Props) => {

    const history = useHistory();
    const [modal, setModal] = useState<Modals>('');
    const [isloading, setIsloading] = useState(false);
    const [items, setItems] = useState<UserGetDto[]>([]);
    const [selectedUser, setSelectedUser] = useState<UserGetDto>();

    const forceUpdate = useForceUpdate();

    const load = () => {
        setIsloading(true);
        User_GetAll().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Fetch items failed' });
            }
        }).finally(() => setIsloading(false))
    }

    const submitAddUser = (values: any) => {
        User_Add(values).then(res => {
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

    const submitAddInstance = (values: any) => {
        if (!selectedUser) return;
        values.userId = selectedUser.id
        Instance_Add(values).then(res => {
            if (res.data.success) {
                load();
                setModal('');
                notify.addNotification({ ...notificationOptions, type: 'success', message: 'Add instance successful' });
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Add instance failed' });
            }
        });
    }

    const showEnvironments = (instance: InstanceGetDto) => {
        const result = [];
        for (const key in instance.environments) {
            const value = instance.environments[key];
            result.push(`${key}=${value}`);
        }
        alert(result.join('\n'));
    }

    const removeInstance = (instance: InstanceGetDto) => {
        if (window.confirm(`Are you sure to remove this instance ?`)) {
            instance.isRemoving = true;
            forceUpdate();
            Instance_Remove(instance.id).then(res => {
                if (res.data.success) {
                    load();
                    notify.addNotification({ ...notificationOptions, type: 'success', message: 'Remove instance successful' });
                } else {
                    notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Add instance failed' });
                }
                instance.isRemoving = false;
                forceUpdate();
            });
        }
    }

    const changeUser = (id: number) => {
        Auth_ChangeUser(id).then(res => {
            if (res.data.success) {
                history.push('/instances');
                Auth_SetLoginInfo(res.data.data);
                notify.addNotification({ ...notificationOptions, type: 'success', message: 'Change user done successfully' });
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Change user failed' });
            }
        })
    }

    useEffect(() => {
        load();
    }, []);

    const roles = ["Admin", "User"]

    return <div className="page-users">
        {isloading ? <Loading /> : null}
        <div className="wrapper">
            <div className="title-bar">
                <h4>Users</h4>
                <button className="btn btn-primary" onClick={() => load()}>Reload</button>
                <button className="btn btn-success" onClick={() => setModal('add-user')}>Add New User</button>
            </div>
            <table className="custom-table">
                <colgroup>
                    <col width="70px" />
                    <col width="150px" />
                    <col width="170px" />
                    <col width="" />
                    <col width="250px" />
                </colgroup>
                <thead>
                    <tr className="table-head">
                        <th>Id</th>
                        <th>Username</th>
                        <th>Roles</th>
                        <th>Instances</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map(user => <tr key={user.id}>
                        <td>{user.id}</td>
                        <td>{user.username}</td>
                        <td>{user.roles.join(',')}</td>
                        <td>
                            <div className="instances">
                                {user.instances.map(instance => <div key={instance.id}>
                                    <span className="badge badge-dark">name: {instance.name}</span>
                                    <span className="badge badge-dark">port: {instance.environments.BASE_PORT}**</span>
                                    <button className="btn badge badge-primary" onClick={() => showEnvironments(instance)}>Environments</button>
                                    <button className="btn badge badge-danger" onClick={() => removeInstance(instance)} disabled={instance.isRemoving}>
                                        {instance.isRemoving ?
                                            <>
                                                <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                                                <span> Loading...</span>
                                            </>
                                            : 'Remove'
                                        }
                                    </button>
                                </div>)}
                            </div>
                        </td>
                        <td>
                            <div className="actions">
                                <button className="btn btn-warning" onClick={() => changeUser(user.id)}>Change User</button>
                                <button className="btn btn-success" onClick={() => { setSelectedUser(user); setModal('add-instance') }}>Add Instance</button>
                            </div>
                        </td>
                    </tr>)}
                </tbody>
            </table>
        </div>
        <Modal title="Create new user" visible={modal === 'add-user'} onCancel={() => setModal('')} footer={null}>
            <Form onFinish={submitAddUser}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="username" label="Username" rules={[{ required: true }]}>
                            <Input placeholder="Username" />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="password" label="Password" rules={[{ required: true }]}>
                            <Input placeholder="Password" type="password" />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="roles" label="Roles" rules={[{ required: true }]}>
                            <Select mode="multiple" showSearch allowClear placeholder="Select one or more roles">
                                {roles.map(role => <Select.Option key={role} value={role}>{role}</Select.Option>)}
                            </Select>
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="instances" label="Instances" rules={[{ required: true }, { pattern: /([a-zA-Z]+(,|))*/, message: "incorect format ([a-zA-Z]+(,|)) (Default,Master,...)" }]}>
                            <Input placeholder="Instances" />
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
        <Modal title="Add new instance" visible={modal === 'add-instance'} onCancel={() => setModal('')} footer={null}>
            <Form onFinish={submitAddInstance}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="name" label="name" rules={[{ required: true }]}>
                            <Input placeholder="Name" />
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