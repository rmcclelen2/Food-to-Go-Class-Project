import React from "react";
import { StyleSheet, StatusBar } from "react-native";
import Menu from "./Screens/Menu.js";
import Home from "./Screens/Home.js";
import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import Ionicons from "react-native-vector-icons/Ionicons";
import Order from "./Screens/Order.js";
import OrderPlace from "./Screens/OrderPlace.js";
import Checkout from "./Screens/Checkout.js";
import OrderEnd from "./Screens/OrderEnd.js";

const Tab = createBottomTabNavigator();

export default function App() {
  return (
    <>
      <StatusBar
        barStyle="light-content"
        backgroundColor="#2C2929"
        translucent={true}
      />
      <NavigationContainer>
        <Tab.Navigator
          style={styles.container}
          initialRouteName="Home"
          screenOptions={({ route }) => ({
            tabBarIcon: ({ focused, color, size }) => {
              let iconName;
              if (route.name === "Home") {
                iconName = focused ? "home" : "home-outline";
              } else if (route.name === "Menu") {
                iconName = focused ? "fast-food" : "fast-food-outline";
              } else if (route.name === "Order") {
                iconName = focused ? "clipboard" : "clipboard-outline";
              }
              return <Ionicons name={iconName} size={size} color={color} />;
            },
            tabBarActiveTintColor: "orange",
            tabBarInactiveTintColor: "white",
            tabBarActiveBackgroundColor: "#2C2929",
            tabBarInactiveBackgroundColor: "#2C2929",
          })}
        >
          <Tab.Screen
            name="Home"
            component={Home}
            options={{
              headerStyle: {
                backgroundColor: "#2C2929",
              },
              headerTitleStyle: {
                color: "white",
              },
              headerTitleAlign: "center",
            }}
          />
          <Tab.Screen
            name="Menu"
            component={Menu}
            options={{
              headerStyle: {
                backgroundColor: "#2C2929",
              },
              headerTitleStyle: {
                color: "white",
              },
              headerTitleAlign: "center",
            }}
          />
          <Tab.Screen
            name="Order"
            component={OrderNav}
            options={{
              headerShown: false,
              headerStyle: {
                backgroundColor: "#2C2929",
              },
              headerTitleStyle: {
                color: "white",
              },
              headerTitleAlign: "center",
            }}
          />
        </Tab.Navigator>
      </NavigationContainer>
    </>
  );
}
function OrderNav() {
  const Stack = createNativeStackNavigator();

  return (
    <Stack.Navigator initialRouteName="Your Order">
      <Stack.Screen
        name="Your Order"
        component={Order}
        options={{
          headerStyle: {
            backgroundColor: "#2C2929",
            color: "orange",
          },
          headerTitleStyle: {
            color: "white",
          },
          headerTitleAlign: "center",
        }}
      />
      <Stack.Screen
        name="Confirm Order"
        component={OrderPlace}
        options={{
          headerStyle: {
            backgroundColor: "#2C2929",
          },
          headerTitleStyle: {
            color: "white",
          },
          headerTitleAlign: "center",
        }}
      />
      <Stack.Screen
        name="Checkout"
        component={Checkout}
        options={{
          headerStyle: {
            backgroundColor: "#2C2929",
          },
          headerTitleStyle: {
            color: "white",
          },
          headerTitleAlign: "center",
        }}
      />
      <Stack.Screen
        name="Thank You"
        component={OrderEnd}
        options={{
          headerStyle: {
            backgroundColor: "#2C2929",
          },
          headerTitleStyle: {
            color: "white",
          },
          headerTitleAlign: "center",
        }}
      />
    </Stack.Navigator>
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
});
