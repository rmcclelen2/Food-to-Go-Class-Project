import { createMedia } from "@artsy/fresnel";
import React, { Component } from "react";
import {
  Button,
  Container,
  Divider,
  Embed,
  Grid,
  Header,
  Icon,
  List,
  Menu,
  Segment,
  Sidebar,
  Visibility
} from "semantic-ui-react";
import { NavLink } from "react-router-dom";
import PropTypes from "prop-types";

const { MediaContextProvider, Media } = createMedia({
  breakpoints: {
    mobile: 0,
    tablet: 768,
    computer: 1024
  }
});

const HomepageHeading = ({ mobile }) => (
  <Container text>
    <Header
      as="h1"
      content="FoodToGo"
      inverted
      style={{
        fontSize: mobile ? "2em" : "4em",
        fontWeight: "normal",
        marginTop: mobile ? "1.5em" : "3em"
      }}
    />
    <Header
      as="h2"
      content="For all your food needs."
      inverted
      style={{
        fontSize: mobile ? "1.5em" : "1.7em",
        fontWeight: "normal",
        marginTop: mobile ? "0.5em" : "1.5em"
      }}
    />
    <NavLink to="/menu">
      <Button size="huge" color="orange">
        Place Order
        <Icon name="right arrow" />
      </Button>
    </NavLink>
  </Container>
);

HomepageHeading.propTypes = {
  mobile: PropTypes.bool
};

class DesktopContainer extends Component {
  state = {};

  hideFixedNavBar = () => this.setState({ fixed: false });
  showFixedNavBar = () => this.setState({ fixed: true });

  handleItemClick = (e, name) => this.setState({ activeItem: name });

  render() {
    const { children } = this.props;
    const { fixed } = this.state;

    return (
      <Media greaterThan="mobile">
        <Visibility
          once={false}
          onBottomPassed={this.showFixedNavBar}
          onBottomPassedReverse={this.hideFixedNavBar}
        >
          <Segment
            inverted
            textAlign="center"
            style={{
              minHeight: 700,
              padding: "1em 0em",
              backgroundImage:
                "url(https://i.pinimg.com/originals/6e/58/1f/6e581fb70982e576c488457628c72604.jpg)",
              backgroundSize: "cover"
            }}
            vertical
          >
            <Menu
              fixed={fixed ? "top" : null}
              inverted={!fixed}

              secondary={!fixed}
              size="large"
              style={{ color: "orange" }}
            >
              <Container>
                <Menu.Item position="left">
                <NavLink to="/">
                  <Menu.Item
                    as="a"
                    active
                    onClick={this.handleItemClick}
                    color="orange"
                  >
                    Home
                  </Menu.Item>
                </NavLink>
                <NavLink to="/menu">
                  <Menu.Item as="a">Menu</Menu.Item>
                </NavLink>
                </Menu.Item>
                <Menu.Item position="right">
                  <NavLink to="/order">
                    <Button
                      as="a"
                      inverted={!fixed}
                      style={{ marginLeft: "0.5em" }}
                      color="orange"
                    >
                      Order
                    </Button>
                  </NavLink>
                </Menu.Item>
              </Container>
            </Menu>
            <Divider />
            <HomepageHeading />
          </Segment>
        </Visibility>
        {children}
      </Media>
    );
  }
}

DesktopContainer.propTypes = {
  children: PropTypes.node
};

class MobileContainer extends Component {
  state = {};

  handleSidebarHide = () => this.setState({ sidebarOpened: false });

  handleToggle = () => this.setState({ sidebarOpened: true });

  render() {
    const { children } = this.props;
    const { sidebarOpened } = this.state;
    return (
      <Media as={Sidebar.Pushable} at="mobile">
        <Sidebar.Pushable>
          <Sidebar
            as={Menu}
            animation="overlay"
            inverted
            onHide={this.handleSidebarHide}
            vertical
            visible={sidebarOpened}
          >
            <NavLink to="/">
              <Menu.Item as="a" active>
                Home
              </Menu.Item>
            </NavLink>
            <NavLink to="/menu">
              <Menu.Item as="a">Menu</Menu.Item>
            </NavLink>
            <NavLink to="/reservations">
              <Menu.Item as="a">Reservations</Menu.Item>
            </NavLink>
          </Sidebar>
          <Sidebar.Pusher dimmed={sidebarOpened}>
            <Segment
              inverted
              textAlign="center"
              style={{ minHeight: 350, padding: "1em 0em" }}
              vertical
            >
              <Container>
                <Menu inverted pointing secondary size="large">
                  <Menu.Item onClick={this.handleToggle}>
                    <Icon name="sidebar" color="orange" />
                  </Menu.Item>
                  <Menu.Item position="right">
                    <NavLink to="/order">
                      <Button
                        as="a"
                        inverted
                        style={{ marginLeft: "0.5em" }}
                        color="orange"
                      >
                        Order
                      </Button>
                    </NavLink>
                  </Menu.Item>
                </Menu>
              </Container>
              <HomepageHeading mobile />
            </Segment>
            {children}
          </Sidebar.Pusher>
        </Sidebar.Pushable>
      </Media>
    );
  }
}

MobileContainer.propTypes = {
  children: PropTypes.node
};

const ResponsiveContainer = ({ children }) => (
  <MediaContextProvider>
    <DesktopContainer>{children}</DesktopContainer>
    <MobileContainer>{children}</MobileContainer>
  </MediaContextProvider>
);

ResponsiveContainer.propTypes = {
  children: PropTypes.node
};

const Home = () => {
  return (
    <ResponsiveContainer>
      <Segment style={{ padding: "8em 0em" }} vertical>
        <Grid stackable container divided verticalAlign="middle" columns={2}>
          <Grid.Row>
            <Grid.Column>
              <Header as="h3" style={{ fontSize: "2em" }}>
                For Home
              </Header>
              <p style={{ fontSize: "1.33em" }}>
                Take advantage of our quick and simple delivery service to get
                your food to your home fast.
              </p>
              <NavLink to="/menu">
                <Button color="orange">Order Delivery</Button>
              </NavLink>
            </Grid.Column>
            <Grid.Column>
              <Header as="h3" style={{ fontSize: "2em" }}>
                For ToGo
              </Header>
              <p style={{ fontSize: "1.33em" }}>
                Swing on by and pick up your order for pickup when it's fresh
                and ready.
              </p>
              <NavLink to="/menu">
                <Button color="orange">Order ToGo</Button>
              </NavLink>
            </Grid.Column>
          </Grid.Row>
        </Grid>
      </Segment>
      <Segment style={{ padding: "8em 0em" }} vertical>
        <Grid stackable container verticalAlign="middle">
          <Grid.Row>
            <Grid.Column width={8}>
              <Header as="h3" style={{ fontSize: "2em" }}>
                Want the full Experience?
              </Header>
              <p style={{ fontSize: "1.33em" }}>
                Enjoy the complete experience by dining at our restaurant.
              </p>
            </Grid.Column>
            <Grid.Column floated="right" width={6}>
              <Embed
                active
                url="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d601782.5519134006!2d-84.65604910941244!3d33.56869372643647!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x88f5a863fd47ac69%3A0x37784139bd867136!2sFood%20to%20Go!5e0!3m2!1sen!2sus!4v1637108847412!5m2!1sen!2sus"
              />
            </Grid.Column>
          </Grid.Row>
        </Grid>
      </Segment>
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
    </ResponsiveContainer>
  );
};

export default Home;
