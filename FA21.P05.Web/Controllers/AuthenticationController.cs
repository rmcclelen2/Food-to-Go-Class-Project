using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FA21.P05.Web.Data;
using FA21.P05.Web.Extensions;
using FA21.P05.Web.Features.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FA21.P05.Web.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly DataContext dataContext;
        private readonly RoleManager<Role> roleManager;

        public AuthenticationController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            DataContext dataContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dataContext = dataContext;
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Me()
        {
            var currentUserName = User.GetCurrentUserName();
            var currentUser = await dataContext.Set<User>()
                .Where(x => x.UserName == currentUserName)
                .Select(x => new UserDto
                {
                    Name = x.Name,
                    UserName = x.UserName,
                    Schedule = x.Schedule,
                    Id = x.Id,
                    Role = x.Roles.Select(y => y.Role.Name).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return Ok(currentUser);
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = RoleNames.StaffOrAdmin)]
        public async Task<ActionResult> Create(UserLoginDto dto, string role)
        {
            const string defaultPassword = "Password123!";
            var user = await userManager.FindByNameAsync(dto.UserName);
            var currentRoles = await roleManager.RoleExistsAsync(role);
            if (!currentRoles)
            {
                return BadRequest("Role does not exist!");
            }
            if (user != null)
            {
                return BadRequest("User already exists!");
            }
            else
            {
                var newUser = new User
                {
                    Name = dto.Name,
                    Schedule = dto.Schedule,
                    UserName = dto.UserName,
                };
                await userManager.CreateAsync(newUser, defaultPassword);
                await userManager.AddToRoleAsync(newUser, role);
            }
            await dataContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("delete")]
        [Authorize(Roles = RoleNames.StaffOrAdmin)]
        public async Task<ActionResult> Delete(string UserName)
        {
            var user = dataContext
                .Set<User>()
                .FirstOrDefault(x => x.UserName == UserName);
            if (user == null)
            {
                return NotFound("User doesn't exist or has already been deleted!");
            }
            dataContext.Set<User>().Remove(user);
            await dataContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPut]
        [Route("edituser")]
        [Authorize(Roles = RoleNames.StaffOrAdmin)]
        public async Task<ActionResult<UserDto>> EditUserAsync(string UserName, UserDto userDto)
        {
            var user = dataContext
                .Set<User>()
                .FirstOrDefault(x => x.UserName == UserName);
            if (user == null)
            {
                return NotFound();
            }
            user.UserName = userDto.UserName;
            user.Name = userDto.Name;
            user.Schedule = userDto.Schedule;
            //if role is already there, delete it. if not, add it.
            var roles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await userManager .AddToRoleAsync(user, userDto.Role);
            dataContext.SaveChanges();
            return new UserDto
            {
                UserName = user.UserName,
                Name = user.Name,
                Schedule = user.Schedule
            };
        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserLoginDto dto)
        {
            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid Username or Password.");
            }
            await signInManager.SignInAsync(user, false, "Password");
            return Ok();
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
    }
}
