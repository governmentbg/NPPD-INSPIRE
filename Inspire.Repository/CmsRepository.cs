namespace Inspire.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Inspire.Core.Infrastructure.Context;
    using Inspire.Data.Repositories;
    using Inspire.Data.Utilities;
    using Inspire.Domain.Repositories;
    using Inspire.Model.Cms;
    using Inspire.Model.Nomenclature;
    using Inspire.Model.QueryModels;
    using Inspire.Repository.Utilities;
    using Inspire.Utilities.Extensions;

    public class CmsRepository : BaseRepository, ICmsRepository
    {
        public CmsRepository(IAisContext context)
            : base(context)
        {
        }

        public List<Page> SearchPages(PageQueryModel query, Guid language)
        {
            var pages = new List<Page>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.search_page",
                new
                {
                    id = query.Id,
                    permanentLink = query.PermanentLink,
                    pagetypeid = query.TypeId,
                    pagevisibilityid = query.VisibilityTypeId,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        pages.Add(
                            new Page
                            {
                                Id = reader.GetFieldValue<Guid>("id"),
                                Type = new Nomenclature { Id = reader.GetFieldValue<Guid>("pagetypeid") },
                                VisibilityType = new Nomenclature { Id = reader.GetFieldValue<Guid>("pagevisibilityid") },
                                ParentId = reader.GetFieldValue<Guid?>("parentid"),
                                Titles = new SortedDictionary<string, string>
                                         {
                                             { language.ToString(), reader.GetFieldValue<string>("title") }
                                         },
                                TitlesMenu = new SortedDictionary<string, string>
                                         {
                                             { language.ToString(), reader.GetFieldValue<string>("titlemenu") }
                                         },
                                Order = reader.GetFieldValue<long>("custorder"),
                                PermanentLink = reader.GetFieldValue<string>("permanentlink"),
                                DbId = reader.GetFieldValue<long>("id_num"),
                                ParentDbId = reader.GetFieldValue<long?>("parentid_num"),
                                LocationType = new Nomenclature { Id = reader.GetFieldValue<Guid>("pagelocationid") },
                                IsInNewWindow = reader.GetFieldValue<bool?>("innewwindow") ?? false
                            });
                    }
                }
            }

            return pages;
        }

        public List<Page> GetParentPages(Guid pageId, Guid language)
        {
            var pages = new List<Page>();
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_pageparents",
                new
                {
                    id = pageId,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    while (reader.Read())
                    {
                        pages.Add(
                            new Page
                            {
                                Id = reader.GetFieldValue<Guid>("id"),
                                Type = new Nomenclature
                                {
                                    Id = reader.GetFieldValue<Guid?>("pagetypeid")
                                },
                                Titles = new SortedDictionary<string, string>
                                         {
                                             { language.ToString(), reader.GetFieldValue<string>("title") }
                                         },
                                PermanentLink = reader.GetFieldValue<string>("permanentlink"),
                            });
                    }
                }
            }

            return pages;
        }

        public Page GetPage(Guid id, Guid? language = null)
        {
            Page page = null;
            using (var command = Context.Connection.GenerateCommand(
                "ais.get_page",
                new
                {
                    id,
                    languageid = language
                }))
            {
                using (var reader = command.ExecuteReaderSafety())
                {
                    if (reader.Read())
                    {
                        page = new Page
                        {
                            Id = reader.GetFieldValue<Guid>("id"),
                            Type = new Nomenclature { Id = reader.GetFieldValue<Guid>("pagetypeid") },
                            VisibilityType = new Nomenclature
                            { Id = reader.GetFieldValue<Guid>("pagevisibilityid") },
                            ParentId = reader.GetFieldValue<Guid?>("parentid"),
                            CreateDate = reader.GetFieldValue<DateTime>("regdate"),
                            PermanentLink = reader.GetFieldValue<string>("permanentlink"),
                            Titles = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("title")),
                            TitlesMenu = CultureHelper.GetDictionaryData(
                                reader.GetFieldValue<Guid[]>("languageid"),
                                reader.GetFieldValue<string[]>("titlemenu")),
                            Contents = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("content")),
                            Keywords = CultureHelper.GetDictionaryData(
                                       reader.GetFieldValue<Guid[]>("languageid"),
                                       reader.GetFieldValue<string[]>("keywords")),
                            LocationType = new Nomenclature { Id = reader.GetFieldValue<Guid>("pagelocationid") },
                            IsInNewWindow = reader.GetFieldValue<bool?>("innewwindow") ?? false
                        };
                    }
                }
            }

            return page;
        }

        public void ChangePagePosition(Guid pageId, long newPosition, Guid? newParentId)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upd_pageorder",
                new
                {
                    id = pageId,
                    parentid = newParentId,
                    custorder = newPosition,
                }))
            {
                command.ExecuteNonQuerySafety();
            }
        }

        public void Upsert(Page page)
        {
            using (var command = Context.Connection.GenerateCommand(
                "ais.upsert_page",
                new
                {
                    id = page.Id,
                    pagetypeid = page.Type?.Id,
                    parentid = page.ParentId,
                    permanentlink = page.PermanentLink,
                    pagevisibilityid = page.VisibilityType?.Id,
                    innewwindow = page.IsInNewWindow,
                    languageid = page.Titles.Keys.Select(Guid.Parse).ToArray(),
                    title = page.Titles.Values,
                    titlemenu = page.TitlesMenu.Values,
                    content = page.Contents.Values,
                    keywords = page.Keywords.Values,
                    pagelocationid = page.LocationType?.Id
                }))
            {
                command.ExecuteNonQuerySafety();
                page.Id = (Guid)command.Parameters["pid"].Value;
            }
        }

        public void Delete(Guid id)
        {
            using (var command = Context.Connection.GenerateCommand("ais.del_page", new { id }))
            {
                command.ExecuteNonQuerySafety();
            }
        }
    }
}
