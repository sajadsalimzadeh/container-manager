import React, { useEffect, useState } from 'react';
import { RouteComponentProps } from "react-router";
import { UserGetDto } from '../../models';
import { User_ChangeOwnPassword, User_GetOwn } from '../../services';
import { store as notify } from 'react-notifications-component';
import { notificationOptions } from '../../notification';
import { Button, Col, Form, Input, Modal, Row } from 'antd';
import { useForm } from 'antd/lib/form/Form';

declare type Modals = '' | 'change-password';

interface Props extends RouteComponentProps<{}> {

}

export default (props: Props) => {

    const [user, setUser] = useState<UserGetDto>();
    const [modal, setModal] = useState<Modals>('');

    const [form] = useForm();
    const [changePasswordForm] = useForm();

    const load = () => {
        User_GetOwn().then(res => {
            if (res.data.success) {
                setUser(res.data.data);
                form.setFieldsValue(res.data.data);
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Fetch model failed' });
            }
        })
    }

    const submitChangePassword = (values: { currentPassword: string, newPassword: string, repeatPassword: string }) => {
        if (values.newPassword !== values.repeatPassword) {
            notify.addNotification({ ...notificationOptions, type: 'warning', message: 'New And Repeat Password is not same' });
        }
        User_ChangeOwnPassword({currentPassword: values.currentPassword, newPassword: values.newPassword}).then(res => {
            if(res.data.success) {
                setModal('');
                notify.addNotification({ ...notificationOptions, type: 'success', message: res.data.message ?? 'Password successfully changed' });
            } else {
                notify.addNotification({ ...notificationOptions, type: 'danger', message: res.data.message ?? 'Change password failed' });
            }
        });
    }

    useEffect(() => {
        load();
    }, [])

    return <div className="page-profile">
        <div className="card p-5">
            <div className="avatar-box">
                <img className="avatar" src="assets/images/avatar.jpg" />
            </div>
            <Form className="floating-label pt-4" form={form}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="id" label="ID">
                            <Input disabled />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="username" label="Username">
                            <Input disabled />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Button type="default" block onClick={() => setModal('change-password')}>Change Password ...</Button>
                    </Col>
                </Row>
            </Form>
        </div>
        <Modal title="Change Password" visible={modal == 'change-password'} onCancel={() => setModal('')} footer={null}>
            <Form className="floating-label" form={changePasswordForm} onFinish={submitChangePassword}>
                <Row>
                    <Col xs={24}>
                        <Form.Item name="currentPassword" label="Current Password" labelCol={{ xs: 8 }} labelAlign="left" rules={[{required: true}]}>
                            <Input type="password" placeholder="************" />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="newPassword" label="New Password" labelCol={{ xs: 8 }} labelAlign="left" rules={[{required: true}]}>
                            <Input type="password" placeholder="************" />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Form.Item name="repeatPassword" label="Repeat Password" labelCol={{ xs: 8 }} labelAlign="left" rules={[{required: true}]}>
                            <Input type="password" placeholder="************" />
                        </Form.Item>
                    </Col>
                    <Col xs={24}>
                        <Button type="primary" htmlType="submit" block>Save</Button>
                    </Col>
                </Row>
            </Form>
        </Modal>
    </div>;
}