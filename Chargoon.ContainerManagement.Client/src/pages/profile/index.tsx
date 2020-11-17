import React, { useCallback, useEffect, useState } from 'react';
import { User_ChangeOwnPassword, User_GetOwn } from '../../services';
import { Button, Col, Form, Input, message, Modal, Row } from 'antd';
import { useForm } from 'antd/lib/form/Form';

declare type Modals = '' | 'change-password';


export default () => {

    const [modal, setModal] = useState<Modals>('');

    const [form] = useForm();
    const [changePasswordForm] = useForm();

    const load = useCallback(() => {
        User_GetOwn().then(res => {
            if (res.data.success) {
                form.setFieldsValue(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch model failed');
            }
        })
    }, [form]);

    const submitChangePassword = (values: { currentPassword: string, newPassword: string, repeatPassword: string }) => {
        if (values.newPassword !== values.repeatPassword) {
            message.warn('New And Repeat Password is not same');
        }
        User_ChangeOwnPassword({currentPassword: values.currentPassword, newPassword: values.newPassword}).then(res => {
            if(res.data.success) {
                setModal('');
               message.success(res.data.message ?? 'Password successfully changed');
            } else {
                message.error(res.data.message ?? 'Change password failed');
            }
        });
    }

    useEffect(() => {
        load();
    }, [load])

    return <div className="page-profile">
        <div className="card p-5">
            <div className="avatar-box">
                <img className="avatar" src="assets/images/avatar.jpg" alt="avatar"/>
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
        <Modal title="Change Password" visible={modal === 'change-password'} onCancel={() => setModal('')} footer={null} maskClosable={false}>
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