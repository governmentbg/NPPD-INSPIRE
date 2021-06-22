namespace Inspire.Test.Unit.Helpers
{
    using System;
    using System.Collections.Generic;

    using Inspire.Model.Attachment;
    using Inspire.Model.Cms;
    using Inspire.Model.Helpers;
    using Inspire.Model.Language;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.Provider;
    using Inspire.Model.Publication;
    using Inspire.Model.User;
    using Inspire.Portal.Models.Faq;
    using Inspire.Portal.Models.GeoNetwork.Group;
    using Inspire.Portal.Models.GeoNetwork.User;
    using Inspire.Portal.Models.Poll;
    using Inspire.Portal.Models.Provider;
    using Inspire.Portal.Models.Publication;

    internal class DataHelper
    {
        internal static List<Guid> GetTestRoles()
        {
            return new List<Guid>
                   {
                       Guid.NewGuid(),
                       Guid.NewGuid(),
                       Guid.NewGuid()
                   };
        }

        internal static User GetTestUser()
        {
            return new User
                   {
                       Id = Guid.NewGuid(),
                       UserName = "TestUser",
                       Password = "TestPassword",
                       Status = new Nomenclature { Id = EnumHelper.GetStatusIdByEnum(UserStatus.Active) }
                   };
        }

        internal static ProviderViewModel GetProvider()
        {
            return new ProviderViewModel
                   {
                       Id = Guid.NewGuid(),
                       Names = new SortedDictionary<string, string>
                               {
                                   { EnumHelper.GetLanguageId(Language.BG).ToString(), "Име тест" },
                                   { EnumHelper.GetLanguageId(Language.EN).ToString(), "Name Test" }
                               },
                       Descriptions = new SortedDictionary<string, string>
                                      {
                                          { EnumHelper.GetLanguageId(Language.BG).ToString(), "Описание тест" },
                                          { EnumHelper.GetLanguageId(Language.EN).ToString(), "Description Test" }
                                      },
                       Links = new SortedDictionary<string, string>
                               {
                                   { EnumHelper.GetLanguageId(Language.BG).ToString(), "www.linktest.com/bg" },
                                   { EnumHelper.GetLanguageId(Language.EN).ToString(), "www.linktest.com/en" }
                               },
                       Status = new Nomenclature
                                {
                                    Id = EnumHelper.GetProviderStatus(ProviderStatus.Valid),
                                    Name = "Valid"
                                },
                       MainPicture = GetTestImage()
                   };
        }

        internal static Page GetPage()
        {
            return new Page
                   {
                       Type = new Nomenclature { Id = Guid.NewGuid() },
                       Contents = new SortedDictionary<string, string>
                                  {
                                      { EnumHelper.GetLanguageId(Language.BG).ToString(), "Тест съдържание" },
                                      { EnumHelper.GetLanguageId(Language.EN).ToString(), "Test content" }
                                  },
                       Titles = new SortedDictionary<string, string>
                                {
                                    { EnumHelper.GetLanguageId(Language.BG).ToString(), "Тест заглавие" },
                                    { EnumHelper.GetLanguageId(Language.EN).ToString(), "Test title" }
                                },
                       LocationType = new Nomenclature { Id = Guid.NewGuid() },
                       VisibilityType = new Nomenclature { Id = Guid.NewGuid() },
                       PermanentLink = "https://www.text.com",
                       Id = Guid.NewGuid()
                   };
        }

        internal static PublicationUpsertViewModel GetPublicationNews()
        {
            return new PublicationUpsertViewModel
                   {
                       Titles = new SortedDictionary<string, string>
                                {
                                    { EnumHelper.GetLanguageId(Language.BG).ToString(), "Заглавие тест" },
                                    { EnumHelper.GetLanguageId(Language.EN).ToString(), "Title Test" }
                                },
                       Contents = new SortedDictionary<string, string>
                                  {
                                      { EnumHelper.GetLanguageId(Language.BG).ToString(), "Съдържание тест" },
                                      { EnumHelper.GetLanguageId(Language.EN).ToString(), "Content Test" }
                                  },
                       Type = new Nomenclature { Id = EnumHelper.GetPublicationType(PublicationType.News) },
                       StartDate = DateTime.Now
                   };
        }

        internal static UserDTO GetTestUserDTO()
        {
            return new UserDTO
                   {
                       Id = "5",
                       Username = "TestUser",
                       Password = "TestPassword"
                   };
        }

        internal static List<UserGroup> GetTestUserGroups()
        {
            return new List<UserGroup>
                   {
                       new UserGroup(),
                       new UserGroup()
                   };
        }

        internal static FaqUpsertViewModel GetFaq()
        {
            return new FaqUpsertViewModel
                   {
                       Answers = new SortedDictionary<string, string>
                                 {
                                     { EnumHelper.GetLanguageId(Language.BG).ToString(), "Отговор тест" },
                                     { EnumHelper.GetLanguageId(Language.EN).ToString(), "Answer Test" }
                                 },
                       Questions = new SortedDictionary<string, string>
                                   {
                                       { EnumHelper.GetLanguageId(Language.BG).ToString(), "Въпрос тест" },
                                       { EnumHelper.GetLanguageId(Language.EN).ToString(), "Question Test" }
                                   },
                       Category = new Nomenclature
                                  {
                                      Id = Guid.NewGuid()
                                  }
                   };
        }

        internal static FaqCategoryViewModel GetFaqCategory()
        {
            return new FaqCategoryViewModel
                   {
                       Names = new SortedDictionary<string, string>
                               {
                                   { EnumHelper.GetLanguageId(Language.BG).ToString(), "Име тест" },
                                   { EnumHelper.GetLanguageId(Language.EN).ToString(), "Name Test" }
                               }
                   };
        }

        internal static Attachment GetTestImage()
        {
            return new Attachment
                   {
                       Description = "Image description",
                       Name = "Image name",
                       Url = "https://m.netinfo.bg/media/images/44228/44228182/991-ratio-mechka.jpg"
                   };
        }

        internal static PollViewModel GetPoll()
        {
            return new PollViewModel
                   {
                       Titles = new SortedDictionary<string, string>
                                {
                                    { EnumHelper.GetLanguageId(Language.BG).ToString(), "Име тест" },
                                    { EnumHelper.GetLanguageId(Language.EN).ToString(), "Name Test" }
                                },
                       Descriptions = new SortedDictionary<string, string>
                                      {
                                          { EnumHelper.GetLanguageId(Language.BG).ToString(), "Описание тест" },
                                          { EnumHelper.GetLanguageId(Language.EN).ToString(), "Description Test" }
                                      },
                       ValidFrom = DateTime.Now.AddDays(1),
                       ValidTo = DateTime.Now.AddDays(2),
                       Questions = new List<QuestionViewModel>
                                   {
                                       new QuestionViewModel
                                       {
                                           UniqueId = Guid.NewGuid().ToString(),
                                           Titles = new SortedDictionary<string, string>
                                                    {
                                                        {
                                                            EnumHelper.GetLanguageId(Language.BG).ToString(), "Име тест"
                                                        },
                                                        {
                                                            EnumHelper.GetLanguageId(Language.EN).ToString(),
                                                            "Name Test"
                                                        }
                                                    },
                                           QuestionDescriptions = new SortedDictionary<string, string>
                                                                  {
                                                                      {
                                                                          EnumHelper.GetLanguageId(Language.BG)
                                                                              .ToString(),
                                                                          "Име тест"
                                                                      },
                                                                      {
                                                                          EnumHelper.GetLanguageId(Language.EN)
                                                                              .ToString(),
                                                                          "Name Test"
                                                                      }
                                                                  },
                                           Options = new List<OptionViewModel>
                                                     {
                                                         new OptionViewModel
                                                         {
                                                             Values = new SortedDictionary<string, string>
                                                                      {
                                                                          {
                                                                              EnumHelper.GetLanguageId(Language.BG)
                                                                                  .ToString(),
                                                                              "Име тест"
                                                                          },
                                                                          {
                                                                              EnumHelper.GetLanguageId(Language.EN)
                                                                                  .ToString(),
                                                                              "Name Test"
                                                                          }
                                                                      }
                                                         }
                                                     }
                                       }
                                   }
                   };
        }
    }
}