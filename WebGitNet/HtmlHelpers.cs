﻿//-----------------------------------------------------------------------
// <copyright file="HtmlHelpers.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace WebGitNet
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class HtmlHelpers
    {
        public static MvcHtmlString Markdown(this HtmlHelper html, string markdown)
        {
            markdown = new Regex("#([0-9]+)").Replace(markdown, "<a href='http://netologic.by:8888/tfs/web/UI/Pages/WorkItems/WorkItemEdit.aspx?id=$1' target='_blank'>#$1</a>");
            return new MvcHtmlString(markdown);
        }

        public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string name, int size = 72)
        {
            var imgUrl = string.Format(
                "https://secure.gravatar.com/avatar/{0}.png?s={1}&r=pg",
                HashString(email),
                size);

            return new MvcHtmlString(string.Format("<img alt=\"\" width=\"{0}\" height=\"{0}\" title=\"{1}\" src=\"{2}\" />", size, html.AttributeEncode(name), html.AttributeEncode(imgUrl)));
        }

        public static IEnumerable<Route> FindSatisfiableRoutes(this HtmlHelper html, object routeData = null)
        {
            var routes = html.RouteCollection;
            var current = html.ViewContext.RequestContext.RouteData.Route;
            var request = html.ViewContext.RequestContext;
            var routeValues = routeData == null ? html.ViewContext.RouteData.Values : new RouteValueDictionary(routeData);

            return from route in routes.OfType<Route>()
                   where route != current
                   let name = route.GetName()
                   where name != null
                   let routed = route.GetVirtualPath(request, routeValues)
                   where routed != null
                   orderby name
                   select route;
        }

        public static string GetName(this Route route)
        {
            if (route == null || route.Defaults == null)
            {
                return null;
            }

            var routeName = route.Defaults["routeName"];

            if (routeName == null)
            {
                return null;
            }

            return routeName.ToString();
        }

        public static string GetIcon(this Route route)
        {
            if (route == null || route.Defaults == null)
            {
                return null;
            }

            var routeIcon = route.Defaults["routeIcon"];

            if (routeIcon == null)
            {
                return null;
            }

            return routeIcon.ToString();
        }

        private static string HashString(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value.Trim().ToLowerInvariant());
            using (var md5 = new MD5CryptoServiceProvider())
            {
                data = md5.ComputeHash(data);
            }

            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                ret.Append(data[i].ToString("x2").ToLowerInvariant());
            }

            return ret.ToString();
        }
    }
}