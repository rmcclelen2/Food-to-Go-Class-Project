using System;
using System.Linq;
using System.Linq.Expressions;
using FA21.P05.Web.Data;
using FA21.P05.Web.Features.Identity;
using FA21.P05.Web.Features.MenuItems;
using FA21.P05.Web.Features.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FA21.P05.Web.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext dataContext;

        public OrdersController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private static Expression<Func<Order, OrderDto>> MapDto()
        {
            return x => new OrderDto
            {
                Id = x.Id,
                OrderTotal = x.OrderTotal,
                Placed = x.Placed,
                Started = x.Started,
                Finished = x.Finished,
                CustomerRecieved = x.CustomerRecieved,
                CustomerFirst = x.CustomerFirst,
                CustomerLast = x.CustomerLast,
                Canceled = x.Canceled,
                Address = x.Address,
                OrderItems = x.OrderItems.Select(y => new OrderItemDto
                {
                    Id = y.Id,
                    LineItemPrice = y.LineItemPrice,
                    LineItemQuantity = y.LineItemQuantity,
                    LineItemTotal = y.LineItemTotal,
                    MenuItemId = y.MenuItemId
                })
            };
        }

        [HttpGet]
        public ActionResult<OrderDto> GetOrders()
        {
            var result = dataContext
                .Set<Order>()
                .Select(MapDto())
                .ToList();
            return Ok(result);
        }

        [HttpDelete("{id}/delete")]
        public ActionResult<OrderDto> DeleteOrder(int id)
        {
            var result = dataContext
                .Set<Order>()
                .FirstOrDefault(x => x.Id == id);


            if (result.Finished.HasValue)
            {
                return BadRequest("Order isn't finished!");
            }

            dataContext.Set<Order>().Remove(result);
            dataContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<OrderDto> GetById(int id)
        {
            var result = dataContext
                .Set<Order>()
                .Select(MapDto())
                .FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public ActionResult<OrderDto> Create(CreateOrderDto create)
        {
            var createdOrder = new Order
            {
                Placed = DateTimeOffset.UtcNow,
                CustomerFirst = create.CustomerFirst,
                CustomerLast = create.CustomerLast,
                Address = create.Address
            };

            var matchingIds = create.OrderItems.Select(x => x.MenuItemId);

            var relatedMenuItems = dataContext
                .Set<MenuItem>()
                .Where(x => matchingIds.Contains(x.Id))
                .ToArray();

            foreach (var item in create.OrderItems)
            {
                var menuItem = relatedMenuItems.FirstOrDefault(y => y.Id == item.MenuItemId);
                if (menuItem == null)
                {
                    return BadRequest();
                }

                var orderItem = new OrderItem 
                {
                    LineItemPrice = menuItem.Price,
                    LineItemQuantity = item.LineItemQuantity,
                    LineItemTotal = menuItem.Price * item.LineItemQuantity,
                    MenuItemId = menuItem.Id
                };
                createdOrder.OrderItems.Add(orderItem);
            }

            createdOrder.OrderTotal = createdOrder.OrderItems.Sum(x => x.LineItemTotal);
            dataContext.Set<Order>().Add(createdOrder);
            dataContext.SaveChanges();

            var dto = new OrderDto()
            {
                Id = createdOrder.Id,
                Address = createdOrder.Address,
                CustomerFirst = createdOrder.CustomerFirst,
                CustomerLast = createdOrder.CustomerLast,
                 OrderTotal = createdOrder.OrderTotal,
                 Placed = createdOrder.Placed,
                 OrderItems = createdOrder.OrderItems.Select(y => new OrderItemDto
                 {
                     Id = y.Id,
                     LineItemPrice = y.LineItemPrice,
                     LineItemQuantity = y.LineItemQuantity,
                     LineItemTotal = y.LineItemTotal,
                     MenuItemId = y.MenuItemId
                 })
            };

            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, dto);
        }

        [HttpPut("{id}/cancel")]
        public ActionResult<OrderDto> Cancel(int id)
        {
            var order = dataContext
                .Set<Order>()
                .FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Canceled != null)
            {
                return BadRequest("The order is already canceled");
            }

            if (order.Started != null)
            {
                return BadRequest("The order is already started");
            }

            if (order.Placed < DateTimeOffset.UtcNow.AddMinutes(-15))
            {
                return BadRequest("The order is older than 15 minutes");
            }

            order.Canceled = DateTimeOffset.UtcNow;
            dataContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}/finish")]
        public ActionResult<OrderDto> Finish(int id)
        {
            var order = dataContext
                .Set<Order>()
                .FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Finished != null)
            {
                return BadRequest("The order is already finished");
            }

            if (order.Started == null)
            {
                return BadRequest("The order has not been started yet!");
            }

            order.Finished = DateTimeOffset.UtcNow;
            dataContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}/received")]
        public ActionResult<OrderDto> Received(int id)
        {
            var order = dataContext
                .Set<Order>()
                .FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Finished == null)
            {
                return BadRequest("The order hasn't been finished yet!");
            }

            if (order.Started == null)
            {
                return BadRequest("The order has not been started yet!");
            }

            order.CustomerRecieved = DateTimeOffset.UtcNow;
            dataContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}/start")]
        [Authorize(Roles = RoleNames.StaffOrAdmin)]
        public ActionResult<OrderDto> Start(int id)
        {
            var order = dataContext
                .Set<Order>()
                .FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Canceled != null)
            {
                return BadRequest("The order has already been canceled.");
            }

            if (order.Started != null)
            {
                return BadRequest("The order has already been started.");
            }

            if (order.Finished != null)
            {
                return BadRequest("The order has already been finished.");
            }

            order.Started = DateTimeOffset.UtcNow;
            dataContext.SaveChanges();

            return Ok();
        }
    }
}