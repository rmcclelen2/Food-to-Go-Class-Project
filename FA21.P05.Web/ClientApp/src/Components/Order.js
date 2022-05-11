import React, { useState, useEffect } from "react";
import Navbar from "./Navbar";
import { Icon, Button, Table, Header, Form } from "semantic-ui-react";
import axios from "axios";
import addItemToCartUsingArray from "../Components/AxiosComponents/helper";


const Cart = () => {
  const [itemData, setItemData] = useState([]);
  const [isVisible, setIsVisible] = useState(true);
  const [isDelivery, setIsDelivery] = useState(false);
  const [formData, setFormData] = useState({
    orderItems: [],
    customerFirst: "",
    customerLast: "",
    address: ""
  });

  useEffect(() => {
    setItemData(JSON.parse(localStorage.getItem("cart")));
  }, []);
  function showCheckout() {
    setIsVisible(!isVisible);
  }
  function clearCart() {
    localStorage.clear("cart");
    setItemData(null);
  }
  const sendOrder = e => {
    e.preventDefault();
    const testOrder = formData;
    for (var i = 0; i < itemData.length; ++i) {
      testOrder.orderItems.push({
        lineItemQuantity: itemData[i].quantity,
        menuItemId: itemData[i].id
      });
      clearCart();
    }

    axios
      .post(`api/orders/`, testOrder)
      .then(res => {
        alert("Order Placed!");
      })
      .catch(err => {
        alert(err.response.data);
      });
  };
  let orderTotal = 0;
  if (itemData) {
    orderTotal = itemData.reduce(function(acc, curr) {
      return acc + curr.quantity * curr.price;
    }, 0);
  }

  function changeQty(id, name, price, qty) {
    let index = itemData.findIndex(x => x.id === id);

    //check if quantity about to go negative, if so delete item
    if (itemData[index].quantity + qty <= 0 || qty === -999) {
      addItemToCartUsingArray(id, name, price, 0);
      setItemData(JSON.parse(localStorage.getItem("cart")));
    } else {
      addItemToCartUsingArray(id, name, price, qty);
      setItemData(JSON.parse(localStorage.getItem("cart")));
    }
  }

  return (
    <>
      <div
        style={{
          backgroundColor: "#282c34",
          color: "orange"
        }}
      >
        <Navbar />

        {itemData ? (
          <div>
            {isVisible ? (
              <div>
                <Table
                  vertical
                  floated="left"
                  celled
                  inverted
                  style={{
                    backgroundColor: "#282c34",
                    border: "2px solid orange",
                    color: "orange",
                    width: "50%"
                  }}
                >
                  <Table.Header>
                    <Table.Row>
                      <Table.HeaderCell singleLine textAlign="center">
                        <p style={{ color: "orange" }}>Item</p>
                      </Table.HeaderCell>
                      <Table.HeaderCell textAlign="center">
                        {" "}
                        <p style={{ color: "orange" }}>Quantity</p>
                      </Table.HeaderCell>
                      <Table.HeaderCell textAlign="center">
                        <p style={{ color: "orange" }}>Remove</p>
                      </Table.HeaderCell>
                      <Table.HeaderCell textAlign="center">
                        <p style={{ color: "orange" }}>Price</p>
                      </Table.HeaderCell>
                    </Table.Row>
                  </Table.Header>

                  <Table.Body>
                    {itemData.map(item => (
                      <Table.Row key={item.id}>
                        <Table.Cell>
                          <Header
                            style={{ color: "orange" }}
                            as="h4"
                            textAlign="center"
                          >
                            {item.name}
                          </Header>
                        </Table.Cell>
                        <Table.Cell singleLine textAlign="center">
                          <Button
                            icon="minus"
                            size="tiny"
                            style={{
                              color: "orange",
                              backgroundColor: "#282c34"
                            }}
                            onClick={() =>
                              changeQty(item.id, item.name, item.price, -1)
                            }
                          />
                          {item.quantity}{" "}
                          <Button
                            icon="plus"
                            size="tiny"
                            style={{
                              color: "orange",
                              backgroundColor: "#282c34"
                            }}
                            onClick={() =>
                              changeQty(item.id, item.name, item.price, 1)
                            }
                          />
                        </Table.Cell>
                        <Table.Cell textAlign="center">
                          <Button
                            icon
                            style={{
                              color: "orange",
                              backgroundColor: "#282c34"
                            }}
                            onClick={() =>
                              changeQty(item.id, item.name, item.price, -999)
                            }
                          >
                            <Icon className="trash" />
                          </Button>
                        </Table.Cell>
                        <Table.Cell textAlign="center">
                          ${(item.quantity * item.price).toFixed(2)}
                        </Table.Cell>
                      </Table.Row>
                    ))}
                  </Table.Body>

                  <Table.Footer
                    fullWidth
                    style={{ outline: "2px solid orange" }}
                  >
                    <Table.Row>
                      <Table.HeaderCell colSpan="3">
                        <Button
                          icon
                          labelPosition="left"
                          fluid
                          color="green"
                          onClick={() => showCheckout()}
                        >
                          <Icon className="long arrow alternate right" />Check
                          Out
                        </Button>
                      </Table.HeaderCell>
                      <Table.HeaderCell colSpan="1" textAlign="center">
                        <p style={{ color: "orange" }}>
                          TOTAL: ${orderTotal.toFixed(2)}
                        </p>
                      </Table.HeaderCell>
                    </Table.Row>
                    <Table.Row>
                      <Table.HeaderCell colSpan="3" textAlign="center" />
                      <Table.HeaderCell colSpan="1" textAlign="center">
                        <Button
                          icon
                          labelPosition="left"
                          negative
                          size="large"
                          onClick={() => clearCart()}
                        >
                          <Icon name="trash alternate" />
                          Clear order
                        </Button>
                      </Table.HeaderCell>
                    </Table.Row>
                  </Table.Footer>
                </Table>
              </div>
            ) : (
              <div style={{ textAlign: "left" }}>
                <Form
                  size="small"
                  inverted
                  style={{
                    backgroundColor: "#282c34",
                    border: "2px solid orange",
                    color: "orange",
                    width: "50%"
                  }}
                  onSubmit={sendOrder}
                >
                  <div style={{ padding: "10px" }}>
                    <Form.Field>
                      <label style={{ color: "orange" }}>First Name</label>
                      <Form.Input
                        value={formData.customerFirst}
                        onChange={e =>
                          setFormData({
                            ...formData,
                            customerFirst: e.target.value
                          })
                        }
                        required
                        placeholder="First Name"
                      />
                    </Form.Field>
                    <Form.Field>
                      <label style={{ color: "orange" }}>Last Name</label>
                      <Form.Input
                        value={formData.customerLast}
                        onChange={e =>
                          setFormData({
                            ...formData,
                            customerLast: e.target.value
                          })
                        }
                        required
                        placeholder="Last Name"
                      />
                      <p>
                        Want to make a change to your order?<Button
                          onClick={() => showCheckout(false)}
                          style={{
                            color: "orange",
                            cursor: "pointer",
                            background: "none",
                            border: "none",
                            textDecoration: "underline",
                            padding: "2px"
                          }}
                        >
                          Go back
                        </Button>
                      </p>
                    </Form.Field>
                    <Form.Field>
                      <Form.Radio
                        onClick={() => setIsDelivery(!isDelivery)}
                        toggle
                        label="Delivery"
                        color="orange"
                      />
                      {isDelivery ? (
                        <Form.Field>
                          <label style={{ color: "orange" }}>
                            Delivery Address
                          </label>
                          <Form.Input
                            value={formData.address}
                            onChange={e =>
                              setFormData({
                                ...formData,
                                address: e.target.value
                              })
                            }
                            required
                            placeholder="Address"
                          />
                        </Form.Field>
                      ) : null}
                    </Form.Field>
                    <Button
                      icon
                      type="submit"
                      labelPosition="left"
                      fluid
                      primary
                    >
                      <Icon name="money" /> Place Order
                    </Button>
                  </div>
                </Form>
              </div>
            )}
          </div>
        ) : (
          <p>
            Your order is empty, try adding one of our delicious food items!
          </p>
        )}
      </div>
      <>
        <div
          style={{
            backgroundColor: "#282c34",
            color: "orange",
            height: "100vh",
            minHeight: "100vh",
            width: "100%"
          }}
        >
          <Button.Group vertical floated="left" />
        </div>
      </>
    </>
  );
};

export default Cart;
