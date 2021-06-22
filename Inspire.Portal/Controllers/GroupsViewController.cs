namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Mvc;

    using AutoMapper;

    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model.Group;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Models.Queries;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.GeoNetwork.Group;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Extensions;

    [AllowAnonymous]
    public class GroupsViewController : BaseDbController
    {
        private readonly IGroupService groupService;
        private readonly IRestApiService restApiService;

        public GroupsViewController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            IGroupService groupService,
            IRestApiService restApiService)
            : base(logger, mapper, contextManager)
        {
            this.groupService = groupService;
            this.restApiService = restApiService;
        }

        public ActionResult Index()
        {
            this.InitViewTitleAndBreadcrumbs(Resource.Organizations);

            List<GroupTableModel> result;

            using (ContextManager.NewConnection())
            {
                result = groupService.Search(new GroupQueryModel());
            }

            InitGeoNetworkData(result);

            return View(result);
        }

        [HttpGet]
        public ActionResult Breadcrumbs()
        {
            return PartialView("_Breadcrums");
        }

        public ActionResult Info(Guid id)
        {
            Group organization;
            using (ContextManager.NewConnection())
            {
                organization = groupService.Get(id);
            }

            this.InitViewTitleAndBreadcrumbs(
                organization.Names.GetValueForCurrentCulture(),
                new[]
                {
                    new Breadcrumb
                    {
                        Title = Resource.Organizations,
                        Url = Url.Action("Index")
                    }
                });

            return Request.IsAjaxRequest()
                ? PartialView(organization) as ActionResult
                : View(organization);
        }

        private void InitGeoNetworkData(IReadOnlyCollection<GroupTableModel> items)
        {
            if (items.IsNotNullOrEmpty())
            {
                using (var client = restApiService.GetClient())
                {
                    foreach (var item in items)
                    {
                        var groupDto = restApiService.GetRequest<GroupDTO>(client, $"groups/{item.GeoNetworkId}");
                        if (groupDto.Logo.IsNullOrEmptyOrWhiteSpace())
                        {
                            continue;
                        }

                        var request = (HttpWebRequest)WebRequest.Create($"{ConfigurationReader.GeoNetworkAddress}images/harvesting/{groupDto.Logo}");
                        item.Logo = Convert.ToBase64String(((HttpWebResponse)request.GetResponse()).GetResponseStream().ToByteArray());
                    }
                }
            }
        }
    }
}