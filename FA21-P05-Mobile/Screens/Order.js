import AsyncStorage from "@react-native-async-storage/async-storage";
import React, { useEffect, useState } from "react";
import { Card } from "react-native-elements";
import {
  Alert,
  StyleSheet,
  Text,
  Button,
  View,
  ScrollView,
} from "react-native";
import addToOrder from "../Components/addToOrder.js";

export default function Order({ navigation: { navigate } }) {
  const [order, setOrder] = useState([]);
  function quantityEdit(item, edit) {
    if (item.quantity + edit <= 0 || edit === -999) {
      addToOrder(item.id, item.name, item.price, 0);
      getData;
    } else {
      addToOrder(item.id, item.name, item.price, edit);
      getData;
    }
  }

  const getData = async () => {
    try {
      const jsonvalue = await AsyncStorage.getItem("order");
      setOrder(JSON.parse(jsonvalue));
    } catch (e) {}
  };

  const clearOrder = async () => {
    try {
      await AsyncStorage.clear().then(() => {
        setOrder([]);
      });
    } catch (e) {}
  };

  if (order == null) {
    useEffect(() => {
      getData();
    });
    return (
      <ScrollView style={styles.scrollView}>
        <Card>
          <Card.Title style={styles.cardTitles}>Order</Card.Title>
          <Card.Divider />

          <View style={styles.container}>
            <Text style={styles.menuName}>Order Is Empty</Text>
            <Card.Divider />
          </View>
        </Card>
        <Button
          style={styles.button}
          onPress={clearOrder}
          title="Clear Order"
          color="orange"
          accessibilityLabel="Clear Order"
        />
        <Text> </Text>
        <Text style={styles.warn}> Order Not Displaying? Tap Below!</Text>
        <Button
          style={styles.button}
          onPress={getData}
          title="Refresh Order"
          color="orange"
          accessibilityLabel="Refresh Order"
        />
      </ScrollView>
    );
  } else {
    useEffect(() => {
      getData();
    });
    var total = 0;
    order.map((card) => {
      total = total + card.price * card.quantity;
    });
    total = total.toFixed(2);
    return (
      <ScrollView style={styles.scrollView}>
        <Card>
          <Card.Title style={styles.cardTitles}>Order</Card.Title>
          <Card.Divider />
          {order.map((card) => {
            return (
              <View key={card.id} style={styles.container}>
                <Text style={styles.menuName}>{card.name}</Text>
                <Text style={styles.text}>Price: {card.price}</Text>
                <Text style={styles.text}>
                  Item Total: ${(card.price * card.quantity).toFixed(2)}
                </Text>
                <View>
                  <Button
                    style={styles.iconButton}
                    onPress={() => quantityEdit(card, -1)}
                    title="-"
                    color="orange"
                    accessibilityLabel="-"
                  />

                  <Text style={styles.text}> Quantity: {card.quantity} </Text>
                  <Button
                    style={styles.iconButton}
                    onPress={() => quantityEdit(card, 1)}
                    title="+"
                    color="orange"
                    accessibilityLabel="+"
                  />
                  <Text></Text>
                  <Button
                    style={styles.iconButton}
                    onPress={() => quantityEdit(card, -999)}
                    title="Remove Item"
                    color="orange"
                    accessibilityLabel="Remove Item"
                  />
                </View>
                <Card.Divider />
                <Text style={styles.line}></Text>
                <Card.Divider />
              </View>
            );
          })}
          <Text style={styles.menuName}> Order Total: ${total} </Text>
        </Card>

        <Text> </Text>
        <Button
          style={styles.button}
          onPress={clearOrder}
          title="Clear Order"
          color="orange"
          accessibilityLabel="Clear Order"
        />
        <Text> </Text>
        <Button
          style={styles.button}
          // onPress={() => Alert.alert("OOPS", "Feature in Development")}
          onPress={() => navigate("Confirm Order")}
          Icon=""
          title="Place Order"
          color="orange"
          accessibilityLabel="Clear Order"
          width="20"
        />
      </ScrollView>
    );
  }
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
    textAlign: "center",
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
    textAlign: "center",
  },
  menuName: {
    paddingBottom: 10,
    alignItems: "center",
    justifyContent: "center",
    fontSize: 24,
    color: "black",
    textAlign: "center",
  },
  warn: {
    paddingBottom: 10,
    alignItems: "center",
    justifyContent: "center",
    fontSize: 24,
    color: "orange",
    textAlign: "center",
  },
  image: {
    borderColor: "black",
  },
  scrollView: {
    flex: 1,
    backgroundColor: "#2C2929",
  },
  button: {
    alignItems: "center",
    justifyContent: "center",
    alignSelf: "center",
    flexDirection: "row",
    color: "orange",
    width: 20,
  },
  iconButton: {
    alignItems: "center",
    justifyContent: "center",
    alignSelf: "center",
    color: "orange",
    height: 3,
  },
  line: {
    borderWidth: 1,
    borderColor: "orange",
    width: 300,
    height: 0,
  },
});
