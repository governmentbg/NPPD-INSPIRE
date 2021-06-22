namespace Inspire.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;
    using System.Xml.Linq;

    using AutoMapper;

    using Inspire.Common.Mvc.Extensions;
    using Inspire.Common.Mvc.Infrastructure.BaseTypes;
    using Inspire.Common.Mvc.Infrastructure.CustomResult;
    using Inspire.Core.Infrastructure.Cache;
    using Inspire.Core.Infrastructure.Logger;
    using Inspire.Core.Infrastructure.TransactionManager;
    using Inspire.Domain.Services;
    using Inspire.Model;
    using Inspire.Model.Cms;
    using Inspire.Model.Faq;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Provider;
    using Inspire.Model.Publication;
    using Inspire.Model.QueryModels;
    using Inspire.Model.Sitemap;
    using Inspire.Model.TableModels;
    using Inspire.Portal.App_GlobalResources;
    using Inspire.Portal.Areas.Admin.Controllers;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Metadata;
    using Inspire.Portal.Models.Provider;
    using Inspire.Portal.Models.Publication;
    using Inspire.Portal.Services.RestApiService;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using Newtonsoft.Json.Linq;

    [RoutePrefix("")]
    public class HomeController : BaseDbController
    {
        private readonly IAdminService adminService;
        private readonly ICacheService cacheService;
        private readonly ICmsService cmsService;
        private readonly IFaqService faqService;
        private readonly INomenclatureService nomenclatureService;
        private readonly IProviderService providerService;
        private readonly IPublicationService publicationService;
        private readonly IRestApiService restApiService;

        public HomeController(
            ILogger logger,
            IMapper mapper,
            IDbContextManager contextManager,
            INomenclatureService nomenclatureService,
            IPublicationService publicationService,
            IAdminService adminService,
            ICacheService cacheService,
            IFaqService faqService,
            ICmsService cmsService,
            IRestApiService restApiService,
            IProviderService providerService)
            : base(logger, mapper, contextManager)
        {
            this.nomenclatureService = nomenclatureService;
            this.publicationService = publicationService;
            this.adminService = adminService;
            this.cacheService = cacheService;
            this.faqService = faqService;
            this.cmsService = cmsService;
            this.restApiService = restApiService;
            this.providerService = providerService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.IsHomePage = true;
            ViewBag.SearchInfo = cacheService.GetOrSetCache(
                "geonetworkRecordsCount",
                () =>
                {
                    try
                    {
                        using (var client = restApiService.GetClient())
                        {
                            return restApiService.PostRequest<JObject>(
                                client,
                                "search/records/_search",
                                new StringContent(
                                    ApplicationData.ReadGeonetworkConfigFile("SearchInfo.json"),
                                    Encoding.UTF8,
                                    "application/json"));
                        }
                    }
                    catch (Exception exc)
                    {
                        Logger.Error(exc);
                        return null;
                    }
                });

            using (ContextManager.NewConnection())
            {
                ViewBag.Providers = providerService.Search(
                    new ProviderQueryModel { StatusId = EnumHelper.ProviderStatuses[ProviderStatus.Valid] });
                ViewBag.BackgroundImages = adminService.GetHomeImages();
            }

            return View();
        }

        [HttpGet]
        public ActionResult ChangeCulture(string lang)
        {
            CultureInfo newClientCulture = null;
            try
            {
                newClientCulture = CultureInfo.GetCultureInfo(lang);
            }
            catch
            {
                // ignored
            }

            if (newClientCulture != null && Global.AllowCultures.All(
                item => item.TwoLetterISOLanguageName !=
                        newClientCulture.TwoLetterISOLanguageName))
            {
                newClientCulture = null;
            }

            var urlReferrer = Request.UrlReferrer.AbsolutePath;
            string redirectUrl;
            var regexPattern =
                $"(?i)\\/{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower()}\\/|(?i)\\/{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower()}$";

            if (newClientCulture != null
                && Url.IsLocalUrl(urlReferrer)
                && Regex.IsMatch(urlReferrer, regexPattern))
            {
                var newPath = Regex.Replace(
                    urlReferrer,
                    regexPattern,
                    $"/{newClientCulture.TwoLetterISOLanguageName.ToLower()}/");
                redirectUrl = $"{newPath}{Request.UrlReferrer.Query}";
            }
            else
            {
                redirectUrl = Url.RouteUrl(
                    "Default",
                    new
                    {
                        culture = newClientCulture?.TwoLetterISOLanguageName.ToLower(),
                        controller = string.Empty,
                        action = string.Empty
                    });
            }

            if (Request.IsAjaxRequest())
            {
                this.SetAjaxResponseRedirectUrl(redirectUrl, true);
                return new EmptyResult();
            }

            return Redirect(redirectUrl);
        }

        [OutputCache(CacheProfile = "Default")]
        [HttpGet]
        public JsonResult Resources()
        {
            var key = $"Resources_{CurrentCulture.TwoLetterISOLanguageName}";
            var resource = cacheService.GetOrSetCache(
                key,
                () => typeof(Resource)
                      .GetProperties()
                      .ToDictionary(p => p.Name, p => p.GetValue(null) as string));
            return new JsonResultMaxLength(resource);
        }

        [OutputCache(CacheProfile = "Default")]
        [HttpGet]
        public JsonResult Nomenclature(string name)
        {
            List<Nomenclature> result;
            using (ContextManager.NewConnection())
            {
                result = nomenclatureService.Get(name).ToList();
            }

            return new JsonResultMaxLength(result);
        }

        [ChildActionOnly]
        [HttpGet]
        public ActionResult LatestNews(int count = 2)
        {
            List<Publication> result;
            using (ContextManager.NewConnection())
            {
                result = publicationService.GetVisiblePublicationsByType(PublicationType.News);
            }

            result = result
                     .Where(item => item.IsLead)
                     .OrderByDescending(item => item.StartDate)
                     .Take(count)
                     .ToList();
            var data = Mapper.Map<List<PublicationPublicViewModel>>(result);
            PublicContentHelper.DecodeAndTrimContent(data, ConfigurationReader.LeadNewsTrimLength);
            return PartialView("_LatestNews", data);
        }

        [ChildActionOnly]
        [HttpGet]
        public ActionResult LatestEvents()
        {
            using (ContextManager.NewConnection())
            {
                return PartialView("_LatestEvents", publicationService.GetVisiblePublicationsByType(PublicationType.Event));
            }
        }

        [ChildActionOnly]
        [HttpGet]
        public ActionResult LeadChart()
        {
            return new EmptyResult();
        }

        [ChildActionOnly]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Menu()
        {
            var menu = GetMenuItems();
            return PartialView("_Menu", menu);
        }

        [ChildActionOnly]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Header()
        {
            InitUISettings();
            return PartialView("_Header");
        }

        [ChildActionOnly]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult HeaderMenu()
        {
            var menuItems = GetCmsHeaderMenuItems();
            return PartialView("_HeaderMenu", menuItems);
        }

        [ChildActionOnly]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Footer()
        {
            InitUISettings();
            ViewBag.FooterMenuItems =
                (GetCmsMenuItems() ?? new List<MenuItem>()).Union(GetCmsHeaderMenuItems() ?? new List<MenuItem>());
            return PartialView("_Footer");
        }

        [HttpGet]
        public ActionResult Faq(Guid categoryId)
        {
            FaqCategory category;
            List<FaqTableModel> faq = null;
            using (ContextManager.NewConnection())
            {
                category = faqService.GetFaqCategory(categoryId, false);
                if (category != null)
                {
                    faq = faqService.Search(new FaqQueryModel { Category = new Nomenclature { Id = categoryId }, Status = EnumHelper.GetFaqStatus(FaqStatus.Public).Value });
                }
            }

            if (category == null)
            {
                throw new UserException(Resource.NoDataFound);
            }

            ViewBag.CategoryId = categoryId;

            this.InitViewTitleAndBreadcrumbs(category.Names.GetValueForCurrentCulture(), new[] { new Breadcrumb { Title = Resource.FAQ } });
            return View("Faq", faq);
        }

        [HttpPost]
        public ActionResult SearchFaq(Guid categoryId, string searchWord)
        {
            FaqCategory category;
            List<FaqTableModel> faq = null;
            using (ContextManager.NewConnection())
            {
                category = faqService.GetFaqCategory(categoryId, false);
                if (category != null)
                {
                    faq = faqService.Search(new FaqQueryModel { Category = new Nomenclature { Id = categoryId }, SearchWord = $"%{searchWord}%", Status = EnumHelper.GetFaqStatus(FaqStatus.Public).Value });
                }
            }

            if (category == null)
            {
                throw new UserException(Resource.NoDataFound);
            }

            ViewBag.CategoryId = categoryId;

            return PartialView("_FaqResult", faq);
        }

        [Route("sitemap.xml")]
        [HttpGet]
        public ActionResult SitemapXml()
        {
            List<Page> pages;
            using (ContextManager.NewConnection())
            {
                pages = cmsService.SearchPages(
                    new PageQueryModel
                    {
                        VisibilityTypeId = EnumHelper.GetPageVisibilityTypeId(VisibilityType.Public)
                    });
            }

            var sitemapNodes = pages.Select(
                page =>
                    new SitemapNode
                    {
                        Url = GetUrl(page.PermanentLink, true)
                    });

            var xml = GetSitemapDocument(sitemapNodes);
            return Content(xml, MimeTypes.GetMimeType("sitemap.xml"), Encoding.UTF8);
        }

        [HttpGet]
        public JsonResult Suggest(string text)
        {
            List<string> recordNames = null;
            try
            {
                var jsonQuery = JObject.Parse(ApplicationData.GeonetworkSearchSuggestConfig.ToString());
                jsonQuery.SelectToken("$...multi_match.query").Replace(text);

                JObject json;
                using (var client = restApiService.GetClient())
                {
                    json = restApiService.PostRequest<JObject>(
                        client,
                        "search/records/_search",
                        new StringContent(
                            jsonQuery.ToString(),
                            Encoding.UTF8,
                            "application/json"));
                }

                recordNames =
                    json["hits"]["hits"]
                        .Select(item => item["_source"]["resourceTitleObject"]?.Translate())
                        .ToList();
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
            }

            return Json(recordNames, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LeadThemes(LeadThemeType type)
        {
            var jsonQuery = JObject.Parse(ApplicationData.GeonetworkSearchLeadThemesConfig.ToString());
            switch (type)
            {
                case LeadThemeType.Latest:
                    {
                        ((JArray)jsonQuery.SelectToken("sort"))?.Add(new JObject { { "createDate", "desc" } });
                        break;
                    }

                case LeadThemeType.MostPopular:
                    {
                        ((JArray)jsonQuery.SelectToken("sort"))?.Add(new JObject { { "popularity", "asc" } });
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            JObject json = null;
            try
            {
                using (var client = restApiService.GetClient())
                {
                    json = restApiService.PostRequest<JObject>(
                        client,
                        "search/records/_search",
                        new StringContent(
                            jsonQuery.ToString(),
                            Encoding.UTF8,
                            "application/json"));
                }
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
            }

            return PartialView("_MatadataCollection", json);
        }

        [ChildActionOnly]
        [HttpGet]
        public ActionResult GetProviders()
        {
            var result = new List<ProviderPublicViewModel>();
            using (ContextManager.NewConnection())
            {
                result = Mapper.Map<List<ProviderPublicViewModel>>(providerService.Search(
                                                                       new ProviderQueryModel() { StatusId = EnumHelper.ProviderStatuses[ProviderStatus.Valid] }));
            }

            PublicContentHelper.DecodeAndTrimContent(result, ConfigurationReader.ProviderTrimLength);
            return PartialView("_Providers", result);
        }

        private static string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            foreach (var sitemapNode in sitemapNodes.Where(item => item.Url != null))
            {
                var urlElement = new XElement(
                    xmlns + "url",
                    new XElement(
                        xmlns + "loc",
                        sitemapNode.Url != null ? Uri.EscapeUriString(sitemapNode.Url) : string.Empty),
                    sitemapNode.LastModified == null
                        ? null
                        : new XElement(
                            xmlns + "lastmod",
                            sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null
                        ? null
                        : new XElement(
                            xmlns + "changefreq",
                            sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null
                        ? null
                        : new XElement(
                            xmlns + "priority",
                            sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            var document = new XDocument(root);
            return document.ToString();
        }

        private List<MenuItem> GetMenuItems()
        {
            var cacheKey = GetKeySessionKey("MainMenu");
            var menuItems = cacheService.GetOrSetCache(
                cacheKey,
                () =>
                {
                    var menu = GetCmsMenuItems() ?? new List<MenuItem>();

                    var adminMenuItems = GetAdminMenuItems();
                    if (adminMenuItems.IsNotNullOrEmpty())
                    {
                        menu.Add(
                            new MenuItem
                            {
                                Url = "#",
                                Title = Resource.Admin,
                                Items = adminMenuItems
                            });
                    }

                    if (User == null || User?.Identity?.IsAuthenticated == false)
                    {
                        this.AddToMenu(
                            menu,
                            typeof(AccountController),
                            "Login",
                            Resource.Login,
                            @class: "bttn blue");
                    }
                    else
                    {
                        this.AddToMenu(
                            menu,
                            typeof(AccountController),
                            "UserProfile",
                            Resource.Profile,
                            items: GetUserMenu());
                    }

                    return menu;
                });

            return menuItems;
        }

        private List<MenuItem> GetAdminMenuItems()
        {
            var menu = new List<MenuItem>();
            this.AddToMenu(menu, typeof(PublicationController), "Index", Resource.Publications);
            this.AddToMenu(menu, typeof(ProviderController), "Index", Resource.Providers);
            this.AddToMenu(menu, typeof(FaqController), "Index", Resource.FAQ);
            this.AddToMenu(menu, typeof(RoleController), "Index", Resource.Roles);
            this.AddToMenu(menu, typeof(UserController), "Index", Resource.Users);
            this.AddToMenu(menu, typeof(GroupController), "Index", Resource.Organizations);
            this.AddToMenu(menu, typeof(BackgroundImagesController), "Index", Resource.EditHomeImages);
            this.AddToMenu(menu, typeof(PollController), "Index", Resource.Polls);
            this.AddToMenu(menu, typeof(CmsController), "Index", Resource.Pages);
            this.AddToMenu(menu, typeof(SettingController), "UpsertUISettings", Resource.UISettings);
            this.AddToMenu(menu, typeof(TransactionHistoryController), "Index", Resource.TransactionHistoryReport);
            this.AddToMenu(menu, typeof(UserLoginController), "Index", Resource.UserLoginReport);

            return menu;
        }

        private List<MenuItem> GetUserMenu()
        {
            var menu = new List<MenuItem>();
            if (User != null)
            {
                this.AddToMenu(menu, typeof(AccountController), "ChangePassword", Resource.ChangingPassword);
                this.AddToMenu(menu, typeof(AccountController), "Logout", Resource.Exit, @class: "logout-js");
            }

            return menu;
        }

        private List<MenuItem> GetCmsMenuItems()
        {
            var cacheKey = GetKeySessionKey("MenuItems");
            return cacheService.GetOrSetCache(
                cacheKey,
                () => GetPagesTree(
                    GetVisibleCmsPageByUser()?.Where(item => item.Location == LocationType.MainMenu),
                    null));
        }

        private List<MenuItem> GetCmsHeaderMenuItems()
        {
            var cacheKey = GetKeySessionKey("HeaderMenuItems");
            return cacheService.GetOrSetCache(
                cacheKey,
                () => GetPagesTree(
                    GetVisibleCmsPageByUser()?.Where(item => item.Location == LocationType.HeaderMenu),
                    null));
        }

        private List<Page> GetVisibleCmsPageByUser()
        {
            var cacheKey = GetKeySessionKey("VisiblePages");
            return cacheService.GetOrSetCache(
                cacheKey,
                () =>
                {
                    List<Page> pages;
                    using (ContextManager.NewConnection())
                    {
                        pages = cmsService.SearchPages(new PageQueryModel());
                    }

                    var allowPages = pages
                           .Where(
                               item => item.Visibility == VisibilityType.Public ||
                                       (User?.Identity?.IsAuthenticated == true &&
                                       item.Visibility == VisibilityType.AuthenticatedUsed))
                           .ToList();

                    InitFAQMenuItems(allowPages);

                    return allowPages;
                });
        }

        private string GetKeySessionKey(string key)
        {
            return $"{key}_{CurrentCulture.TwoLetterISOLanguageName}_{User?.Id}";
        }

        private List<MenuItem> GetPagesTree(IEnumerable<Page> pages, Guid? parentId)
        {
            return pages?
                   .Where(item => item.ParentId == parentId)
                   .Select(
                       item =>
                           new MenuItem
                           {
                               Title = item.TitlesMenu?.GetValueForCurrentCulture() ?? item.Titles?.GetValueForCurrentCulture(),
                               Url = GetUrl(item.PermanentLink),
                               Items = GetPagesTree(pages, item.Id),
                               InNewWindow = item.IsInNewWindow
                           })
                   .ToList();
        }

        private void InitFAQMenuItems(List<Page> allowPages)
        {
            var faqPage = allowPages.FirstOrDefault(
                page =>
                page.PageType == PageType.Link
                && page.PermanentLink.IsNotNullOrEmpty()
                && Regex.IsMatch(page.PermanentLink, @"^\/*faq\/*$", RegexOptions.IgnoreCase));
            if (faqPage != null)
            {
                List<FaqCategory> faqCategories;
                using (ContextManager.NewConnection())
                {
                    faqCategories = faqService.SearchFaqCategories();
                }

                if (faqCategories.IsNotNullOrEmpty())
                {
                    faqPage.PermanentLink = "#";
                    foreach (var faqCategory in faqCategories.Where(x => x.HasQuestions))
                    {
                        allowPages.Add(
                            new Page
                            {
                                Id = faqCategory.Id,
                                ParentId = faqPage.Id,
                                Titles = faqCategory.Names,
                                Type = faqPage.Type,
                                VisibilityType = faqPage.VisibilityType,
                                LocationType = faqPage.LocationType,
                                IsInNewWindow = faqPage.IsInNewWindow,
                                PermanentLink = Url.DynamicAction("Faq", typeof(HomeController), new { categoryId = faqCategory.Id }, absoluteUrl: true)
                            });
                    }
                }
            }
        }

        private void InitUISettings()
        {
            var cacheKey = GetKeySessionKey("UISettings");
            ViewBag.UISettings = cacheService.GetOrSetCache(
                cacheKey,
                () =>
                {
                    using (ContextManager.NewConnection())
                    {
                        return Mapper.Map<UISettingsViewModel>(adminService.GetUISettings(false));
                    }
                });
        }
    }
}