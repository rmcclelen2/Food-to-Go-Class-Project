import "./App.css";
import { Router, Route, Switch, Redirect } from "react-router-dom";
import React, { useState, useMemo, useEffect } from "react";
import "semantic-ui-css/semantic.min.css";
import Home from "./Components/Home";
import Menu from "./Components/Menu";
import Login from "./Components/Login";
import Order from "./Components/Order";
import history from "./history";
import { UserContext } from "./Components/UserContext";
import Orders from "./Components/Orders";
import Schedule from "./Components/Schedule";
import ScrollToTop from "./Components/ScrollToTop";

export default function App() {
  const [loggedIn, setLoggedIn] = useState(null);
  const value = useMemo(() => ({ loggedIn, setLoggedIn }), [
    loggedIn,
    setLoggedIn
  ]);
  useEffect(() => {
    const loggedInUser = localStorage.getItem("user");
    if (loggedInUser) {
      setLoggedIn(loggedInUser);
    }
  }, []);
  return (
    <Router history={history}>
      <ScrollToTop />
      <div className="App">
        <Switch>
          <UserContext.Provider value={value}>
            <Route path="/" component={Home} exact />
            <Route path="/menu" component={Menu} />
            <Route path="/login" component={Login} />
            <Route path="/order" component={Order} />
            {/*<Route path="/adminpanel">*/}
            {/*  {loggedIn ? <Admin /> : <Redirect to="/login" />}*/}
            {/*</Route>*/}
            <Route path="/schedule">
              {loggedIn ? <Schedule /> : <Redirect to="/login" />}
            </Route>
            <Route path="/orders">
              {loggedIn ? <Orders /> : <Redirect to="/login" />}
            </Route>
          </UserContext.Provider>
        </Switch>
      </div>
    </Router>
  );
}
