import React, { useEffect, useState } from "react";
import axios from "axios";
import { Card, Icon, Container, Button } from "semantic-ui-react";
import addItemToCartUsingArray from "./helper";

export default function DrinkList() {
  const [itemData, setItemData] = useState([]);
  const [buttonDisabled, setButtonDisabled] = useState(false);

  useEffect(() => {
    axios.get("api/menu-items/drinks").then(response => {
      setItemData(response.data);
    });
  }, []);
  function handleButtonClick(id, name, price) {
    addItemToCartUsingArray(id, name, price, 1);
    setButtonDisabled(true);
    setTimeout(() => setButtonDisabled(false), 3000);
  }
  return (
    <Container>
      <Card.Group itemsPerRow={3}>
        {itemData.map(card => (
          <Card
            style={{ backgroundColor: "#282c34", outline: "2px solid orange" }}
            key={card.price}
          >
            <Card.Content
              style={{
                height: "240px",
                background: "url(" + card.image + ") no-repeat",
                backgroundPosition: "center center",
                backgroundSize: "cover",
                borderColor: "red"
              }}
            />
            <Card.Content>
              <Card.Header style={{ color: "orange" }}>{card.name}</Card.Header>
              <Card.Description
                style={{ color: "orange", borderBottom: "1px solid orange" }}
              >
                {card.description}
              </Card.Description>
            </Card.Content>
            <Card.Content style={{ color: "orange" }} extra>
              <Icon name="dollar" />
              {card.price}
              <Button
                fluid
                style={{ backgroundColor: "orange", color: "#282c34" }}
                loading={buttonDisabled}
                disabled={buttonDisabled}
              >
                <Button.Content
                  onClick={() =>
                    handleButtonClick(card.id, card.name, card.price)
                  }
                >
                  <i className="add icon" />ADD TO ORDER
                </Button.Content>
              </Button>
            </Card.Content>
          </Card>
        ))}
      </Card.Group>
    </Container>
  );
}
