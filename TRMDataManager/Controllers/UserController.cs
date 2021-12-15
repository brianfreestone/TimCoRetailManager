using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();

            return data.GetUserById(userId);
        }

        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> listUserModels = new List<ApplicationUserModel>();
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                foreach (var user in users)
                {
                    ApplicationUserModel userModel = new ApplicationUserModel();
                    userModel.Id = user.Id;
                    userModel.Email = user.Email;

                    foreach (var role in user.Roles)
                    {
                        userModel.Roles.Add(role.RoleId, roles.Where(x => x.Id == role.RoleId).First().Name);
                    }
                    listUserModels.Add(userModel);
                }

                return listUserModels;

            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public void AddARole(UserRolePairModel pair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(pair.UserId, pair.RoleName);

                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public void RemoveARole(UserRolePairModel pair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(pair.UserId, pair.RoleName);

                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);
            }
        }


    }
}
