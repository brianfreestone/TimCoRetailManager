using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMApi.Data;
using TRMApi.Models;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            this._context = context;
            this._userManager = userManager;
            this._config = config;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData(_config);

            return data.GetUserById(userId);
        }

        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> listUserModels = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            foreach (var user in users)
            {
                ApplicationUserModel userModel = new ApplicationUserModel();
                userModel.Id = user.Id;
                userModel.Email = user.Email;

                userModel.Roles = userRoles.Where(x => x.UserId == userModel.Id).ToDictionary(key => key.RoleId, val => val.Name);

                listUserModels.Add(userModel);
            }

            return listUserModels;


        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return roles;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddARole(UserRolePairModel pair)
        {

            var user = await _userManager.FindByIdAsync(pair.UserId);
            await _userManager.AddToRoleAsync(user, pair.RoleName);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pair)
        {
            var user = await _userManager.FindByIdAsync(pair.UserId);
            await _userManager.RemoveFromRoleAsync(user, pair.RoleName);

        }

    }
}

