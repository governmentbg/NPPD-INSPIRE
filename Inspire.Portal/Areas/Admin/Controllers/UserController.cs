namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Role;
    using Inspire.Model.User;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.Group;
    using Inspire.Portal.Areas.Admin.Models.Queries;
    using Inspire.Portal.Areas.Admin.Models.User;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.GeoNetwork.Group;
    using Inspire.Portal.Models.GeoNetwork.User;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Services.UserMailService;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using Newtonsoft.Json;

    using GeoNetWorkProfile = Inspire.Models.GeoNetwork.User.Profile;

    [CustomAuthorize]
    public class UserController : SearchTableController<UserQueryModel, UserTableViewModel>
    {
        private readonly IAccountService accountService;
        private readonly INomenclatureService nomenclatureService;
        private readonly IRestApiService restApiService;
        private readonly IRoleService roleService;
        private readonly IUserMailService userMailService;
        private readonly IUserService userService;
        private readonly IGroupService groupService;

        public UserController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IUserService userService,
            IUserMailService userMailService,
            IAccountService accountService,
            INomenclatureService nomenclatureService,
            IRoleService roleService,
            IRestApiService restApiService,
            IGroupService groupService)
            : base(logger, mapper, contextManager, resource, Resource.Users)
        {
            this.userService = userService;
            this.userMailService = userMailService;
            this.accountService = accountService;
            this.nomenclatureService = nomenclatureService;
            this.roleService = roleService;
            this.restApiService = restApiService;
            this.groupService = groupService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchUser)]
        public override ActionResult Index(UserQueryModel model = null)
        {
            InitBreadcrumb(Title);
            return base.Index(model);
        }

        [HttpGet]
        public override ActionResult TableControls()
        {
            return PartialView("_TableControls");
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertUser)]
        public ActionResult Upsert(Guid? id)
        {
            var model = new UserUpsertViewModel();

            List<Nomenclature> profiles;
            using (ContextManager.NewConnection())
            {
                ViewBag.Roles = roleService.Search(new RoleQuery { UserId = User.Id });
                profiles = nomenclatureService.Get("nrolegnw");

                if (id.HasValue)
                {
                    model = Mapper.Map<UserUpsertViewModel>(userService.Get(id.Value));
                    model.Roles = roleService.GetUserRoles(id.Value);
                }
            }

            //// when register user there is no geonetwork account yet
            if (model.GeoNetworkId.HasValue)
            {
                using (var client = restApiService.GetClient())
                {
                    var geoNetworkUser = restApiService.GetRequest<UserDTO>(client, $"users/{model.GeoNetworkId}");

                    model.IsAdministrator = geoNetworkUser.Profile == Enum.GetName(
                        typeof(GeoNetWorkProfile),
                        GeoNetWorkProfile.Administrator);

                    model.GeoNetworkAddress = geoNetworkUser.Addresses?.FirstOrDefault();

                    if (!model.IsAdministrator)
                    {
                        var geoNetworkUserGroups = restApiService.GetRequest<List<UserGroup>>(
                            client,
                            $"users/{model.GeoNetworkId}/groups");
                        var firstGroupId = geoNetworkUserGroups.FirstOrDefault()?.Id;

                        model.Group = new GroupForUser
                        {
                            Id = firstGroupId?.GroupId
                        };

                        model.Profile = profiles?.Single(
                            item => item.Id.Equals(
                                EnumHelper.GetProfileIdByProfile(
                                    (GeoNetWorkProfile)Enum.Parse(
                                        typeof(GeoNetWorkProfile),
                                        firstGroupId?.Profile,
                                        true))));
                    }
                }
            }

            this.InitAdminBreadcrumb(
                Title,
                string.Format(
                    model.Id.HasValue
                        ? string.Format(Resource.Editing, model.UserName)
                        : string.Format(Resource.Creating, Resource.User.ToLower())),
                true);

            return Request.IsAjaxRequest()
                ? PartialView(model) as ActionResult
                : View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertUser)]
        public ActionResult Upsert(UserUpsertViewModel model)
        {
            var isNewUser = !model.Id.HasValue;
            if (isNewUser)
            {
                User existingUser;
                using (ContextManager.NewConnection())
                {
                    existingUser = accountService.GetByUserName(model.UserName);
                }

                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", Resource.UserNameIsAlreadyTaken);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = restApiService.GetClient())
                    {
                        var json = JsonConvert.SerializeObject(
                            Mapper.Map<UserDTO>(model),
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCaseContractResolver()
                            });

                        restApiService.PutRequest(
                            client,
                            isNewUser ? "users" : $"users/{model.GeoNetworkId}",
                            new StringContent(json, Encoding.UTF8, "application/json"));

                        if (isNewUser && !model.GeoNetworkId.HasValue)
                        {
                            var geonetWorkUsers = restApiService.GetRequest<List<UserDTO>>(client, "users");
                            model.GeoNetworkId = Convert.ToInt64(
                                geonetWorkUsers?.FirstOrDefault(x => x.Username.Equals(model.UserName))?.Id);

                            if (model.GeoNetworkId <= 0)
                            {
                                throw new UserException(Resource.UserNotFound);
                            }
                        }
                    }

                    var user = Mapper.Map<User>(model);
                    using (var transaction = ContextManager.NewTransaction())
                    {
                        // set group id from our db
                        var group = groupService.Search(new GroupQueryModel { Name = model.Group?.Name }).FirstOrDefault();
                        user.OrganizationId = group?.Id;

                        user.Id = userService.Upsert(user);

                        userService.SetUserRoles(
                            new SetRole
                            {
                                UserId = user.Id.Value,
                                Roles = model.Roles
                            });

                        transaction.Commit();
                    }

                    if (isNewUser)
                    {
                        userMailService.SendCompleteRegistrationEmail(user);
                    }

                    this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
                    return RedirectToAction("Index", new { user.Id });
                }
                catch (UserDbException e)
                {
                    Logger.Error(e);
                    ModelState.AddModelError(string.Empty, Resource.DbErrorMessage);
                }
                catch (UserException e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            using (ContextManager.NewConnection())
            {
                ViewBag.Roles = roleService.Search(new RoleQuery { UserId = User.Id });
            }

            InitBreadcrumb(
                string.Format(
                    model.Id.HasValue
                        ? string.Format(Resource.Editing, model.UserName)
                        : string.Format(Resource.Creating, Resource.User.ToLower())));
            return View(model);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertUser)]
        public ActionResult SearchPositions(string text)
        {
            List<string> result;
            using (ContextManager.NewConnection())
            {
                result = userService.SearchPositionByText(text);
            }

            return new JsonResultMaxLength(result);
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_TableTemplate");
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.ChangeUserStatus)]
        public ActionResult ChangeStatus(Guid id, string searchQueryId = null)
        {
            var user = FindResults(new UserQueryModel { Id = id }).FirstOrDefault();
            if (user == null)
            {
                throw new WarningException(Resource.NoDataFound);
            }

            var model = new ChangeStatus
            {
                UserId = user.Id,
                StatusId = user.StatusId
            };

            ViewBag.SearchQueryId = searchQueryId;
            return Request.IsAjaxRequest()
                ? PartialView(model) as ActionResult
                : View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.ChangeUserStatus)]
        public ActionResult ChangeStatus(ChangeStatus model, string searchQueryId = null)
        {
            if (!ModelState.IsValid)
            {
                return Json(RenderRazorViewToString("ChangeStatus", model), JsonRequestBehavior.AllowGet);
            }

            UserTableViewModel updatedUser;
            User user;
            using (var transaction = ContextManager.NewTransaction())
            {
                userService.ChangeStatus(model.StatusId, model.UserId, User.Id);
                transaction.Commit();

                user = userService.Get(model.UserId);

                updatedUser = Mapper.Map<UserTableViewModel>(
                    userService.Search(new UserQuery { Id = model.UserId }).SingleOrDefault());
            }

            if (model.StatusId == EnumHelper.GetStatusIdByEnum(UserStatus.Active))
            {
                userMailService.SendChangePasswordMail(user);
            }

            RefreshGridItem(searchQueryId, updatedUser, x => x.Id == updatedUser.Id);
            return Json(
                new { success = true, refreshgrid = true, searchqueryid = searchQueryId },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.ChangeUserStatus)]
        public ActionResult GetUserStatuses()
        {
            List<Nomenclature> result;
            using (ContextManager.NewConnection())
            {
                result = nomenclatureService.Get("nuserstatus").ToList();
            }

            return new JsonResultMaxLength(
                result.Where(item => !item.Id.Equals(Guid.Parse("5973b840-1794-4a78-8de0-db1a511ccfa8"))));
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.RolesManagement)]
        public ActionResult SetRole(Guid id, string searchQueryId = null)
        {
            var model = new SetRoleUpsertModel
            {
                UserId = id
            };

            ViewBag.SearchQueryId = searchQueryId;

            using (ContextManager.NewConnection())
            {
                model.Roles = roleService.GetUserRoles(id);
                ViewBag.Roles = roleService.Search(new RoleQuery { UserId = User.Id });
            }

            return PartialView("_SetRole", model);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.RolesManagement)]
        public ActionResult GetAllRoles()
        {
            List<Role> result;
            using (ContextManager.NewConnection())
            {
                result = roleService.Search(new RoleQuery { UserId = User.Id }).ToList();
            }

            return Json(Mapper.Map<List<Nomenclature>>(result), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.RolesManagement)]
        public ActionResult SetRole(SetRoleUpsertModel model, string searchQueryId = null)
        {
            if (!ModelState.IsValid)
            {
                using (ContextManager.NewConnection())
                {
                    ViewBag.Roles = roleService.Search(new RoleQuery());
                }

                return Json(RenderRazorViewToString("_SetRole", model), JsonRequestBehavior.AllowGet);
            }

            var mappedModel = Mapper.Map<SetRole>(model);
            UserTableViewModel updatedUser;
            using (var transaction = ContextManager.NewTransaction())
            {
                userService.SetUserRoles(mappedModel);
                transaction.Commit();
                updatedUser = Mapper.Map<UserTableViewModel>(
                    userService.Search(new UserQuery { Id = model.UserId }).SingleOrDefault());
            }

            RefreshGridItem(searchQueryId, updatedUser, x => x.Id == updatedUser.Id);
            return Json(
                new { success = true, refreshgrid = true, searchqueryid = searchQueryId },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Info(Guid id)
        {
            User user;
            using (ContextManager.NewConnection())
            {
                user = userService.Get(id);
            }

            return PartialView("Info", user);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.SendChangePasswordMail)]
        public ActionResult SendForgottenPasswordMail(Guid id)
        {
            User user;
            using (ContextManager.NewConnection())
            {
                user = userService.Get(id);
            }

            userMailService.SendChangePasswordMail(user);

            this.ShowMessage(MessageType.Success, Resource.AdminForgottenPasswordLinkSuccess);

            var url = Url.Action("Index", new UserQueryModel { Id = id });
            if (Request.IsAjaxRequest())
            {
                this.SetAjaxResponseRedirectUrl(url, true);
                return new EmptyResult();
            }

            return Redirect(url);
        }

        [HttpGet]
        public override ActionResult Scripts()
        {
            return PartialView("_Scripts");
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        [HttpGet]
        public ActionResult GetUsersByRole(Guid? id, string text = null)
        {
            List<Nomenclature> result;
            using (ContextManager.NewConnection())
            {
                result = userService.GetUsersByRole(id);
            }

            result.Insert(0, new Nomenclature { Id = null, Name = Resource.All });
            if (text.IsNotNullOrEmpty())
            {
                return new JsonResultMaxLength(
                    result.Where(item => item.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0));
            }

            return new JsonResultMaxLength(result);
        }

        [HttpGet]
        public ActionResult GetProfiles()
        {
            using (ContextManager.NewConnection())
            {
                return new JsonResultMaxLength(nomenclatureService.Get("nrolegnw"));
            }
        }

        [HttpGet]
        public ActionResult GetGroups()
        {
            // If not send profile, all groups are returned
            // var profiles = ['Administrator', 'UserAdmin', 'Reviewer', 'Editor', 'RegisteredUser', 'Guest'];
            var profile = "UserAdmin";
            using (var client = restApiService.GetClient())
            {
                return new JsonResultMaxLength(restApiService.GetRequest<List<GroupDTO>>(client, $"groups?profile={profile}"));
            }
        }

        protected override void InitialQuery(UserQueryModel model)
        {
            List<Nomenclature> statuses, organisations;
            using (ContextManager.NewConnection())
            {
                statuses = nomenclatureService.Get("nuserstatus").ToList();
                organisations = userService.GetOrganisationsForDdl();
            }

            model.StatusDataSource = statuses
                                     .Select(item => new KeyValuePair<string, string>(item.Id?.ToString(), item.Name))
                                     .ToList().AddDefaultValue();
            model.OrganisationDataSource = organisations
                                           .Select(item => new KeyValuePair<string, string>(item.Id?.ToString(), item.Name))
                                           .ToList().AddDefaultValue();
        }

        [CustomAuthorize(ApplicationRights.SearchUser)]
        protected override IEnumerable<UserTableViewModel> FindResults(UserQueryModel query)
        {
            List<User> result;
            using (ContextManager.NewConnection())
            {
                result = userService.Search(Mapper.Map<UserQuery>(query)).ToList();
            }

            var model = Mapper.Map<List<UserTableViewModel>>(result);

            return model;
        }

        private void InitBreadcrumb(string title)
        {
            var breadcrumbs = new List<Breadcrumb>
                              {
                                  new Breadcrumb
                                  {
                                      Title = Resource.Admin
                                  }
                              };

            this.InitViewTitleAndBreadcrumbs(title, breadcrumbs);
        }
    }
}