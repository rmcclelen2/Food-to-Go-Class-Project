import React from "react";
import { StyleSheet, ScrollView, Text, View } from "react-native";
import SpecialList from "../Components/SpecialList.js";
import DrinkList from "../Components/DrinkList";
import EntreeList from "../Components/EntreeList.js";
import SideList from "../Components/SideList.js";
export default function Menu() {
  return (
    <ScrollView style={styles.scrollView}>
      <SpecialList />
      <EntreeList />
      <SideList />
      <DrinkList />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#2C2929",
    alignItems: "center",
    justifyContent: "center",
    color: "white",
  },
  text: {
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    color: "white",
  },
  titles: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "white",
  },
  cardTitles: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 36,
    color: "white",
  },
  menuName: {
    paddingBottom: 10,
    backgroundColor: "#ffffff",
    alignItems: "center",
    justifyContent: "center",
    fontSize: 24,
    color: "white",
  },
  image: {
    borderColor: "black",
  },
  scrollView: {
    flex: 1,
    backgroundColor: "#2C2929",
  },
});
