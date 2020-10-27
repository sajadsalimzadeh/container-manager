import React, { lazy, Suspense } from 'react';
import { HashRouter, Route, Switch } from 'react-router-dom';
import { Loading } from './components/loading';
import ReactNotification from 'react-notifications-component'

import Auth from './layouts/auth';
import LayoutPanel from './layouts/panel';

import './App.scss';
import 'jquery/dist/jquery.min.js';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

function App() {

  return (
    <>
      <ReactNotification />
      <HashRouter>
        <Switch>
          <Route path="/" exact component={Auth} />
          <Route>
            <LayoutPanel>
              <Suspense fallback={<Loading />}>
                <Switch>
                  <Route path="/instances" component={lazy(() => import('./pages/instances'))} />
                  <Route path="/users" component={lazy(() => import('./pages/users'))} />
                  <Route path="/profile" component={lazy(() => import('./pages/profile'))} />
                </Switch>
              </Suspense>
            </LayoutPanel>
          </Route>
        </Switch>
      </HashRouter>
    </>
  );
}

export default App;
