namespace Inspire.Portal.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Enums;
    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Filters.CustomAuthorize;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Group;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.Group;
    using Inspire.Portal.Areas.Admin.Models.Queries;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.GeoNetwork.Group;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Utilities;
    using Inspire.Table.Mvc.Controllers;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using Newtonsoft.Json;

    [CustomAuthorize]
    public class GroupController : SearchTableController<GroupQueryViewModel, GroupTableViewModel>
    {
        private readonly IGroupService groupService;
        private readonly IRestApiService restApiService;

        public GroupController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IResourceManager resource,
            IRestApiService restApiService,
            IGroupService groupService)
            : base(logger, mapper, contextManager, resource, Resource.Organizations)
        {
            this.restApiService = restApiService;
            this.groupService = groupService;
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.SearchOrganization)]
        public override ActionResult Index(GroupQueryViewModel model = null)
        {
            this.InitAdminBreadcrumb(
                Title,
                Resource.Organizations);

            return base.Index(model);
        }

        [HttpGet]
        [CustomAuthorize(ApplicationRights.UpsertOrganization)]
        public ActionResult Upsert(Guid? id)
        {
            var model = new GroupUpsertModel();
            if (id.HasValue)
            {
                Group group;
                using (ContextManager.NewConnection())
                {
                    group = groupService.Get(id.Value);
                }

                model = Mapper.Map<GroupUpsertModel>(group);
                InitGroupUpsertModel(model, group.GeoNetworkId);
            }

            this.InitAdminBreadcrumb(
                Title,
                string.Format(
                    model.Id.HasValue
                        ? string.Format(Resource.Editing, model.Names.GetValueForCurrentCulture())
                        : string.Format(Resource.Creating, Resource.Organization.ToLower())),
                true);

            return Request.IsAjaxRequest()
                ? PartialView(model) as ActionResult
                : View(model);
        }

        [HttpPost]
        [CustomAuthorize(ApplicationRights.UpsertOrganization)]
        public ActionResult Upsert(GroupUpsertModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = restApiService.GetClient())
                    {
                        // read all cagegories from api to map selected ones. The api needs all properties, not only ids!!!
                        var all = restApiService.GetRequest<List<Category>>(client, "tags");
                        var groupDto = Mapper.Map<GroupDTO>(model);
                        groupDto.DefaultCategory = model.DefaultCategory != null
                            ? all.Single(item => item.Id.Equals(model.DefaultCategory.Id))
                            : null;
                        groupDto.AllowedCategories = all.Where(
                            item => model.SelectedAllowedCategories.IsNotNullOrEmpty() &&
                                    model.SelectedAllowedCategories.Contains(item.Id.Value));

                        var json = SerializeGroupDTOToJson(groupDto);
                        restApiService.PutRequest(
                            client,
                            model.GeoNetworkId.HasValue ? $"groups/{model.GeoNetworkId}" : "groups",
                            new StringContent(json, Encoding.UTF8, "application/json"));

                        var geoNetworkGroup = restApiService.GetRequest<List<GroupDTO>>(client, "groups")
                                                            ?.Single(
                                                                item => item.Name == model.Names.GetValueForCurrentCulture());
                        model.GeoNetworkId = Convert.ToInt64(geoNetworkGroup.Id);

                        InitLabels(geoNetworkGroup, model);
                        json = SerializeGroupDTOToJson(geoNetworkGroup);

                        restApiService.PutRequest(
                            client,
                            $"groups/{model.GeoNetworkId}",
                            new StringContent(json, Encoding.UTF8, "application/json"));
                    }

                    var group = Mapper.Map<Group>(model);
                    using (var transaction = ContextManager.NewTransaction())
                    {
                        groupService.Upsert(group);
                        transaction.Commit();
                    }

                    this.ShowMessage(MessageType.Success, Resource.ChangesSuccessfullySaved);
                    return RedirectToAction("Index", new { group.Id });
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    ModelState.AddModelError(string.Empty, e is UserException userException ? userException.Message : Resource.InternalServerError);
                }
            }

            this.InitAdminBreadcrumb(
                Title,
                string.Format(
                    model.Id.HasValue
                        ? string.Format(Resource.Editing, model.Names.GetValueForCurrentCulture())
                        : string.Format(Resource.Creating, Resource.Organization.ToLower())),
                true);

            return Request.IsAjaxRequest()
                ? PartialView(model) as ActionResult
                : View(model);
        }

        [HttpGet]
        public ActionResult GetCategories()
        {
            using (var client = restApiService.GetClient())
            {
                return Json(restApiService.GetRequest<List<Category>>(client, "tags"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public override ActionResult TableControls()
        {
            return PartialView("_TableControls");
        }

        [HttpGet]
        public override ActionResult ClientTemplate()
        {
            return PartialView("_TableTemplate");
        }

        [HttpGet]
        public override ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        [HttpGet]
        public override ActionResult Scripts()
        {
            return PartialView("_Scripts");
        }

        protected override IEnumerable<GroupTableViewModel> FindResults(GroupQueryViewModel query)
        {
            List<GroupTableViewModel> list;

            using (ContextManager.NewConnection())
            {
                list = Mapper.Map<List<GroupTableViewModel>>(groupService.Search(Mapper.Map<GroupQueryModel>(query)));
            }

            return list;
        }

        private void InitGroupUpsertModel(GroupUpsertModel model, long? geoNetworkId)
        {
            using (var client = restApiService.GetClient())
            {
                var geoNetworkGroup = restApiService.GetRequest<GroupDTO>(client, $"groups/{geoNetworkId}");

                model.EnableAllowedCategories = geoNetworkGroup?.EnableAllowedCategories ?? false;
                model.Logo = geoNetworkGroup?.Logo;
                model.Label = geoNetworkGroup?.Label;
                model.DefaultCategory = geoNetworkGroup?.DefaultCategory;
                model.SelectedAllowedCategories = geoNetworkGroup?.AllowedCategories?.Select(x => x.Id.Value).ToList();
            }
        }

        private string SerializeGroupDTOToJson(GroupDTO groupDto)
        {
            return JsonConvert.SerializeObject(
                groupDto,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCaseContractResolver()
                });
        }

        private void InitLabels(GroupDTO geoNetworkGroup, GroupUpsertModel model)
        {
            if (geoNetworkGroup.Label.IsNotNullOrEmpty() && model.Names.IsNotNullOrEmpty())
            {
                foreach (var name in model.Names)
                {
                    var threeLetterLanguageName = Global.Cultures.FirstOrDefault(x => x.Value == name.Key).Key
                                                        .ThreeLetterISOLanguageName.ToLower();

                    if (geoNetworkGroup.Label.ContainsKey(threeLetterLanguageName))
                    {
                        geoNetworkGroup.Label[threeLetterLanguageName] = name.Value;
                    }
                }
            }
        }
    }
}