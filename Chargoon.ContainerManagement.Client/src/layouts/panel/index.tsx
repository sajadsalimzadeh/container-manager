import React, { useState } from 'react';
import { Link, withRouter } from 'react-router-dom';
import { Auth_HasRole } from '../../services';

interface Props {
    children?: any;
}

export default withRouter((props: Props) => {

    const [username] = useState(localStorage.getItem('username'));

    const hasRole = (value: string) => Auth_HasRole(value);

    return <div className="layout-panel">
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="navbar-brand">
                <img src="assets/images/logo.png" alt="logo"/>
            </div>
            <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarText"
                aria-controls="navbarText" aria-expanded="false" aria-label="Toggle navigation">
                <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbarText">
                <ul className="navbar-nav mr-auto">
                    <li className="nav-item">
                        <Link to="/profile" className="nav-link">
                            <i className="fa fa-user"></i>
                            <span>{username}</span>
                        </Link>
                    </li>
                    {hasRole('user') ? <li className="nav-item">
                        <Link to="/instances" className="nav-link">
                            <i className="fa fa-plug"></i>
                            <span>Instances</span>
                        </Link>
                    </li> : null}
                    {hasRole('admin') ? <li className="nav-item">
                        <Link to="/images" className="nav-link">
                            <i className="fa fa-code-fork"></i>
                            <span>Images</span>
                        </Link>
                    </li> : null}
                    {hasRole('admin') ? <li className="nav-item">
                        <Link to="/templates" className="nav-link">
                            <i className="fa fa-code"></i>
                            <span>Templates</span>
                        </Link>
                    </li> : null}
                    {hasRole('admin') ? <li className="nav-item">
                        <Link to="/users" className="nav-link">
                            <i className="fa fa-users"></i>
                            <span>Users</span>
                        </Link>
                    </li> : null}
                </ul>
                <span className="navbar-text">
                </span>
            </div>
            <div>
                <Link to="/" className="btn btn-danger my-2 my-sm-0">
                    <span>Logout</span>
                    <i className="fa fa-sign-out"></i>
                </Link>
            </div>
        </nav>
        <main>
            {props.children}
        </main>
        <footer>

        </footer>
    </div>;
})