import React, { Component } from "react";
import { Menu, Button, Container } from "semantic-ui-react";
import { NavLink } from "react-router-dom";
import history from "./../history";

export default class Navbar extends Component {
  state = {};

  handleItemClick = (e, { name }) => this.setState({ activeItem: name });

  render() {
    const { activeItem } = this.state;

    return (
      <Menu
        id="menu-navbar"
        pointing
        secondary
        size="large"
        style={{ padding: "1em 0em", borderColor: "orange" }}
      >
        <Container>
          <NavLink to="/">
            <Menu.Item
              style={{ color: "orange" }}
              name="Home"
              active={activeItem === "Home"}
              onClick={this.handleItemClick}
            >
              Home
            </Menu.Item>
          </NavLink>
          <NavLink to="/menu">
            <Menu.Item
              style={{ color: "orange" }}
              name="Menu"
              active={activeItem === "Menu"}
              onClick={this.handleItemClick}
            >
              Menu
            </Menu.Item>
          </NavLink>
          <Menu.Item
            position={"right"}
            name="Order"
            active={activeItem === "Order"}
          >
            <Button
              color="orange"
              inverted
              variant="btn btn-success"
              onClick={() => history.push("/order")}
            >
              Order
            </Button>
          </Menu.Item>
        </Container>
      </Menu>
    );
  }
}
