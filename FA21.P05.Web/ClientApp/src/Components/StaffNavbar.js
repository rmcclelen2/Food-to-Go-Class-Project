import React, { useContext, useState, useEffect } from "react";
import { Menu, Button, Container } from "semantic-ui-react";
import { NavLink, useHistory } from "react-router-dom";
import { UserContext } from "../Components/UserContext";
import axios from "axios";

export default function StaffNavbar() {
  const { setLoggedIn } = useContext(UserContext);
  const history = useHistory();
  const navigateTo = () => history.push("/login");
  const [userData, setUserData] = useState([]);
  const [activeItem] = useState(history.location.pathname.replace("/", ""));

  useEffect(() => {
    axios.get("api/authentication/me").then(response => {
      setUserData(response.data);
    });
  }, []);

  function redirectToLogin() {
    setLoggedIn(null);
    localStorage.clear("user");
    navigateTo();
  }
  if (userData.role === "Admin") {
    return (
      <Menu id="menu-navbar" pointing size="large">
        <Container>
          <NavLink to="/">
            <Menu.Item
              name="Home"
              active={activeItem === "Home"}
              color="orange"
            >
              Home
            </Menu.Item>
          </NavLink>
          <NavLink to="/orders">
            <Menu.Item
              name="Orders"
              active={activeItem === "orders"}
              color="orange"
            >
              Orders
            </Menu.Item>
          </NavLink>
          <NavLink to="/schedule">
            <Menu.Item
              name="Schedule"
              active={activeItem === "login" || activeItem === "schedule"}
              color="orange"
            >
              Schedule
            </Menu.Item>
          </NavLink>

          {/*<NavLink to="/adminpanel">*/}
          {/*  <Menu.Item*/}
          {/*    name="Admin"*/}
          {/*    active={activeItem === "adminpanel"}*/}
          {/*    color="orange"*/}
          {/*  >*/}
          {/*    Admin Panel*/}
          {/*  </Menu.Item>*/}
          {/*</NavLink>*/}
          <Menu.Item position={"right"}>
            <Button
              color="orange"
              onClick={() => {
                redirectToLogin();
              }}
            >
              Logout
            </Button>
          </Menu.Item>
        </Container>
      </Menu>
    );
  } else {
    return (
      <Menu id="menu-navbar" pointing secondary size="large">
        <Container>
          <NavLink to="/">
            <Menu.Item
              name="Home"
              active={activeItem === "Home"}
              color="orange"
            >
              Home
            </Menu.Item>
          </NavLink>
          <NavLink to="/orders">
            <Menu.Item
              name="Orders"
              active={activeItem === "orders"}
              color="orange"
            >
              Orders
            </Menu.Item>
          </NavLink>
          <NavLink to="/schedule">
            <Menu.Item
              name="Schedule"
              active={activeItem === "schedule"}
              color="orange"
            >
              Schedule
            </Menu.Item>
          </NavLink>
          <Menu.Item position={"right"}>
            <Button
              color="orange"
              onClick={() => {
                redirectToLogin();
              }}
            >
              Logout
            </Button>
          </Menu.Item>
        </Container>
      </Menu>
    );
  }
}
