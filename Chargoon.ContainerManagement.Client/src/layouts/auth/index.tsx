import React, { useEffect, useState } from 'react';
import { RouteComponentProps, useHistory } from 'react-router';
import { Auth_Login, Auth_SetLoginInfo } from '../../services';
import { Loading } from '../../components/loading';
import Cookies from 'js-cookie';
import { message } from 'antd';

interface Props extends RouteComponentProps<{}> {

}

declare type Form = {
    username: Control,
    password: Control,
};
declare type Validation = { isvalid: boolean, message?: string };

interface Control {
    value: string;
    isdirty?: boolean;
    validate?: (f: Form) => Validation[];
}
function required(value: string): Validation {
    if (!!value?.trim()) return { isvalid: true };
    return { isvalid: false, message: 'this field is required' }
};

export default () => {

    const history = useHistory();
    const [isloading, setIsloading] = useState(false);
    const [form, setForm] = useState<Form>({
        username: {
            value: '',
            validate: (f) => [required(f.username.value)]
        },
        password: {
            value: '',
            validate: (f) => [required(f.password.value)]
        },
    });
    const isValid = (c: Control) => { if (c.validate) return c.validate(form).findIndex(x => !x.isvalid) < 0; return true; }

    const onSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        setIsloading(true);
        Auth_Login({ username: form.username.value, password: form.password.value }).then(res => {
            if (res.data.success) {
                Auth_SetLoginInfo(res.data.data);
                history.push('/instances');
            } else {
                message.error(res.data.message ?? 'Username or password is incorrect');

                form.password.value = '';
                setForm({ ...form });
            }
        }).finally(() => setIsloading(false))
    }

    useEffect(() => {
        Cookies.remove('token');
    }, [])

    return <div className="layout-auth">
        <div className="limiter">
            <div className="container-login">
                <div className="wrap-login">
                    {isloading ? <Loading /> : null}
                    <div className="login-pic js-tilt" data-tilt>
                        <img src="assets/images/login.png" alt="IMG" />
                    </div>
                    <form className="login-form validate-form" autoComplete="off" onSubmit={onSubmit}>
                        <h1 className="login-form-title">
                            Container Management System
                        </h1>
                        <div className={"wrap-input validate-input " + (isValid(form.username) ? 'valid' : 'invalid') + (form.username.isdirty ? ' dirty' : '')} data-validate="Username is required">
                            <input className="input form-control" required type="text" autoFocus placeholder="Username" value={form.username.value} onChange={e => { form.username.value = e.target.value; form.username.isdirty = true; setForm({ ...form }); }} />
                            <span className="focus-input"></span>
                            <span className="symbol-input">
                                <i className="fa fa-user" aria-hidden="true"></i>
                            </span>
                        </div>
                        <div className={"wrap-input validate-input " + (isValid(form.password) ? 'valid' : 'invalid') + (form.password.isdirty ? ' dirty' : '')} data-validate="Password is required">
                            <input className="input form-control" type="password" placeholder="Password" required value={form.password.value} onChange={e => { form.password.value = e.target.value; form.password.isdirty = true; setForm({ ...form }); }} />
                            <span className="focus-input"></span>
                            <span className="symbol-input">
                                <i className="fa fa-lock" aria-hidden="true"></i>
                            </span>
                        </div>
                        <div className="container-login-form-btn">
                            <button className="login-form-btn btn btn-success">
                                Login
                            </button>
                        </div>
                        <div className="text-center p-t-12">
                        </div>
                        <div className="text-center p-t-136">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>;
}