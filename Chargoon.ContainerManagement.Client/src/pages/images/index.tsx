import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, InputNumber, message, Modal, Popconfirm, Row } from 'antd';
import { ImageBuildLogDto, ImageGetDto } from '../../models';
import { Image_Add, Image_Change, Image_GetAll, Image_GetAllBuildLogs, Image_GetBuildLogLink, Image_Remove } from '../../services';
import { useForm } from 'antd/lib/form/Form';

declare type Modals = '' | 'form' | 'build-logs';


export default () => {

    const [id, setId] = useState<number>();
    const [modal, setModal] = useState<Modals>('');
    const [loading, setLoading] = useState(false);
    const [items, setItems] = useState<ImageGetDto[]>([]);
    const [buildLogs, setBuildLogs] = useState<ImageBuildLogDto[]>([])
    const [searchValue, setSearchValue] = useState('');
    const [isFormLoading, setIsFormLoading] = useState(false);

    const [form] = useForm();

    const load = () => {
        setLoading(true);
        Image_GetAll().then(res => {
            if (res.data.success) {
                setItems(res.data.data);
            } else {
                message.error(res.data.message ?? 'Fetch items failed');
            }
        }).finally(() => setLoading(false))
    }

    const submitForm = (values: any) => {
        if (id) {
            setIsFormLoading(true);
            Image_Change(id, values).then(res => {
                if (res.data.success) {
                    if (res.data.success) {
                        load();
                        setModal('');
                       message.success('Edit image successful');
                    } else {
                        message.error(res.data.message ?? 'Edit image failed');
                    }
                }
            }).finally(() => setIsFormLoading(false));
        } else {
            setIsFormLoading(true);
            Image_Add(values).then(res => {
                if (res.data.success) {
                    if (res.data.success) {
                        load();
                        setModal('');
                       message.success('Add image successful');
                    } else {
                        message.error(res.data.message ?? 'Add image failed');
                    }
                }
            }).finally(() => setIsFormLoading(false));
        }
    }

    const remove = (image: ImageGetDto) => {
        setLoading(true);
        Image_Remove(image.id).then(res => {
            if (res.data.success) {
                load();
                message.success('Image delete successfully');
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
    
    const showEdit = (image: ImageGetDto) => {
        setId(image.id);
        form.setFieldsValue({ ...image });
        setModal('form');
    }

    const showBuildLogs = (image: ImageGetDto) => {
        setId(image.id);
        setLoading(true);
        Image_GetAllBuildLogs(image.id).then(res => {
            if (res.data.success) {
                setBuildLogs(res.data.data);
            } else {
                message.error(res.data.message);
            }
        }).finally(() => setLoading(false))
        setModal('build-logs');
    }

    useEffect(() => {
        load();
    }, []);


    return <div className="page-images">
        <div className="wrapper">
            <div className="title-bar">
                <h4>Images</h4>
                <button className="btn btn-primary" onClick={() => load()}>Reload</button>
                <button className="btn btn-success" onClick={() => showAdd()}>Add New Image</button>
                <Input placeholder="Search by name ..." value={searchValue} onChange={e => setSearchValue(e.target.value)}/>
            </div>
            <table className={"custom-table" + (loading ? ' loading' : '')}>
                <colgroup>
                    <col width="70px" />
                    <col width="200px" />
                    <col width="" />
                    <col width="200px" />
                    <col width="200px" />
                    <col width="" />
                    <col width="300px" />
                </colgroup>
                <thead>
                    <tr className="table-head">
                        <th>Id</th>
                        <th>Name</th>
                        <th>Build Path</th>
                        <th>Build Cron</th>
                        <th>Life Time</th>
                        <th></th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.filter(x => x.name.toLowerCase().indexOf(searchValue.toLowerCase()) > -1).map(image => <tr key={image.id}>
                        <td>{image.id}</td>
                        <td>{image.name}</td>
                        <td>{image.buildPath}</td>
                        <td>{image.buildCron}</td>
                        <td>{image.lifeTime}</td>
                        <td></td>
                        <td>
                            <div className="actions">
                                <button className="btn btn-warning" onClick={() => showEdit(image)}>Edit</button>
                                <button className="btn btn-info" onClick={() => showBuildLogs(image)}>Build Logs</button>
                                <Popconfirm title="Are you sure?" onConfirm={() => remove(image)}>
                                    <button className="btn btn-danger">Remove</button>
                                </Popconfirm>
                            </div>
                        </td>
                    </tr>)}
                </tbody>
            </table>
        </div>
        <Modal title="Image Form" visible={modal === 'form'} onCancel={() => setModal('')} footer={null}>
            <Form className={"floating-label" + (isFormLoading ? ' loading' : '')} form={form} onFinish={submitForm}>
                <Row>
                    <Col xs={24} className="p-2">
                        <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                            <Input placeholder="Name" />
                        </Form.Item>
                    </Col>
                    <Col xs={24} className="p-2">
                        <Form.Item name="buildPath" label="Build Path" rules={[{ required: true }]}>
                            <Input placeholder="C:// ...." />
                        </Form.Item>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Form.Item name="buildCron" label="Build Cron">
                            <Input placeholder="0 6 * * *" />
                        </Form.Item>
                    </Col>
                    <Col xs={12} className="p-2">
                        <Form.Item name="lifeTime" label="Life Time">
                            <InputNumber placeholder="day" />
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
        <Modal title="Build Logs" visible={modal === 'build-logs'} onCancel={() => setModal('')} footer={null}>
            {id ? <ul>
                {buildLogs.map((log, i) =>
                    <li key={i}>
                        <span>{log.buildName}</span>
                        <ul>
                            {log.scripts.map((script, j) => <li key={j}><a href={Image_GetBuildLogLink(id, log.buildName, script)}>{script}</a></li>)}
                        </ul>
                    </li>
                )}
            </ul> : null}
        </Modal>
    </div>;
}