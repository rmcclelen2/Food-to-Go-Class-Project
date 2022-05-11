import React, { useEffect, useState } from "react";
import AsyncStorage from "@react-native-async-storage/async-storage";
import {
  KeyboardAvoidingView,
  StyleSheet,
  Text,
  TextInput,
  Button,
} from "react-native";
import axios from "axios";
import BASEURL from "../config";
export default function Checkout({ navigation: { navigate } }) {
  const [order, setOrder] = useState([]);
  const [address, setAddress] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
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
  function orderSend(order, address, firstName, lastName) {
    console.log(order);
    const testOrder = {
      orderItems: [],
      address: "",
      customerFirst: "",
      customerLast: "",
    };
    for (var i = 0; i < order.length; ++i) {
      testOrder.orderItems.push({
        lineItemQuantity: order[i].quantity,
        menuItemId: order[i].id,
      });
    }
    if (address === undefined) {
      address = "Pickup";
    } else {
      testOrder.address = address;
    }
    testOrder.customerFirst = firstName;
    testOrder.customerLast = lastName;
    console.log(testOrder);
    axios
      .post(BASEURL + "api/orders/", testOrder)
      .then((res) => {
        console.log(res);
      })
      .catch((err) => {
        Alert.alert(err.response.data);
      });
    navigate("Thank You");
    clearOrder();
  }

  var total = 0;
  order.map((card) => {
    total = total + card.price * card.quantity;
  });
  total = total.toFixed(2);

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      // behavior={Platform.OS === "android" ? "padding" : "height"}
      style={styles.container}
    >
      <Text style={styles.menuName}>Enter Your Name</Text>
      <TextInput
        style={{
          height: 50,
          padding: 5,
          borderColor: "orange",
          borderWidth: 5,
          borderRadius: 10,
          backgroundColor: "white",
          color: "orange",
        }}
        placeholder="First Name"
        onChangeText={(text) => setFirstName(text)}
        defaultValue={firstName}
      />
      <Text> </Text>
      <TextInput
        style={{
          height: 50,
          padding: 5,
          borderColor: "orange",
          borderWidth: 5,
          borderRadius: 10,
          backgroundColor: "white",
          color: "orange",
        }}
        placeholder="Last Name"
        onChangeText={(text) => setLastName(text)}
        defaultValue={lastName}
      />
      <Text> </Text>
      <Text style={styles.menuName}>Enter Your Delivery Address</Text>
      <Text style={styles.menuName}>OR</Text>
      <Text style={styles.menuName}>Leave Blank For In-Store Pickup</Text>
      <TextInput
        style={{
          height: 50,
          padding: 5,
          borderColor: "orange",
          borderWidth: 5,
          borderRadius: 10,
          backgroundColor: "white",
          color: "orange",
        }}
        placeholder="Delivery Address"
        onChangeText={(text) => setAddress(text)}
        defaultValue={address}
      />
      <Text></Text>
      <Button
        style={styles.button}
        // onPress={() => navigate("Thank You")}
        onPress={() => orderSend(order, address, firstName, lastName)}
        Icon=""
        title="Finish"
        color="orange"
        accessibilityLabel="Finish"
      />
    </KeyboardAvoidingView>
  );
}
const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#2C2929",
    alignItems: "center",
    justifyContent: "center",
    color: "black",
  },
  text: {
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    color: "orange",
  },
  titles: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "orange",
  },
  cardTitles: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "orange",
  },
  menuName: {
    paddingBottom: 10,
    alignItems: "center",
    justifyContent: "center",
    fontSize: 24,
    color: "orange",
  },
  image: {
    borderColor: "black",
  },
  scrollView: {
    marginHorizontal: 20,
    backgroundColor: "#ffffff",
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
