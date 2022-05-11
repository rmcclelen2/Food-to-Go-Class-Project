using System;
using System.Collections.Generic;
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
    [Route("api/menu-items")]
    public class MenuItemController : ControllerBase
    {
        private readonly DataContext dataContext;

        public MenuItemController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private static Expression<Func<MenuItem, MenuItemDto>> MapDto()
        {
            return x => new MenuItemDto
            {
                Id = x.Id,
                Price = x.Price,
                Description = x.Description,
                IsSpecial = x.IsSpecial,
                IsEntree = x.IsEntree,
                IsSide = x.IsSide,
                IsDrink = x.IsDrink,
                Image = x.Image,
                Name = x.Name
            };
        }

        [HttpGet]
        public ActionResult<IEnumerable<MenuItemDto>> Get()
        {
            return GetDtos().ToList();
        }

        private IQueryable<MenuItemDto> GetDtos()
        {
            return dataContext.Set<MenuItem>().Select(MapDto());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<MenuItemDto> GetById(int id)
        {
            var result = dataContext
                .Set<MenuItem>()
                .Select(MapDto())
                .FirstOrDefault(x => x.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleNames.StaffOrAdmin)]
        public ActionResult<MenuItemDto> Update(int id, MenuItemDto item)
        {
            var entity = dataContext
                .Set<MenuItem>()
                .FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            if (item.Price <= 0)
            {
                return BadRequest();
            }

            entity.Price = item.Price;
            entity.Description = item.Description;
            entity.Name = item.Name;
            entity.IsSpecial = item.IsSpecial;
            entity.IsEntree = item.IsEntree;
            entity.IsSide = item.IsSide;
            entity.IsDrink = item.IsDrink;
            entity.Image = item.Image;
            dataContext.SaveChanges();

            return new MenuItemDto
            {
                Id = entity.Id,
                Price = entity.Price,
                Description = entity.Description,
                IsSpecial = entity.IsSpecial,
                IsEntree = entity.IsEntree,
                IsSide = entity.IsSide,
                IsDrink = entity.IsDrink,
                Image = entity.Image,
                Name = entity.Name
            };
        }

        [HttpGet]
        [Route("specials")]
        public ActionResult<IEnumerable<MenuItemDto>> GetSpecials()
        {
            var result = dataContext
                .Set<MenuItem>()
                .Where(x => x.IsSpecial)
                .Select(MapDto())
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        [Route("entrees")]
        public ActionResult<IEnumerable<MenuItemDto>> GetEntrees()
        {
            var result = dataContext
                .Set<MenuItem>()
                .Where(x => x.IsEntree)
                .Select(MapDto())
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        [Route("sides")]
        public ActionResult<IEnumerable<MenuItemDto>> GetSides()
        {
            var result = dataContext
                .Set<MenuItem>()
                .Where(x => x.IsSide)
                .Select(MapDto())
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        [Route("drinks")]
        public ActionResult<IEnumerable<MenuItemDto>> GetDrinks()
        {
            var result = dataContext
                .Set<MenuItem>()
                .Where(x => x.IsDrink)
                .Select(MapDto())
                .ToList();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult<MenuItemDto> Create(MenuItemDto menuItem)
        {
            if (menuItem.Price <= 0)
            {
                return BadRequest();
            }

            var item = dataContext
                .Set<MenuItem>()
                .Add(new MenuItem
                {
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    IsSpecial = menuItem.IsSpecial,
                    IsDrink = menuItem.IsDrink,
                    IsSide = menuItem.IsSide,
                    IsEntree = menuItem.IsEntree,
                    Image = menuItem.Image,
                    Name = menuItem.Name
                });

            dataContext.SaveChanges();
            menuItem.Id = item.Entity.Id;

            return CreatedAtAction(nameof(GetById), new { id = menuItem.Id }, menuItem);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public ActionResult Delete(int id)
        {
            var entity = dataContext
                .Set<MenuItem>()
                .FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            var anyOrdersWithItem = dataContext
                .Set<OrderItem>()
                .Any(x => x.MenuItemId == id);

            if (anyOrdersWithItem)
            {
                return BadRequest();
            }

            dataContext.Set<MenuItem>().Remove(entity);
            dataContext.SaveChanges();

            return Ok();
        }
    }
}
