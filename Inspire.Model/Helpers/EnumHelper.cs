namespace Inspire.Model.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Model.Attachment;
    using Inspire.Model.Cms;
    using Inspire.Model.Email.SendMessages;
    using Inspire.Model.Faq;
    using Inspire.Model.Language;
    using Inspire.Model.Poll;
    using Inspire.Model.Provider;
    using Inspire.Model.Publication;
    using Inspire.Model.User;
    using Inspire.Models.GeoNetwork.User;
    using Inspire.Utilities.Enums;

    public static class EnumHelper
    {
        public static readonly Dictionary<ObjectType, Guid> ObjectTypes =
            new Dictionary<ObjectType, Guid>
            {
                { ObjectType.User, Guid.Parse("20b68373-24ee-42ed-8e7e-ac9d8cacc0c5") },
                { ObjectType.Publication, Guid.Parse("58887ecc-a31a-4893-a498-c327263dc816") },
                { ObjectType.Provider, Guid.Parse("17ff018c-5fce-4116-b44d-9b4cfa0ce0f8") },
                { ObjectType.HomeBackgroundImage, Guid.Parse("82dde31f-1f17-4a4a-a793-f80b87757845") }
            };

        public static readonly Dictionary<PublicationType, Guid> PublicationTypes =
            new Dictionary<PublicationType, Guid>
            {
                { PublicationType.News, Guid.Parse("6b625611-89ff-4aa5-8eed-3f4046f33f93") },
                { PublicationType.Event, Guid.Parse("d0b4929f-ad95-418a-8a32-e15fcfa36d2d") }
            };

        public static readonly Dictionary<PageType, Guid> PageTypes =
            new Dictionary<PageType, Guid>
            {
                { PageType.Link, Guid.Parse("2ed87179-2cf6-4321-bd97-88cd0528881e") },
                { PageType.Content, Guid.Parse("2f97ef8c-f664-4147-9234-44d873e5fb0c") }
            };

        public static readonly Dictionary<VisibilityType, Guid> PageVisibilityTypes =
            new Dictionary<VisibilityType, Guid>
            {
                        { VisibilityType.Hide, Guid.Parse("68f9397b-5283-464c-9245-3a6d90e1f60e") },
                        { VisibilityType.AuthenticatedUsed, Guid.Parse("46f526b1-3c88-4850-834b-e57b32a3e919") },
                        { VisibilityType.Public, Guid.Parse("f7312629-49de-4a0b-91ca-ea6523f13869") }
            };

        public static readonly Dictionary<LocationType, Guid> PageLocationTypes =
            new Dictionary<LocationType, Guid>
            {
                { LocationType.MainMenu, Guid.Parse("2e09551b-1c96-47e3-a915-e270678d3eef") },
                { LocationType.HeaderMenu, Guid.Parse("49436a7e-c743-4780-82fd-a84a9d27f144") },
                { LocationType.None, Guid.Parse("86e45f62-f4e2-4df9-92aa-14a863b641ba") }
            };

        public static readonly Dictionary<AttachmentType, Guid> AttachmentTypes =
            new Dictionary<AttachmentType, Guid>
            {
                { AttachmentType.Image, Guid.Parse("baad015d-5ad8-4fa0-8f39-8e3c470de01b") }
            };

        public static readonly Dictionary<UserStatus, Guid> UserStatuses =
            new Dictionary<UserStatus, Guid>
            {
                { UserStatus.InActive, Guid.Parse("5973b840-1794-4a78-8de0-db1a511ccfa8") },
                { UserStatus.Active, Guid.Parse("9ea414ce-aac7-41ef-9a8e-260b703c64da") },
                { UserStatus.Blocked, Guid.Parse("a1ac03cf-0686-4a84-909e-9fd03e231d8f") }
            };

        public static readonly Dictionary<SentMailMessageType, Guid> MessageTypes =
            new Dictionary<SentMailMessageType, Guid>
            {
                { SentMailMessageType.UserRegistrationMail, Guid.Parse("fa31fd10-0ac2-4df2-8c5e-6915603c26ac") },
                { SentMailMessageType.ForgottenPasswordMail, Guid.Parse("6dae9c3d-3b35-4676-b1af-ef246c8f55a8") }
            };

        public static readonly Dictionary<Language, Guid> Languages =
            new Dictionary<Language, Guid>()
            {
                { Language.BG, Guid.Parse("55b24098-c804-4c95-b7eb-f2b89e258084") },
                { Language.EN, Guid.Parse("554add5c-3ed9-4efc-ac7a-951bb6528f34") }
            };

        public static readonly Dictionary<Profile, Guid> Profiles = new Dictionary<Profile, Guid>
                                                                    {
                                                                        { Profile.Administrator, Guid.Parse("669d0463-04ad-45b9-8bd0-e168e2b53c9a") },
                                                                        { Profile.UserAdmin, Guid.Parse("d047010b-8a26-4679-bedd-c4f48bb74928") },
                                                                        { Profile.Reviewer, Guid.Parse("9801ff7c-ecdc-4ecb-bfdc-527b0ac2b8b3") },
                                                                        { Profile.Editor, Guid.Parse("02711f0c-41ae-4ffe-afeb-08677fd61b75") },
                                                                        { Profile.RegisteredUser, Guid.Parse("a56d2bf1-e26e-44f2-9ba2-e5afea90de6a") }
                                                                    };

        public static readonly Dictionary<ProviderStatus, Guid> ProviderStatuses = new Dictionary<ProviderStatus, Guid>()
                                                                    {
                                                                        { ProviderStatus.Valid, Guid.Parse("175d59cc-6d49-4cbc-ad5b-457ca5821af1") },
                                                                        { ProviderStatus.Archived, Guid.Parse("7831e14b-7fca-46a3-94b4-95acaec56c7a") }
                                                                    };

        public static readonly Dictionary<PollStatus, Guid> PollStatuses = new Dictionary<PollStatus, Guid>()
            {
                { PollStatus.New, Guid.Parse("eaf20f09-ac2b-4151-a593-1fa91d27c60c") },
                { PollStatus.Valid, Guid.Parse("4b065f51-746b-4d6d-aea6-d6c3c2a29852") },
                { PollStatus.Finished, Guid.Parse("c6bf3ce7-8ef4-408b-b519-0bf0be5b0429") }
            };

        public static readonly Dictionary<QuestionType, Guid> QuestionStatuses = new Dictionary<QuestionType, Guid>()
                                                                           {
                                                                               { QuestionType.TextBox, Guid.Parse("06e2bdce-ca2e-48f4-8859-447e3a924895") },
                                                                               { QuestionType.TextArea, Guid.Parse("cf9ef6b2-f787-439b-a421-583bac647a94") },
                                                                               { QuestionType.RadioButton, Guid.Parse("2965b711-d8db-470b-8776-48a7893a3853") },
                                                                               { QuestionType.Checkbox, Guid.Parse("a702b8c2-18b0-47cd-b3bd-b850ad6c5131") }
                                                                           };

        public static readonly Dictionary<FaqStatus, Guid> FaqStatuses = new Dictionary<FaqStatus, Guid>()
                                                                         {
                                                                             { FaqStatus.Hidden, Guid.Parse("c2bf41d5-efc8-43d7-a4e7-e5864c8a3733") },
                                                                             { FaqStatus.Public, Guid.Parse("1b73b2a7-8e14-4548-8556-b9000db97ec4") },
                                                                             { FaqStatus.Archived, Guid.Parse("12a40b96-bd74-4577-881d-1b06b71cf2ac") }
                                                                         };

        public static AttachmentType GetAttachmentTypeByTypeId(Guid typeId)
        {
            return AttachmentTypes.Any(item => item.Value == typeId)
                ? AttachmentTypes.First(item => item.Value == typeId).Key
                : AttachmentType.None;
        }

        public static Guid GetAttachmentTypeGuidByType(AttachmentType type)
        {
            return AttachmentTypes[type];
        }

        public static Guid GetObjectIdByObjectTypeId(ObjectType objectTypeId)
        {
            return ObjectTypes[objectTypeId];
        }

        public static Guid GetStatusIdByEnum(UserStatus status)
        {
            return UserStatuses[status];
        }

        public static Guid GetMessageTypeIdByEnum(SentMailMessageType messageType)
        {
            return MessageTypes[messageType];
        }

        public static PublicationType GetPublicationTypeById(Guid type)
        {
            return PublicationTypes.Any(item => item.Value == type)
                ? PublicationTypes.First(item => item.Value == type).Key
                : PublicationType.None;
        }

        public static PageType GetPageTypeById(Guid type)
        {
            return PageTypes.Any(item => item.Value == type)
                ? PageTypes.First(item => item.Value == type).Key
                : PageType.None;
        }

        public static Guid? GetPageTypeIdByType(PageType type)
        {
            return PageTypes.Any(item => item.Key == type)
                ? PageTypes.First(item => item.Key == type).Value
                : default(Guid?);
        }

        public static VisibilityType GetPageVisibilityTypeById(Guid type)
        {
            return PageVisibilityTypes.Any(item => item.Value == type)
                ? PageVisibilityTypes.First(item => item.Value == type).Key
                : VisibilityType.Hide;
        }

        public static Guid? GetPageVisibilityTypeId(VisibilityType type)
        {
            return PageVisibilityTypes.Any(item => item.Key == type)
                ? PageVisibilityTypes.First(item => item.Key == type).Value
                : default(Guid?);
        }

        public static LocationType GetPageLocationTypeById(Guid type)
        {
            return PageLocationTypes.Any(item => item.Value == type)
                ? PageLocationTypes.First(item => item.Value == type).Key
                : LocationType.None;
        }

        public static Guid? GetPageLocationByType(LocationType type)
        {
            return PageLocationTypes.Any(item => item.Key == type)
                ? PageLocationTypes.First(item => item.Key == type).Value
                : default(Guid?);
        }

        public static Guid? GetProfileIdByProfile(Profile profile)
        {
            return Profiles.Any(item => item.Key == profile)
                ? Profiles.First(item => item.Key == profile).Value
                : default(Guid?);
        }

        public static Profile GetProfileById(Guid type)
        {
            return Profiles.Any(item => item.Value == type)
                ? Profiles.First(item => item.Value == type).Key
                : Profile.Guest;
        }

        public static Guid GetProviderStatus(ProviderStatus status)
        {
            return ProviderStatuses[status];
        }

        public static Guid GetLanguageId(Language lang)
        {
            return Languages[lang];
    }

        public static Guid GetPublicationType(PublicationType type)
        {
            return PublicationTypes[type];
        }

        public static Guid? GetFaqStatus(FaqStatus type)
        {
            return FaqStatuses[type];
        }
    }
}