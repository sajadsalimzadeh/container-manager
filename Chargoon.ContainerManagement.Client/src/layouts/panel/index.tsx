import React, { useEffect, useState } from 'react';
import { Link, withRouter } from 'react-router-dom';
import { Auth_HasRole } from '../../services';

interface Props {
    children?: any;
}

export default withRouter((props: Props) => {

    const [username] = useState(localStorage.getItem('username'));

    const hasRole = (value: string) => Auth_HasRole(value);

    return <div className="layout-panel">
        <nav className="navbar navbar-expand-lg navbar-light bg-light">
            <a className="navbar-brand" href="#">
                <img src="assets/images/logo.png" />
            </a>
            <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarText"
                aria-controls="navbarText" aria-expanded="false" aria-label="Toggle navigation">
                <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbarText">
                <ul className="navbar-nav mr-auto">
                    <li className="nav-item">
                        <Link to="/profile" className="nav-link">{username}</Link>
                    </li>
                    {hasRole('user') ? <li className="nav-item">
                        <Link to="/instances" className="nav-link">Instances</Link>
                    </li> : null}
                    {hasRole('admin') ? <li className="nav-item">
                        <Link to="/users" className="nav-link">Users</Link>
                    </li> : null}
                    <li className="nav-item">
                        <Link to="/" className="nav-link">Logout</Link>
                    </li>
                </ul>
                <span className="navbar-text">
                </span>
            </div>
        </nav>
        <main>
            {props.children}
        </main>
        <footer>

        </footer>
    </div>;
})