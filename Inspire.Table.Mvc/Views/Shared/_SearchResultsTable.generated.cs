﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    
    #line 1 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    using System.Reflection;
    
    #line default
    #line hidden
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 2 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    using Inspire.Common.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    using Inspire.Core.Infrastructure.Attribute;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    using Inspire.Table.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    using Inspire.Utilities.Extensions;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    using Kendo.Mvc.UI;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_SearchResultsTable.cshtml")]
    public partial class _Views_Shared__SearchResultsTable_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Views_Shared__SearchResultsTable_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 7 "..\..\Views\Shared\_SearchResultsTable.cshtml"
  
    List<Tuple<TableOptionsAttribute, Type, PropertyInfo>> tableColumns = SearchTableHelpers.GetPropertiesTableData(Model);
    var serverOperation = ViewBag.ServerOperation is bool && (bool)ViewBag.ServerOperation;
    var selectable = ViewBag.Selectable is bool && (bool)ViewBag.Selectable;
    var searchQueryId = ViewBag.SearchQueryId as string;
    var groupableColumns = ViewBag.GroupableColumns as List<string>;
    var groupable = ViewBag.Groupable is bool && (bool)ViewBag.Groupable || groupableColumns.IsNotNullOrEmpty();
    var controller = ViewContext.Controller;
    var controllerType = controller.GetType();
    var controllerName = controller.GetControllerName();
    var area = CustomHelpers.GetControllerArea(controllerType);
    var gridId = Guid.NewGuid().ToString();
    var data = Model as IEnumerable<dynamic>;
    var isSortable = ViewBag.IsSortable as bool?;
    var jsController = controllerName.IsNotNullOrEmpty() ? char.ToLower(controllerName[0]) + controllerName.Substring(1) : "";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<script>\r\n\r\n    if (typeof siteBase === \'undefined\' || !siteBase) {\r\n        " +
"window.siteBase = \'");

            
            #line 27 "..\..\Views\Shared\_SearchResultsTable.cshtml"
                      Write(VirtualPathUtility.ToAbsolute("~/"));

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    }\r\n\r\n    if (typeof onGridDataBound !== \'function\') {\r\n        window.onG" +
"ridDataBound =\r\n            function(e) {\r\n                var grid = e.sender;\r" +
"\n                if (grid.options.selectable === true && grid.dataSource.total()" +
" > 0) {\r\n                    grid.select(e.sender.tbody.find(\'tr:first\')); //сел" +
"ектиране на 1вия ред в таблицата след рендване\r\n                }\r\n            }" +
";\r\n    }\r\n\r\n    var templates = [];\r\n    if (typeof getClientTemplateById != \'fu" +
"nction\') {\r\n        window.getClientTemplateById = function(templateId, item) {\r" +
"\n            var isCached = templateId in templates;\r\n            var template =" +
" isCached ? templates[templateId] : null;\r\n            if (isCached === false) {" +
"\r\n                var templateElement = templateId ? $(\"#\" + templateId) : null;" +
"\r\n                if (templateElement) {\r\n                    var templateHtml =" +
" templateElement.html();\r\n                    if (templateHtml) {\r\n             " +
"           template = kendo.template(templateHtml); // Render template\r\n        " +
"                templates[templateId] = template;\r\n                    }\r\n      " +
"          }\r\n            }\r\n\r\n            return template ? template(item) : \"\";" +
"\r\n        };\r\n    }\r\n\r\n    if (typeof bindGridExportEvents != \'function\') {\r\n   " +
"     window.bindGridExportEvents = function(gridSelector) {\r\n\r\n            var e" +
"xportSelector = kendo.format(\"{0} .export-js\", gridSelector);\r\n\r\n            $(d" +
"ocument).off(\"click\", exportSelector);\r\n            $(document).on(\"click\",\r\n   " +
"             exportSelector,\r\n                function(e) {\r\n                   " +
" e.preventDefault();\r\n                    var sender = $(e.currentTarget);\r\n\r\n  " +
"                  var grid = $(gridSelector).data(\"kendoGrid\");\r\n               " +
"     if (grid.dataSource.view().length < 1) {\r\n                        if (notif" +
"ication && resources) {\r\n                            notification.displayMessage" +
"(resources.NoDataFound, \"warning\");\r\n                        }\r\n\r\n              " +
"          return;\r\n                    }\r\n\r\n                    var controller =" +
" sender.data(\"controller\");\r\n                    var action = sender.data(\"actio" +
"n\");\r\n                    var area = sender.data(\"area\");\r\n                    v" +
"ar type = sender.data(\"type\");\r\n                    var searchQueryId = grid.ele" +
"ment.data(\"searchqueryid\");\r\n\r\n                    // ask the parameterMap to cr" +
"eate the request object for you\r\n                    var requestObject = (new ke" +
"ndo.data.transports[\"aspnetmvc-server\"]({ prefix: \"\" }))\r\n                      " +
"  .options.parameterMap({\r\n                            page: grid.dataSource.pag" +
"e(),\r\n                            sort: grid.dataSource.sort(),\r\n               " +
"             filter: grid.dataSource.filter(),\r\n                            grou" +
"p: grid.dataSource.group()\r\n                        });\r\n\r\n                    v" +
"ar params = $.param({\r\n                        searchqueryid: searchQueryId,\r\n  " +
"                      type: type,\r\n                        pageSize: grid.dataSo" +
"urce._pageSize,\r\n                        page: requestObject.page || \'1\',\r\n     " +
"                   sort: requestObject.sort || \'~\',\r\n                        fil" +
"ter: requestObject.filter || \'~\',\r\n                        group: requestObject." +
"group || \'~\'\r\n                    });\r\n\r\n                    var areaData = \"\";\r" +
"\n                    if (area) {\r\n                        areaData = area + \"/\";" +
"\r\n                    }\r\n\r\n                    var url = siteBase + areaData + c" +
"ontroller + \"/\" + action;\r\n                    if (core !== \"undefined\" && core." +
"getPathToActionMethod !== \"undefined\") {\r\n                        url = core.get" +
"PathToActionMethod(action, controller, { area: area });\r\n                    }\r\n" +
"\r\n                    window.location.href = url + \"?\" + params;\r\n              " +
"  });\r\n        };\r\n    }\r\n\r\n    if (typeof dateTimePickerFilter != \'function\') {" +
"\r\n        window.dateTimePickerFilter = function(args) {\r\n            var elemen" +
"t = null;\r\n            if (args[\"length\"]) {\r\n                element = args.len" +
"gth > 0 ? args[0] : null;\r\n            } else if (args[\"element\"]) {\r\n          " +
"      element = args[\"element\"];\r\n            }\r\n\r\n            if (element) {\r\n " +
"               $(element).kendoDateTimePicker();\r\n            }\r\n        };\r\n   " +
" }\r\n\r\n    if (typeof datePickerFilter != \'function\') {\r\n        window.datePicke" +
"rFilter = function(args) {\r\n            var element = null;\r\n            if (arg" +
"s[\"length\"]) {\r\n                element = args.length > 0 ? args[0] : null;\r\n   " +
"         } else if (args[\"element\"]) {\r\n                element = args[\"element\"" +
"];\r\n            }\r\n\r\n            if (element) {\r\n                $(element).kend" +
"oDatePicker();\r\n            }\r\n        };\r\n    }\r\n\r\n    if (typeof getSearchResu" +
"ltGrid != \'function\') {\r\n        window.getSearchResultGrid = function(gridId) {" +
"\r\n            return $(\"#\" + gridId).data(\"kendoGrid\");\r\n        };\r\n    }\r\n\r\n  " +
"  function placeholderHandler(element) {\r\n        return element.clone().addClas" +
"s(\"k-state-hover\").css(\"opacity\", 0.65);\r\n    }\r\n</script>\r\n\r\n<div");

WriteLiteral(" class=\"tableWrapper\"");

WriteLiteral(">\r\n");

            
            #line 163 "..\..\Views\Shared\_SearchResultsTable.cshtml"
    
            
            #line default
            #line hidden
            
            #line 163 "..\..\Views\Shared\_SearchResultsTable.cshtml"
      
        var template = string.Empty;
        if (data != null)
        {
            var genericArgument = data.GetType().GetGenericArguments()[0];
            var dnAttribute = genericArgument.GetCustomAttributes(typeof(ClientTemplateAttribute), true).FirstOrDefault();
            if (dnAttribute != null)
            {
                template = genericArgument.GetAttributeValue((ClientTemplateAttribute ct) => ct.Name) ?? "template";
            }
        }

        
            
            #line default
            #line hidden
            
            #line 175 "..\..\Views\Shared\_SearchResultsTable.cshtml"
   Write(Html.DynamicAction("ClientTemplate", controller.GetType()));

            
            #line default
            #line hidden
            
            #line 175 "..\..\Views\Shared\_SearchResultsTable.cshtml"
                                                                   

        var grid = Html.Kendo().Grid(data)
                       .Name(gridId)
                       .Groupable(g => g.Enabled(groupable))
                       .ToolBar(toolbar => toolbar.Template(
                           
            
            #line default
            #line hidden
item => new System.Web.WebPages.HelperResult(__razor_template_writer => {

WriteLiteralTo(__razor_template_writer, "\r\n                               <div");

WriteLiteralTo(__razor_template_writer, " class=\"options left text-left\"");

WriteLiteralTo(__razor_template_writer, ">\r\n");

WriteLiteralTo(__razor_template_writer, "                                   ");

            
            #line 183 "..\..\Views\Shared\_SearchResultsTable.cshtml"
   WriteTo(__razor_template_writer, Html.DynamicAction("TableControls", controller.GetType()));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n                               </div>\r\n                               <div");

WriteLiteralTo(__razor_template_writer, " class=\"options right text-right\"");

WriteLiteralTo(__razor_template_writer, ">\r\n                                   <span");

WriteLiteralTo(__razor_template_writer, " class=\"export\"");

WriteLiteralTo(__razor_template_writer, ">\r\n");

WriteLiteralTo(__razor_template_writer, "                                       ");

            
            #line 187 "..\..\Views\Shared\_SearchResultsTable.cshtml"
       WriteTo(__razor_template_writer, Html.ExportLinks());

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n");

WriteLiteralTo(__razor_template_writer, "                                       ");

            
            #line 188 "..\..\Views\Shared\_SearchResultsTable.cshtml"
       WriteTo(__razor_template_writer, Html.DynamicAction("TableEndControls", controller.GetType()));

            
            #line default
            #line hidden
WriteLiteralTo(__razor_template_writer, "\r\n                                   </span>\r\n                               </di" +
"v>\r\n                            ");

})
            
            #line 191 "..\..\Views\Shared\_SearchResultsTable.cshtml"
                                   ))
                       .HtmlAttributes(new { data_searchQueryId = searchQueryId, data_controller = controllerName, data_area = area, @class = "overwrite-table" })
                       .Columns(columns =>
                       {
                           foreach (var column in tableColumns.Where(c => !c.Item1.Ignore))
                           {
                               // Get client template and init dateTime formater if it is empty
                               var clientTemplate = SearchTableHelpers.GetPropertyClientTemplate(column);
                               var isDateTime = column.Item2 == typeof(DateTime) || column.Item2 == typeof(DateTime?);
                               var columnName = column.Item1.Name;
                               columns
                                   .Bound(columnName)
                                   .Title(column.Item1.Title)
                                   .Hidden(column.Item1.IsHidden)
                                   .HtmlAttributes(new { @class = column.Item1.CssClass ?? string.Empty })
                                   .Format(column.Item1.Format.IsNotNullOrEmpty() ? column.Item1.Format.StartsWith("{0") ? column.Item1.Format : "{0:" + column.Item1.Format + "}" : null)
                                   .ClientTemplate(clientTemplate)
                                   .Sortable(!column.Item1.IsCheckbox && !column.Item1.DisableFilterable)
                                   .HeaderHtmlAttributes(new { @class = column.Item1.HeaderClass })
                                   .HeaderTemplate(column.Item1.IsCheckbox ? "<input type='checkbox' class='checkAll-js' />" : null)
                                   .ClientFooterTemplate(column.Item1.IsSummable ? "#= sum #" : null)
                                   .Width(column.Item1.Width > 0 ? column.Item1.Width : column.Item1.IsCheckbox ? 28 : 0)
                                   //// .Groupable(column.Item1.IsGroupable)
                                   .ClientGroupHeaderTemplate($"<span class='{column.Item1.CssClass}'>#: value != null ? value : '' #</span>")
                                   .Filterable(f => f

                                                   // Expression with index is not supported for filtering as it may not be translated by the QueryableProviders - https://www.telerik.com/forums/filtering-is-broken-for-some-values-again
                                                   .Enabled(!column.Item1.IsCheckbox && !column.Item1.DisableFilterable && column.Item1.Key.IsNullOrEmpty())
                                                   .UI(isDateTime
                                                       ? column.Item1.Format?.Equals("g", StringComparison.InvariantCultureIgnoreCase) == true ? "dateTimePickerFilter" : "datePickerFilter"
                                                       : string.Empty)
                                                   .Cell(cell => cell
                                                             .ShowOperators(!column.Item1.IsCheckbox && column.Item1.DisableFilterable)
                                                             .Operator(column.Item2 == typeof(string) ? "contains" : string.Empty)
                                                             .Template(isDateTime
                                                                 ? column.Item1.Format?.Equals("g", StringComparison.InvariantCultureIgnoreCase) == true ? "dateTimePickerFilter" : "datePickerFilter"
                                                                 : string.Empty)));
                           }
                       })
                       .Sortable()
                       .Resizable(r => r.Columns(true))
                       .Reorderable(r => r.Columns(true))
                       .Filterable(f => f.Mode(GridFilterMode.Menu))
                       .Selectable(s => s.Enabled(selectable).Mode(GridSelectionMode.Single).Type(GridSelectionType.Row))
                       .Events(e => e.DataBound("onGridDataBound"))
                       .DataSource(d => d.Ajax()
                                         .ServerOperation(serverOperation)
                                         .PageSize(20)
                                         .Read("Grid_ReadData", controllerName, new
                                                                                {
                                                                                    Area = area,
                                                                                    searchQueryId
                                                                                })
                                         .Model(m =>
                                         {
                                             foreach (var column in tableColumns.Where(item => item.Item1.Key.IsNullOrEmpty()))
                                             {
                                                 m.Field(column.Item1.Name, column.Item2);
                                             }
                                         })
                                         .Aggregates(aggregates =>
                                         {
                                             foreach (var column in tableColumns.Where(item => item.Item1.IsSummable))
                                             {
                                                 aggregates.Add(column.Item1.Name, column.Item2).Sum();
                                             }
                                         })
                                         .Group(g =>
                                         {
                                             if (groupable)
                                             {
                                                 if (groupableColumns.IsNotNullOrEmpty())
                                                 {
                                                     foreach (var group in groupableColumns)
                                                     {
                                                         g.Add<string>(group);
                                                     }
                                                 }
                                                 else
                                                 {
                                                     foreach (var column in tableColumns.Where(c => c.Item1.IsGroupable))
                                                     {
                                                         g.Add<string>(column.Item3.Name);
                                                     }
                                                 }
                                             }
                                         }))
                       .Pageable(p => p.Enabled(true).PageSizes(new List<object> { 10, 20, 50, 100, "all" }).PreviousNext(true).ButtonCount(5));

        if (!string.IsNullOrWhiteSpace(template))
        {
            grid.ClientDetailTemplateId(template);
        }

        
            
            #line default
            #line hidden
            
            #line 285 "..\..\Views\Shared\_SearchResultsTable.cshtml"
   Write(grid);

            
            #line default
            #line hidden
            
            #line 285 "..\..\Views\Shared\_SearchResultsTable.cshtml"
             

        if (isSortable.HasValue && isSortable.Value)
        {
            
            
            #line default
            #line hidden
            
            #line 289 "..\..\Views\Shared\_SearchResultsTable.cshtml"
        Write(Html.Kendo().Sortable()
                  .For($"#{gridId}")
                  .Filter("table > tbody > tr")
                  .Events(s => s.Change($"{jsController}.onChangeOrder"))
                  .Cursor("move")
                  .HintHandler("$.noop")
                  .PlaceholderHandler("placeholderHandler")
                  .ContainerSelector($"#{gridId} tbody"));

            
            #line default
            #line hidden
            
            #line 296 "..\..\Views\Shared\_SearchResultsTable.cshtml"
                                                         
        }
    
            
            #line default
            #line hidden
WriteLiteral("\r\n</div>\r\n\r\n<script>\r\n    $(document).ready(function() {\r\n        if (core !== \"u" +
"ndefined\" && core.persistGridPageSize !== \"undefined\") {\r\n            core.persi" +
"stGridPageSize(\"#");

            
            #line 304 "..\..\Views\Shared\_SearchResultsTable.cshtml"
                                   Write(gridId);

            
            #line default
            #line hidden
WriteLiteral("\");\r\n        }\r\n\r\n        bindGridExportEvents(\"#");

            
            #line 307 "..\..\Views\Shared\_SearchResultsTable.cshtml"
                           Write(gridId);

            
            #line default
            #line hidden
WriteLiteral("\");\r\n    });\r\n</script>\r\n\r\n");

            
            #line 311 "..\..\Views\Shared\_SearchResultsTable.cshtml"
Write(Html.DynamicAction("RenderCustomGridScripts", controller.GetType(), new { gridId }));

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591