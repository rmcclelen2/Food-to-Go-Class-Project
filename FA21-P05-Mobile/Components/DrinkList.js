import React, { useEffect, useState } from "react";
import { StyleSheet, Text, View, Button, Image, Alert } from "react-native";
import axios from "axios";
import { Card } from "react-native-elements";
import addToOrder from "./AddToOrder.js";
import BASEURL from "../config.js";

export default function DrinkList() {
  const [menuItems, setMenuItems] = useState([]);
  const [buttonDisabled, setButtonDisabled] = useState(false);

  function buttonHandle(item) {
    try {
      addToOrder(item.id, item.name, item.price, 1);
      setButtonDisabled(true);
      setTimeout(() => setButtonDisabled(false), 1500);
      Alert.alert("Added " + item.name + " To Order");
    } catch (e) {
      Alert.alert(e);
    }
  }
  useEffect(() => {
    axios
      .get(BASEURL + "api/menu-items/drinks")
      .then((res) => {
        const menuItems = res.data;
        setMenuItems(res.data);
      })
      .catch(function (error) {
        // handle error
        alert(error.message);
      });
  }, []);
  return (
    <Card>
      <Card.Title style={styles.cardTitles}>Drinks</Card.Title>
      <Card.Divider />
      {menuItems.map((card) => {
        return (
          <View key={card.id} style={styles.container}>
            <Image
              style={{
                borderColor: "orange",
                borderWidth: 5,
                resizeMode: "cover",
                height: 200,
                width: 300,
                padding: 20,
                borderRadius: 10,
              }}
              source={{ uri: card.image }}
            />
            <Text style={styles.menuName}>{card.name}</Text>
            <Text style={styles.text}>{card.description}</Text>
            <Text style={styles.menuName}>${card.price}</Text>
            <Button
              loading={buttonDisabled}
              disabled={buttonDisabled}
              onPress={() => buttonHandle(card)}
              title="Add To Order"
              color="orange"
              accessibilityLabel="Add to Order"
            />
            <Card.Divider />
            <Text style={styles.line}></Text>
            <Card.Divider />
          </View>
        );
      })}
    </Card>
  );
}
const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    color: "black",
  },
  text: {
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    color: "black",
  },
  titles: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "black",
  },
  cardTitles: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "black",
  },
  menuName: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 24,
    color: "black",
  },
  image: {
    borderColor: "orange",
  },
  line: {
    borderWidth: 1,
    borderColor: "orange",
    width: 300,
    height: 0,
  },
});
