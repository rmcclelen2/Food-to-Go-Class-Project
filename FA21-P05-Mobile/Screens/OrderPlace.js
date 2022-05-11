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

export default function OrderPlace({ navigation: { navigate } }) {
  const [order, setOrder] = useState([]);

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

  useEffect(() => {
    getData();
  }, []);
  var total = 0;
  order.map((card) => {
    total = total + card.price * card.quantity;
  });
  total = total.toFixed(2);
  return (
    <ScrollView style={styles.scrollView}>
      <Card>
        <Card.Title style={styles.cardTitles}>
          Please Confirm Your Order
        </Card.Title>
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
                <Text style={styles.text}> Quantity: {card.quantity} </Text>
              </View>
              <Card.Divider />
              <Card.Divider />
            </View>
          );
        })}
        <Text style={styles.menuName}> Order Total: ${total} </Text>
      </Card>

      <Text> </Text>
      <Text> </Text>
      <Button
        style={styles.button}
        //onPress={() => Alert.alert("OOPS", "Feature in Development")}
        onPress={() => navigate("Checkout")}
        Icon=""
        title="Proceed To Checkout"
        color="orange"
        accessibilityLabel="Proceed To Checkout"
      />
    </ScrollView>
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
    borderColor: "black",
  },
  scrollView: {
    flex: 1,
    backgroundColor: "#2C2929",
  },
  button: {
    width: 150,
    alignItems: "center",
    justifyContent: "center",
    alignSelf: "center",
    flexDirection: "row",
    color: "orange",
  },
  iconButton: {
    width: 150,
    alignItems: "center",
    justifyContent: "center",
    alignSelf: "center",
    flexDirection: "row",
    color: "orange",
    fontSize: 30,
  },
});
