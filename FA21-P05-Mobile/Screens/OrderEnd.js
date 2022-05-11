import React from "react";
import { View, Text, StyleSheet, Button } from "react-native";

export default function OrderEnd({ navigation: { popToTop } }) {
  try {
    return (
      <View style={styles.container}>
        <Text style={styles.menuName}>Your Order Has Been Submitted</Text>
        <Text> </Text>
        <Text style={styles.text}>Pay When You Recieve Your Order</Text>
        <Text> </Text>

        <Button
          style={styles.button}
          onPress={() => popToTop()}
          Icon=""
          title="Return"
          color="orange"
          accessibilityLabel="Return"
        />
      </View>
    );
  } catch (e) {
    return (
      <View style={styles.container}>
        <Text style={styles.menuName}>Order Failed To Submit</Text>
        <Text>{e}</Text>
      </View>
    );
  }
}
const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#2C2929",
    alignItems: "center",
    justifyContent: "center",
  },
  text: {
    alignItems: "center",
    justifyContent: "center",
    color: "orange",
    textAlign: "center",
  },
  titles: {
    paddingBottom: 10,
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "black",
  },
  cardTitles: {
    paddingBottom: 10,
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
    color: "orange",
    textAlign: "center",
    fontWeight: "bold",
  },
  image: {
    borderColor: "black",
  },
  scrollView: {
    flex: 1,
    backgroundColor: "#ffffff",
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
