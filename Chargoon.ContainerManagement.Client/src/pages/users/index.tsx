import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, message, Modal, Row, Select } from 'antd';
import { RouteComponentProps, useHistory } from "react-router";
import { InstanceGetDto, UserGetDto } from '../../models';
import { Auth_ChangeUser, Auth_SetLoginInfo, Instance_Add, Instance_Remove, User_Add, User_GetAll, User_ResetPassword } from '../../services';
import useForceUpdate from 'use-force-update';

declare type Modals = '' | 'add-user' | 'add-instance';

interface Props extends RouteComponentProps<{}> {

}

export default () => {

    const history = useHistory();
    const [modal, setModal] = useState<Modals>('');
    const [loading, setLoading] = useState(false);
    const [items, setItems] = useState<UserGetDto[]>([]);
    const [selectedUser, setSelectedUser] = useState<UserGetDto>();
    const [searchValue, setSearchValue] = useState('');

    const forceUpdate = useForceUpdate();

    const load = () => {
        setLoading(true);
        User_GetAll().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch items failed');
            }
        }).finally(() => setLoading(false))
    }

    const submitAddUser = (values: any) => {
        setLoading(true);
        User_Add(values).then(res => {
            if (res.data.success) {
                if (res.data.success) {
                    load();
                    setModal('');
                   message.success('Add user successful');
                } else {
                    message.error(res.data.message ?? 'Add user failed');
                }
            }
        }).finally(() => setLoading(false));
    }

    const submitAddInstance = (values: any) => {
        if (!selectedUser) return;
        values.userId = selectedUser.id;
        setLoading(true);
        Instance_Add(values).then(res => {
            if (res.data.success) {
                load();
                setModal('');
               message.success('Add instance successful');
            } else {
                message.error(res.data.message ?? 'Add instance failed');
            }
        }).finally(() => setLoading(false));
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
            setLoading(true);
            Instance_Remove(instance.id).then(res => {
                if (res.data.success) {
                    load();
                   message.success('Remove instance successful');
                } else {
                    message.error(res.data.message ?? 'Add instance failed');
                }
                instance.isRemoving = false;
                forceUpdate();
            }).finally(() => setLoading(false));
        }
    }

    const changeUser = (id: number) => {
        setLoading(true);
        Auth_ChangeUser(id).then(res => {
            if (res.data.success) {
                history.push('/instances');
                window.location.reload();
                Auth_SetLoginInfo(res.data.data);
               message.success('Change user done successfully');
            } else {
                message.error(res.data.message ?? 'Change user failed');
            }
        }).finally(() => setLoading(false));
    }

    const resetPassword = (id: number) => {
        const password = window.prompt('Enter new password : ');
        if (!password) {
            message.error('Reset Password aborted');
            return;
        }

        setLoading(true);
        User_ResetPassword(id, { newPassword: password }).then(res => {
            if (res.data.success) {
               message.success('Reset user password done successfully');
            } else {
                message.error(res.data.message ?? 'Reset user password failed');
            }
        }).finally(() => setLoading(false));
    }

    useEffect(() => {
        load();
    }, []);

    const roles = ["Admin", "User"]

    return <div className="page-users">
        <div className="wrapper">
            <div className="title-bar">
                <h4>Users</h4>
                <button className="btn btn-primary" onClick={() => load()}>Reload</button>
                <button className="btn btn-success" onClick={() => setModal('add-user')}>Add New User</button>
                <Input placeholder="Search by username ..." value={searchValue} onChange={e => setSearchValue(e.target.value)}/>
            </div>
            <table className={"custom-table" + (loading ? ' loading' : '')}>
                <colgroup>
                    <col width="70px" />
                    <col width="120px" />
                    <col width="170px" />
                    <col width="" />
                    <col width="360px" />
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
                    {items.filter(x => x.username.toLowerCase().indexOf(searchValue.toLowerCase()) > -1).map(user => <tr key={user.id}>
                        <td>{user.id}</td>
                        <td>{user.username}</td>
                        <td>{user.roles.join(',')}</td>
                        <td>
                            <div className="instances">
                                {user.instances.map(instance => <div key={instance.id}>
                                    <span className="badge badge-dark">name: {instance.name}</span>
                                    <span className="badge badge-dark">template: {instance.template?.name}</span>
                                    <span className="badge badge-dark">port: {instance.environments.BASE_PORT}*</span>
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
                                <button className="btn btn-dark" onClick={() => resetPassword(user.id)}>Reset Password</button>
                            </div>
                        </td>
                    </tr>)}
                </tbody>
            </table>
        </div>
        <Modal title="User Form" visible={modal === 'add-user'} onCancel={() => setModal('')} footer={null} maskClosable={false}>
            <Form className="floating-label" onFinish={submitAddUser}>
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