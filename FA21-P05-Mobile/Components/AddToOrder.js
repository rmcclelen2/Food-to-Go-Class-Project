import React, { Component, useEffect, useState } from "react";
import AsyncStorage from "@react-native-async-storage/async-storage";

export default async function addToOrder(itemid, itemName, itemPrice, add) {
  try {
    let itemToAdd = {
      id: itemid,
      name: itemName,
      price: itemPrice,
      quantity: 1,
    };
    var order = [];
    var temp = [];
    order = [];
    const data = await AsyncStorage.getItem("order");
    temp = JSON.parse(data);
    order = temp;
    let found = false;
    if (order === null) {
      order = [];
      order.push(itemToAdd);
    } else {
      for (let i = 0; i < order.length; i++) {
        if (order[i].id === itemToAdd.id) {
          found = true;

          if (add > 0) {
            order[i].quantity = order[i].quantity + 1;
            break;
          } else if (add === 0) {
            var newOrder = order.filter((x) => x.id !== itemid);
            newOrder = newOrder.splice(0, newOrder.length);
            if (newOrder.length > 0) {
              order = newOrder;
              break;
            } else {
              order = [];
            }
          } else if (add < 0) {
            order[i].quantity = order[i].quantity - 1;
          }
        }
      }

      if (!found) {
        order.push(itemToAdd);
      }
    }
    const jsonvalue = JSON.stringify(order);
    await AsyncStorage.setItem("order", jsonvalue);
  } catch (error) {
    alert(error);
  }
}
