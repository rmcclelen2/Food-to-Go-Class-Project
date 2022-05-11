import React from "react";
import { ImageBackground, StyleSheet, Text, View } from "react-native";

export default function Home() {
  const image = {
    uri: "https://static.toiimg.com/thumb/msid-75347702,width-900,height-1200,resizemode-4.cms",
  };
  return (
    <View style={styles.container}>
      <ImageBackground source={image} resizeMode="cover" style={styles.image}>
        <Text style={styles.titles}> FoodToGo </Text>
        <Text style={styles.text}>Great food at the touch of a button!</Text>
      </ImageBackground>
    </View>
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
    alignItems: "center",
    justifyContent: "center",
    color: "white",
  },
  titles: {
    paddingBottom: 10,
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
    flex: 1,
    justifyContent: "center",
    width: "100%",
    height: "100%",
    alignItems: "center",
  },
});
