import React, { useState, useContext } from "react";
import { Button, Grid, Header, Segment, Form } from "semantic-ui-react";
import axios from "axios";
import Schedule from "./Schedule";
import { UserContext } from "../Components/UserContext";

const Login = () => {
  const { loggedIn, setLoggedIn } = useContext(UserContext);
  const [formData, setFormData] = useState({
    username: "",
    password: ""
  });

  const handleSubmit = e => {
    e.preventDefault();
    axios
      .post("api/authentication/login", formData)
      .then(res => {
        if (res.status === 200) {
          setLoggedIn(formData);
          localStorage.setItem("user", formData);
        }
      })
      .catch(err => console.log(err));
  };

  if (loggedIn == null) {
    return (
      <>
        <Grid
          textAlign="center"
          style={{ height: "100vh", backgroundColor: "#282c34" }}
          verticalAlign="middle"
        >
          <Grid.Column style={{ maxWidth: 450 }}>
            <Header as="h2" color="orange" textAlign="center">
              {" "}
              Employee Login
            </Header>
            <Form
              size="large"
              onSubmit={handleSubmit}
              style={{ border: "solid orange", borderRadius: "7px" }}
            >
              <Segment stacked style={{ backgroundColor: "#282c34" }}>
                <Form.Input
                  fluid
                  icon="user"
                  iconPosition="left"
                  placeholder="Username"
                  value={formData.username}
                  onChange={e =>
                    setFormData({ ...formData, username: e.target.value })
                  }
                  required
                />
                <Form.Input
                  fluid
                  icon="lock"
                  iconPosition="left"
                  placeholder="Password"
                  type="password"
                  value={formData.password}
                  onChange={e =>
                    setFormData({ ...formData, password: e.target.value })
                  }
                  required
                />
                <Button color="orange" fluid size="large" type="submit">
                  <p style={{ color: "#282c34" }}>Login</p>
                </Button>
              </Segment>
            </Form>
          </Grid.Column>
        </Grid>
      </>
    );
  } else if (loggedIn != null) {
    return (
      <>
        <Schedule />
      </>
    );
  }
};
export default Login;
