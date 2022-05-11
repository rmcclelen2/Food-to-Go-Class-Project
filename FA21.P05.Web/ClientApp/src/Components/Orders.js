import React, { useState, useEffect } from "react";
import StaffNavbar from "./StaffNavbar";
import axios from "axios";
import { Card, Icon, Container, Menu } from "semantic-ui-react";

const Orders = () => {
  const [orderData, setOrderData] = useState([]);
  const [itemName, setItemName] = useState([]);
  const [activeItem, setActiveItem] = useState("inProgress");

  useEffect(() => {
    axios.get("api/orders").then(response => {
      setOrderData(response.data);
    });
  }, []);
  useEffect(() => {
    axios.get("api/menu-items").then(response => {
      setItemName(response.data);
    });
  }, []);

  var inProgressOrders = orderData.filter(
    x => x.placed !== null && x.canceled === null && x.finished === null
  );
  var completedOrders = orderData.filter(
    x => x.finished !== null && x.customerRecieved === null
  );
  var receivedOrders = orderData.filter(
    x => x.canceled !== null || x.customerRecieved !== null
  );
  function deleteOrder(id) {
    axios
      .delete(`api/orders/${id}/delete`)
      .then(res => {
        axios.get("api/orders").then(response => {
          setOrderData(response.data);
        });
      })
      .catch(err => {
        alert(err.response.data);
      });
  }
  function cancelOrder(id) {
    axios
      .put(`api/orders/${id}/cancel`)
      .then(res => {
        alert("order canceled");
        axios.get("api/orders").then(response => {
          setOrderData(response.data);
        });
      })
      .catch(err => {
        alert(err.response.data);
      });
  }
  function finishOrder(id) {
    axios
      .put(`api/orders/${id}/finish`)
      .then(res => {
        alert("order finished");
        axios.get("api/orders").then(response => {
          setOrderData(response.data);
        });
      })
      .catch(err => {
        alert(err.response.data);
      });
  }
  function receiveOrder(id) {
    axios
      .put(`api/orders/${id}/received`)
      .then(res => {
        alert("order received");
        axios.get("api/orders").then(response => {
          setOrderData(response.data);
        });
      })
      .catch(err => {
        alert(err.response.data);
      });
  }
  function startOrder(id) {
    axios
      .put(`api/orders/${id}/start`)
      .then(res => {
        alert("order started");
        axios.get("api/orders").then(response => {
          setOrderData(response.data);
        });
      })
      .catch(err => {
        alert(err.response.data);
      });
  }
  return (
    <>
      <StaffNavbar />
      <div>
        <Menu tabular>
          <Menu.Item
            name="inProgress"
            active={activeItem === "inProgress"}
            onClick={() => setActiveItem("inProgress")}
          />
          <Menu.Item
            name="completed"
            active={activeItem === "completed"}
            onClick={() => setActiveItem("completed")}
          />
          <Menu.Item
            name="customerReceived"
            active={activeItem === "customerReceived"}
            onClick={() => setActiveItem("customerReceived")}
          />
        </Menu>
        {activeItem === "inProgress" ? (
          <Container>
            <Card.Group itemsPerRow={3}>
              {inProgressOrders.map(s => (
                <Card key={s.id}>
                  <Card.Content>
                    <Card.Header>Order#: {s.id}</Card.Header>
                    <Card.Description>
                      {s?.orderItems
                        ? s.orderItems.map(item => (
                            <li key={item.id}>
                              Item:{" "}
                              {itemName[item.menuItemId - 1]?.name
                                ? itemName[item.menuItemId - 1].name
                                : null}
                              <br />
                              Quantity: {item.lineItemQuantity}
                              <br />
                            </li>
                          ))
                        : null}

                      {s.address ? <p>Address : {s.address}</p> : null}
                      {s.customerFirst ? (
                        <p>
                          Customer Name:{" "}
                          {s.customerFirst + " " + s.customerLast}
                        </p>
                      ) : null}
                    </Card.Description>
                  </Card.Content>

                  <Card.Content extra>
                    <Icon name="dollar" />
                    {s.orderTotal.toFixed(2)} <br />
                    Placed: {new Date(s.placed).toLocaleString()}
                    <br />
                    {s?.started
                      ? "Started:" + new Date(s.started).toLocaleString()
                      : null}
                    {s?.started || s?.canceled ? null : (
                      <div
                        className="ui bottom attached button"
                        onClick={() => startOrder(s.id)}
                      >
                        <i className="add icon" />
                        Start
                      </div>
                    )}
                    {!s?.started || s?.canceled ? null : (
                      <div
                        className="ui bottom attached button"
                        onClick={() => finishOrder(s.id)}
                      >
                        <i className="check icon" />
                        Finished
                      </div>
                    )}
                    {s?.canceled || s?.started ? null : (
                      <div
                        className="ui bottom attached button"
                        onClick={() => cancelOrder(s.id)}
                      >
                        <i className="ban icon" />
                        Cancel
                      </div>
                    )}
                  </Card.Content>
                </Card>
              ))}
            </Card.Group>
          </Container>
        ) : null}
        {activeItem === "completed" ? (
          <Container>
            <Card.Group itemsPerRow={3}>
              {completedOrders.map(s => (
                <Card key={s.id}>
                  <Card.Content>
                    <Card.Header>Order#: {s.id}</Card.Header>
                    <Card.Description>
                      {s?.orderItems
                        ? s.orderItems.map(item => (
                            <li key={item.id}>
                              Item:{" "}
                              {itemName[item.menuItemId - 1]?.name
                                ? itemName[item.menuItemId - 1].name
                                : null}
                              <br />
                              Quantity: {item.lineItemQuantity}
                              <br />
                            </li>
                          ))
                        : null}
                      {s.address ? <p>Address : {s.address}</p> : null}
                      {s.customerFirst ? (
                        <p>
                          Customer Name:{" "}
                          {s.customerFirst + " " + s.customerLast}
                        </p>
                      ) : null}
                    </Card.Description>
                  </Card.Content>

                  <Card.Content extra>
                    <Icon name="dollar" />
                    {s.orderTotal.toFixed(2)} <br />
                    Placed: {new Date(s.placed).toLocaleString()}
                    <br />
                    {s?.started
                      ? "Started:" + new Date(s.started).toLocaleString()
                      : null}
                    {s?.canceled
                      ? "Canceled:" + new Date(s.canceled).toLocaleString()
                      : null}
                    <br />
                    {s?.finished
                      ? "Finished:" + new Date(s.finished).toLocaleString()
                      : null}
                    <br />
                    {s?.started || s?.canceled ? null : (
                      <div
                        className="ui bottom attached button"
                        onClick={() => startOrder(s.id)}
                      >
                        <i className="add icon" />
                        Start
                      </div>
                    )}
                    {s?.canceled ? null : (
                      <div
                        className="ui bottom attached button"
                        onClick={() => receiveOrder(s.id)}
                      >
                        <i className="shopping bag icon" />
                        Received
                      </div>
                    )}
                  </Card.Content>
                </Card>
              ))}
            </Card.Group>
          </Container>
        ) : null}
        {activeItem === "customerReceived" ? (
          <Container>
            <Card.Group itemsPerRow={3}>
              {receivedOrders.map(s => (
                <Card key={s.id}>
                  <Card.Content>
                    <Card.Header>Order#: {s.id}</Card.Header>
                    <Card.Description>
                      {s?.orderItems
                        ? s.orderItems.map(item => (
                            <li key={item.id}>
                              Item:{" "}
                              {itemName[item.menuItemId - 1]?.name
                                ? itemName[item.menuItemId - 1].name
                                : null}
                              <br />
                              Quantity: {item.lineItemQuantity}
                              <br />
                            </li>
                          ))
                        : null}
                      {s.address ? <p>Address : {s.address}</p> : null}
                      {s.customerFirst ? (
                        <p>
                          Customer Name:{" "}
                          {s.customerFirst + " " + s.customerLast}
                        </p>
                      ) : null}
                    </Card.Description>
                  </Card.Content>

                  <Card.Content extra>
                    <Icon name="dollar" />
                    {s.orderTotal.toFixed(2)} <br />
                    Placed: {new Date(s.placed).toLocaleString()}
                    <br />
                    {s?.started
                      ? "Started:" + new Date(s.started).toLocaleString()
                      : null}
                    {s?.canceled
                      ? "Canceled:" + new Date(s.canceled).toLocaleString()
                      : null}
                    <br />
                    {s?.finished
                      ? "Finished:" + new Date(s.finished).toLocaleString()
                      : null}
                    <br />
                    {s?.customerRecieved
                      ? "Customer Recieved:" +
                        new Date(s.customerRecieved).toLocaleString()
                      : null}
                    <div
                      className="ui bottom attached button"
                      onClick={() => deleteOrder(s.id)}
                    >
                      <i className="trash icon" />
                      Remove
                    </div>
                  </Card.Content>
                </Card>
              ))}
            </Card.Group>
          </Container>
        ) : null}
      </div>
    </>
  );
};

export default Orders;
