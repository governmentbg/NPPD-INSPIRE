namespace Inspire.Portal.Infrastructure.AutoMapperProfile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Inspire.Infrastructure.Membership;
    using Inspire.Model.Admin;
    using Inspire.Model.Cms;
    using Inspire.Model.Faq;
    using Inspire.Model.Group;
    using Inspire.Model.Helpers;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Poll;
    using Inspire.Model.Provider;
    using Inspire.Model.Publication;
    using Inspire.Model.QueryModels;
    using Inspire.Model.Role;
    using Inspire.Model.Search;
    using Inspire.Model.TableModels;
    using Inspire.Model.User;
    using Inspire.Portal.Areas.Admin.Models.Group;
    using Inspire.Portal.Areas.Admin.Models.NonPriorityMetadata;
    using Inspire.Portal.Areas.Admin.Models.PriorityMetadata;
    using Inspire.Portal.Areas.Admin.Models.Queries;
    using Inspire.Portal.Areas.Admin.Models.Role;
    using Inspire.Portal.Areas.Admin.Models.TransactionHistory;
    using Inspire.Portal.Areas.Admin.Models.User;
    using Inspire.Portal.Areas.Admin.Models.UserLogin;
    using Inspire.Portal.Models;
    using Inspire.Portal.Models.Account;
    using Inspire.Portal.Models.Cms;
    using Inspire.Portal.Models.Faq;
    using Inspire.Portal.Models.GeoNetwork.Group;
    using Inspire.Portal.Models.GeoNetwork.User;
    using Inspire.Portal.Models.Poll;
    using Inspire.Portal.Models.Provider;
    using Inspire.Portal.Models.Publication;
    using Inspire.Portal.Models.Search;
    using Inspire.Portal.Utilities;

    using TransactionHistoryQueryModel = Inspire.Portal.Areas.Admin.Models.TransactionHistory.TransactionHistoryQueryModel;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullDestinationValues = true;

            CreateMap<IUser, UserPrincipal>()
                .ForMember(
                    d => d.RoleActivities,
                    s => s.MapFrom(m => m.RoleActivities.Select(item => item.ToString()).ToList()));

            CreateMap<PublicationUpsertViewModel, Publication>().ReverseMap();
            CreateMap<PublicationUpsertViewModel, PublicationPublicViewModel>()
                .ForMember(d => d.Title, s => s.MapFrom(m => m.Titles.GetValueForCurrentCulture()))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.Contents.GetValueForCurrentCulture()));
            CreateMap<Publication, PublicationPublicViewModel>()
                .ForMember(d => d.Title, s => s.MapFrom(m => m.Titles.GetValueForCurrentCulture()))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.Contents.GetValueForCurrentCulture()))
                .ReverseMap();
            CreateMap<Publication, PublicationTableViewModel>()
                .ForMember(d => d.Type, s => s.MapFrom(m => m.Type.Name));
            CreateMap<PublicationQueryModel, PublicationQuery>();

            CreateMap<User, UserTableViewModel>()
                .ForMember(d => d.Name, s => s.MapFrom(m => m.FirstName))
                .ForMember(d => d.StatusId, s => s.MapFrom(m => m.Status.Id))
                .ForMember(d => d.Email, s => s.MapFrom(m => m.Email))
                .ForMember(d => d.Organisation, s => s.MapFrom(m => m.Organisation))
                .ForMember(d => d.Status, s => s.MapFrom(m => m.Status.Name))
                .ForMember(d => d.Roles, s => s.MapFrom(m => string.Join(", ", m.Roles)))
                .ReverseMap();

            CreateMap<UserUpsertViewModel, User>()
                .ForMember(d => d.FirstName, s => s.MapFrom(m => m.Name))
                .ForMember(d => d.LastName, s => s.MapFrom(m => m.Surname))
                .ReverseMap();

            CreateMap<UserQuery, UserQueryModel>().ReverseMap();

            CreateMap<Role, RoleTableViewModel>()
                .ForMember(
                    d => d.ActivitiesString,
                    s => s.MapFrom(m => string.Join(", ", m.Activities.Select(item => item.Name).ToList())));
            CreateMap<RoleQuery, RoleQueryModel>().ReverseMap();

            CreateMap<ChangePasswordViewModel, ChangePasswordModel>().ReverseMap();

            CreateMap<RoleUpsertViewModel, Role>().ReverseMap();
            CreateMap<Nomenclature, Activity>().ReverseMap();

            CreateMap<Role, Nomenclature>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.Id))
                .ForMember(d => d.Name, s => s.MapFrom(m => m.Name));

            CreateMap<SetRoleUpsertModel, SetRole>().ReverseMap();

            CreateMap<UISettingsModel, UISettingsViewModel>().ReverseMap();

            CreateMap<Role, Nomenclature>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.Id))
                .ForMember(d => d.Name, s => s.MapFrom(m => m.Name));

            CreateMap<SearchItem, SearchItemViewModel>().ReverseMap();

            CreateMap<Page, PageUpsertViewModel>().ReverseMap();

            CreateMap<Page, PageViewModel>()
                .ForMember(d => d.Title, s => s.MapFrom(m => m.Titles.GetValueForCurrentCulture()))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.Contents.GetValueForCurrentCulture()))
                .ForMember(d => d.Keywords, s => s.MapFrom(m => m.Keywords.GetValueForCurrentCulture()));

            CreateMap<PageUpsertViewModel, PageViewModel>()
                .ForMember(d => d.Title, s => s.MapFrom(m => m.Titles.GetValueForCurrentCulture()))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.Contents.GetValueForCurrentCulture()))
                .ForMember(d => d.Keywords, s => s.MapFrom(m => m.Keywords.GetValueForCurrentCulture()));

            CreateMap<FaqCategory, FaqCategoryViewModel>().ReverseMap();
            CreateMap<FaqQueryViewModel, FaqQueryModel>()
                .ForMember(x => x.Category, m => m.MapFrom(s => new Nomenclature { Id = s.Category }));
            CreateMap<FaqTableModel, FaqTableViewModel>()
                .ForMember(x => x.Category, m => m.MapFrom(s => s.CategoryName))
                .ForMember(x => x.Status, m => m.MapFrom(s => s.Status.Name));

            CreateMap<Faq, FaqUpsertViewModel>().ReverseMap();

            CreateMap<Faq, FaqTableViewModel>()
                .ForMember(x => x.Category, m => m.MapFrom(s => s.Category.Name))
                .ForMember(x => x.Status, m => m.MapFrom(s => s.Status.Name))
                .ForMember(x => x.Question, m => m.MapFrom(s => s.Questions.GetValueForCurrentCulture()));

            CreateMap<ProviderViewModel, Provider>()
                .ReverseMap();

            CreateMap<ProviderTableViewModel, Provider>()
                .ReverseMap();

            CreateMap<UserUpsertViewModel, UserDTO>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.GeoNetworkId.ToString()))
                .ForMember(d => d.Enabled, s => s.MapFrom(m => true))
                .ForMember(d => d.Addresses, s => s.MapFrom(m => new[] { m.GeoNetworkAddress }))
                .ForMember(d => d.EmailAddresses, s => s.MapFrom(m => new[] { m.Email }))
                .ForMember(d => d.Password, s => s.MapFrom(m => m.UniqueId))
                .ForMember(d => d.Organization, s => s.MapFrom(m => m.Group.Name))
                .ForMember(d => d.Profile, s => s.MapFrom(m => GetProfile(m)))
                .ForMember(
                    d => d.GroupsUserAdmin,
                    s => s.MapFrom(
                        m => m.Profile.Id.Equals(
                            EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.UserAdmin))
                            ? new[] { m.Group.Id.ToString() }
                            : new string[] { }))
                .ForMember(
                    d => d.GroupsReviewer,
                    s => s.MapFrom(
                        m => m.Profile.Id.Equals(
                            EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.Reviewer))
                            ? new[] { m.Group.Id.ToString() }
                            : new string[] { }))
                .ForMember(
                    d => d.GroupsEditor,
                    s => s.MapFrom(
                        m => m.Profile.Id.Equals(
                            EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.Editor))
                            ? new[] { m.Group.Id.ToString() }
                            : new string[] { }))
                .ForMember(
                    d => d.GroupsRegisteredUser,
                    s => s.MapFrom(
                        m => m.Profile.Id.Equals(
                            EnumHelper.GetProfileIdByProfile(
                                Inspire.Models.GeoNetwork.User.Profile.RegisteredUser))
                            ? new[] { m.Group.Id.ToString() }
                            : new string[] { }));

            CreateMap<UserGroup, GroupForUser>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.Group.Id))
                .ForMember(d => d.Name, s => s.MapFrom(m => m.Group.Name));

            CreateMap<Group, GroupTableViewModel>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.Id))
                .ForMember(d => d.Name, s => s.MapFrom(m => m.Names.GetValueForCurrentCulture()))
                .ForMember(d => d.Bulstat, s => s.MapFrom(m => m.Bulstat))
                .ForMember(d => d.Email, s => s.MapFrom(m => m.Email));

            CreateMap<GroupDTO, GroupUpsertModel>()
                .ForMember(
                    d => d.SelectedAllowedCategories,
                    s => s.MapFrom(m => m.AllowedCategories.Select(c => c.Id.Value)));

            CreateMap<GroupUpsertModel, GroupDTO>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.GeoNetworkId.HasValue ? m.GeoNetworkId.ToString() : "-99"))
                .ForMember(d => d.Name, s => s.MapFrom(m => m.Names.GetValueForCurrentCulture()))
                .ForMember(d => d.Description, s => s.MapFrom(m => m.Description.GetValueForCurrentCulture()))
                .ForMember(d => d.Website, s => s.MapFrom(m => m.Website.GetValueForCurrentCulture()))
                .ForMember(d => d.Label, s => s.MapFrom(m => m.Label ?? new Dictionary<string, string>()));

            CreateMap<GroupUpsertModel, Group>().ReverseMap();

            CreateMap<GroupQueryViewModel, GroupQueryModel>();

            CreateMap<GroupTableViewModel, GroupTableModel>().ReverseMap();

            CreateMap<PollViewModel, Poll>().ReverseMap();
            CreateMap<QuestionViewModel, Question>()
                .ForMember(x => x.Descriptions, m => m.MapFrom(s => s.QuestionDescriptions))
                .ReverseMap();
            CreateMap<OptionViewModel, Option>().ReverseMap();

            CreateMap<PollTableModel, PollTableViewModel>()
                .ForMember(
                    x => x.RegDate,
                    m => m.MapFrom(s => s.RegDate.HasValue ? s.RegDate.Value.ToLongDateString() : string.Empty))
                .ForMember(
                    x => x.StatusName,
                    m => m.MapFrom(s => s.Status.Name))
                .ForMember(
                    x => x.ValidFrom,
                    m => m.MapFrom(s => s.ValidFrom.HasValue ? s.ValidFrom.Value.ToLongDateString() : string.Empty))
                .ForMember(
                    x => x.ValidTo,
                    m => m.MapFrom(s => s.ValidTo.HasValue ? s.ValidTo.Value.ToLongDateString() : string.Empty));

            CreateMap<Provider, ProviderPublicViewModel>()
                .ForMember(d => d.Name, s => s.MapFrom(m => m.Names.GetValueForCurrentCulture()))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.Descriptions.GetValueForCurrentCulture()))
                .ForMember(d => d.Link, s => s.MapFrom(m => m.Links.GetValueForCurrentCulture()));

            CreateMap<PollResult, PollResultViewModel>()
                .ForMember(
                    x => x.RegDate,
                    m => m.MapFrom(s => s.RegDate.ToString()))
                .ReverseMap();

            CreateMap<PollQueryModel, PollQueryViewModel>().ReverseMap();

            CreateMap<TransactionHistoryQueryModel, Model.QueryModels.TransactionHistoryQueryModel>();
            CreateMap<Model.TableModels.TransactionHistoryTableModel, TransactionHistoryTableViewModel>();

            CreateMap<UserLoginQueryModel, UserLoginQueryViewModel>().ReverseMap();
            CreateMap<UserLoginTableModel, UserLoginTableViewModel>().ReverseMap();

            CreateMap<PriorityMetadataTableModel, PriorityMetadataTableViewModel>();
            CreateMap<NonPriorityMetadataTableModel, NonPriorityMetadataTableViewModel>();
        }

        private string GetProfile(UserUpsertViewModel model)
        {
            if (model.IsAdministrator)
            {
                return Enum.GetName(
                    typeof(Inspire.Models.GeoNetwork.User.Profile),
                    Inspire.Models.GeoNetwork.User.Profile.Administrator);
            }

            if (model.Profile.Id.Equals(
                EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.UserAdmin)))
            {
                return Enum.GetName(
                    typeof(Inspire.Models.GeoNetwork.User.Profile),
                    Inspire.Models.GeoNetwork.User.Profile.UserAdmin);
            }

            if (model.Profile.Id.Equals(
                EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.Reviewer)))
            {
                return Enum.GetName(
                    typeof(Inspire.Models.GeoNetwork.User.Profile),
                    Inspire.Models.GeoNetwork.User.Profile.Reviewer);
            }

            if (model.Profile.Id.Equals(
                EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.Editor)))
            {
                return Enum.GetName(
                    typeof(Inspire.Models.GeoNetwork.User.Profile),
                    Inspire.Models.GeoNetwork.User.Profile.Editor);
            }

            if (model.Profile.Id.Equals(
                EnumHelper.GetProfileIdByProfile(Inspire.Models.GeoNetwork.User.Profile.RegisteredUser)))
            {
                return Enum.GetName(
                    typeof(Inspire.Models.GeoNetwork.User.Profile),
                    Inspire.Models.GeoNetwork.User.Profile.RegisteredUser);
            }

            return Enum.GetName(
                typeof(Inspire.Models.GeoNetwork.User.Profile),
                Inspire.Models.GeoNetwork.User.Profile.RegisteredUser);
        }
    }
}