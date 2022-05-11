import React from "react";
import EntreeList from "./AxiosComponents/EntreeList";
import DrinkList from "./AxiosComponents/DrinkList";
import SpecialList from "./AxiosComponents/SpecialList";
import SideList from "./AxiosComponents/SideList";
import Navbar from "./Navbar";
import { Segment, Grid, Header, List, Container } from "semantic-ui-react";
import { NavLink } from "react-router-dom";

const Menu = () => {
  let header_style = { fontSize: 46 };
  let para_style = { fontSize: 20 };

  return (
    <div style={{ backgroundColor: "#282c34", color: "orange" }}>
      <Navbar />
      <div className="Menu">
        <h1 className="page-header" style={header_style}>
          Menu
        </h1>
        <hr color="orange" width="50%" />
        <br />
        <b>
          <p align="center" style={para_style}>
            Specials
          </p>
        </b>
        <table width="60%" align="center">
          <SpecialList />
        </table>
        <br />
        <hr color="orange" width="80%" />
        <br />
        <b>
          <p align="center" style={para_style}>
            Entrees
          </p>
        </b>
        <table width="60%" align="center">
          <EntreeList />
        </table>
        <br />
        <hr color="orange" width="80%" align="center" />
        <br />
        <b>
          <p align="center" style={para_style}>
            Sides
          </p>
        </b>
        <table width="60%" align="center">
          <SideList />
        </table>
        <br />
        <hr color="orange" width="80%" align="center" />
        <br />
        <b>
          <p align="center" style={para_style}>
            Drinks
          </p>
        </b>
        <table width="60%" align="center">
          <DrinkList />
        </table>

        <Segment inverted vertical style={{ padding: "5em 0em" }}>
          <Container>
            <Grid.Row>
              <Grid.Column width={3}>
                <Header inverted as="h4" content="Employees" />
                <List link inverted>
                  <NavLink to="/login">
                    <List.Item as="a">Employee Login</List.Item>
                  </NavLink>
                  <List.Item as="a">Contact Us</List.Item>
                  <List.Item as="a" />
                </List>
              </Grid.Column>
            </Grid.Row>
          </Container>
        </Segment>
      </div>
    </div>
  );
};

export default Menu;
