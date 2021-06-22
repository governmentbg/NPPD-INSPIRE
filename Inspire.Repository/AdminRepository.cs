namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Admin;
    using Inspire.Model.QueryModels;
    using Inspire.Model.TableModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class AdminRepository : BaseRepository, IAdminRepository
    {
        public AdminRepository(IAisContext context)
            : base(context)
        {
        }

        public void UpsertUISetting(UISettingsModel model)
        {
            using (var command = Context.Connection.GenerateCommand(
                "admdata.upd_nfooter",
                null,
                new Dictionary<string, object>
                {
                    { "paddress", model.Address.Values.ToArray() },
                    { "pphone", model.Phone },
                    { "pworkingtime", model.WorkingTime.Values.ToArray() },
                    { "pemail", model.Email },
                    { "psignalstext", model.Explanation.Values.ToArray() },
                    { "pfblink", model.FbLink },
                    { "pytlink", model.LinkedInLink },
                    { "pportalname", model.PortalName.Values.ToArray() },
                    { "pportalsubname", model.PortalSubName.Values.ToArray() },
                    { "pportalrights", model.PortalRights.Values.ToArray() },
                    { "planguageid", model.Address.Keys.Select(Guid.Parse).ToArray() }
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public UISettingsModel GetUISettings(Guid? languageId)
        {
            UISettingsModel data = null;
            using (var command = Context.Connection.GenerateCommand(
                "admdata.get_nfooter",
                new
                {
                    languageId
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    if (reader.Read())
                    {
                        data = new UISettingsModel
                        {
                            Id = reader.GetFieldValue<Guid?>("id"),
                            Address = new SortedDictionary<string, string>(),
                            Phone = reader.GetFieldValue<string>("phone"),
                            WorkingTime = new SortedDictionary<string, string>(),
                            Email = reader.GetFieldValue<string>("email"),
                            Explanation = new SortedDictionary<string, string>(),
                            PortalName = new SortedDictionary<string, string>(),
                            PortalSubName = new SortedDictionary<string, string>(),
                            PortalRights = new SortedDictionary<string, string>(),
                            FbLink = reader.GetFieldValue<string>("fblink"),
                            LinkedInLink = reader.GetFieldValue<string>("ytlink")
                        };

                        var languages = reader.GetFieldValue<Guid[]>("languageid");
                        var addresses = reader.GetFieldValue<string[]>("address");
                        var workingTimes = reader.GetFieldValue<string[]>("workingtime");
                        var signalsText = reader.GetFieldValue<string[]>("signalstext");
                        var portalNamesText = reader.GetFieldValue<string[]>("portalname");
                        var portalSubNamesText = reader.GetFieldValue<string[]>("portalsubname");
                        var portalRightsText = reader.GetFieldValue<string[]>("portalrights");

                        for (var i = 0; i < languages.Length; i++)
                        {
                            var language = languages[i].ToString();

                            data.Address.Add(language, addresses[i]);
                            data.WorkingTime.Add(language, workingTimes[i]);
                            data.Explanation.Add(language, signalsText[i]);
                            data.PortalName.Add(language, portalNamesText[i]);
                            data.PortalSubName.Add(language, portalSubNamesText[i]);
                            data.PortalRights.Add(language, portalRightsText[i]);
                        }
                    }
                }
            }

            return data;
        }

        public List<UserLoginTableModel> SearchUserLogin(UserLoginQueryModel query)
        {
            var result = new List<UserLoginTableModel>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.search_userlogin",
                new
                {
                    isfromgn = query.IsFromGN,
                    username = query.Username?.ExtendToSearch(),
                    logindatefrom = query.LoginDateFrom,
                    logindateto = query.LoginDateTo,
                    loginip = query.LoginIp?.ExtendToSearch()
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        result.Add(
                            new UserLoginTableModel
                            {
                                Username = reader.GetFieldValue<string>("username"),
                                LoginDate = reader.GetFieldValue<DateTime?>("logindate"),
                                LoginIp = reader.GetFieldValue<string>("loginip"),
                            });
                    }
                }
            }

            return result;
        }
    }
}