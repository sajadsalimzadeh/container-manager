import React, { lazy, Suspense, useEffect, useState } from 'react';
import { HashRouter, Route, Switch } from 'react-router-dom';
import { Loading } from './components/loading';
import { Init } from './services';
import { message } from 'antd';
import Auth from './layouts/auth';
import LayoutPanel from './layouts/panel';

import './App.scss';
import 'jquery/dist/jquery.min.js';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

function App() {

  const [isLoad, setIsLoad] = useState(false)

  useEffect(() => {
    const version = '[AIV]{version}[/AIV]';
    Init(`assets/config.json?${version}`).then(res => {
      setIsLoad(true);
    });

    message.config({
      duration: 5
    });
  }, []);

  if (!isLoad) return <Loading />;

  return (
    <>
      <HashRouter>
        <Switch>
          <Route path="/" exact component={Auth} />
          <Route>
            <LayoutPanel>
              <Suspense fallback={<Loading />}>
                <Switch>
                  <Route path="/profile" component={lazy(() => import('./pages/profile'))} />
                  <Route path="/instances" component={lazy(() => import('./pages/instances'))} />
                  <Route path="/users" component={lazy(() => import('./pages/users'))} />
                  <Route path="/images" component={lazy(() => import('./pages/images'))} />
                  <Route path="/templates" component={lazy(() => import('./pages/templates'))} />
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
