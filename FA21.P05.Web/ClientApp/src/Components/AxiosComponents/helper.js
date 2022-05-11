// addToCart adds the necessary info of an item to the cart in localStorage.
// add can only be two values, 1 or -1. if -1, the quantity will decrement.
const addItemToCartUsingArray = (id, itemName, price, add) => {
  let item = {
    id: id,
    name: itemName,
    price: price,
    quantity: 1
  };

  // get cart from local storage
  let cart = [];

  let found = false;
  // see if the cart key is in localstorage
  if (JSON.parse(localStorage.getItem("cart") === null)) {
    // cart was not found in localStorage

    // just add product to the cart array
    cart.push(item);
  } else {
    cart = JSON.parse(localStorage.getItem("cart"));
    // first, find item in array
    if (cart === null) {
      cart = [];
    }
    for (let i = 0; i < cart.length; i++) {
      // if item is in array

      if (cart[i].id === item.id) {
        // found item. update its quantity
        if (add > 0) {
          let prevQuantity = cart[i].quantity;
          item.quantity = prevQuantity + 1;
          // store the item in the items array
          cart[i] = item;
          // done searching
          found = true;
          break;
        }
        //add  === 0 is an identifier to remove the item
        else if (add === 0) {
          var newarray = cart.filter(x => x.name !== item.name);
          newarray = newarray.splice(0, newarray.length);

          if (newarray.length > 0) {
            cart = newarray;
            break;
          } else {
            cart = null;
            break;
          }
        } else {
          let prevQuantity = cart[i].quantity;
          item.quantity = prevQuantity - 1;
          // store the item in the items array

          cart[i] = item;
          // done searching
          found = true;
          break;
        }
      }
    }
    if (!found && add !== 0) {
      cart.push(item);
    }
  }
  // if item was not found in cart, it's a new item
  // stringify items before adding it back to localStorage
  localStorage.setItem("cart", JSON.stringify(cart));
};

export default addItemToCartUsingArray;
