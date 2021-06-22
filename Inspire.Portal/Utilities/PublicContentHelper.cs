namespace Inspire.Portal.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using HtmlAgilityPack;

    using Inspire.Portal.Models.Provider;
    using Inspire.Portal.Models.Publication;
    using Inspire.Utilities.Extensions;

    public class PublicContentHelper
    {
        public static void DecodeAndTrimContent(List<PublicationPublicViewModel> publicContents, int trimLength)
        {
            if (publicContents.IsNullOrEmpty())
            {
                return;
            }

            foreach (var publicContent in publicContents)
            {
                var htmlDoc = new HtmlDocument();
                if (publicContent.Content.IsNotNullOrEmpty())
                {
                    htmlDoc.LoadHtml(HttpUtility.HtmlDecode(publicContent.Content));

                    if (htmlDoc.DocumentNode.InnerText.Length > trimLength)
                    {
                        var iNextSpace = htmlDoc.DocumentNode.InnerText.LastIndexOf(
                            " ",
                            trimLength,
                            StringComparison.Ordinal);
                        publicContent.Content =
                            $"{htmlDoc.DocumentNode.InnerText.Substring(0, iNextSpace > 0 ? iNextSpace : trimLength).Trim()}...";
                    }
                    else
                    {
                        publicContent.Content = htmlDoc.DocumentNode.InnerText;
                    }
                }
            }
        }

        public static void DecodeAndTrimContent(List<ProviderPublicViewModel> publicContents, int trimLength)
        {
            if (publicContents.IsNullOrEmpty())
            {
                return;
            }

            foreach (var publicContent in publicContents)
            {
                var htmlDoc = new HtmlDocument();
                if (publicContent.Content.IsNotNullOrEmpty())
                {
                    htmlDoc.LoadHtml(HttpUtility.HtmlDecode(publicContent.Content));

                    if (htmlDoc.DocumentNode.InnerText.Length > trimLength)
                    {
                        var iNextSpace = htmlDoc.DocumentNode.InnerText.LastIndexOf(
                            " ",
                            trimLength,
                            StringComparison.Ordinal);
                        publicContent.Content =
                            $"{htmlDoc.DocumentNode.InnerText.Substring(0, iNextSpace > 0 ? iNextSpace : trimLength).Trim()}...";
                    }
                    else
                    {
                        publicContent.Content = htmlDoc.DocumentNode.InnerText;
                    }
                }
            }
        }
    }
}