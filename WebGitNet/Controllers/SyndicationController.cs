﻿//-----------------------------------------------------------------------
// <copyright file="SyndicationController.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MarkdownSharp;
    using WebGitNet.ActionResults;

    public class SyndicationController : SharedControllerBase
    {
        public ActionResult Commits(string repo)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            var items = from entry in GitUtilities.GetLogEntries(resourceInfo.FullPath, 20)
                        select FormatLogEntry(entry, repo);

            var feed = BuildFeed(repo + " Commits", items);

            return new AtomActionResult(feed);
        }

        public ActionResult LastCommits()
        {
            var directory = this.FileManager.DirectoryInfo;

            var items = from repo in directory.EnumerateDirectories()
                        from entry in GitUtilities.GetLogEntries(repo.FullName, 1)
                        select FormatLogEntry(entry, repo.Name);

            var feed = BuildFeed("Last Commit in All Repos", items);

            return new AtomActionResult(feed);
        }

        private SyndicationFeed BuildFeed(string title, IEnumerable<SyndicationItem> items)
        {
            var feed = new SyndicationFeed(items);
            feed.BaseUri = new Uri(Request.Url, Url.Content("~/"));
            feed.ImageUrl = new Uri(Url.Content("~/favicon.ico"), UriKind.Relative);
            feed.Title = new TextSyndicationContent(title);

            return feed;
        }

        private SyndicationItem FormatLogEntry(LogEntry entry, string repo)
        {
            var markdownParser = new Markdown(true);
            var item = new SyndicationItem();

            item.Id = entry.CommitHash;
            item.Title = new TextSyndicationContent(entry.Subject);
            item.Content = SyndicationContent.CreateHtmlContent(markdownParser.Transform(entry.Subject + "\n\n" + entry.Body));
            item.LastUpdatedTime = entry.AuthorDate;
            item.PublishDate = entry.CommitterDate;

            item.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(Url.Action("ViewCommit", "Browse", new { repo, @object = entry.CommitHash }), UriKind.Relative)));

            item.Categories.Add(new SyndicationCategory("commit"));
            if (entry.Parents.Count > 1)
            {
                item.Categories.Add(new SyndicationCategory("merge"));
            }

            item.Authors.Add(new SyndicationPerson(entry.AuthorEmail, entry.Author, null));
            if (entry.Author != entry.Committer || entry.AuthorEmail != entry.CommitterEmail)
            {
                item.Contributors.Add(new SyndicationPerson(entry.CommitterEmail, entry.Committer, null));
            }

            return item;
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "Repo Commit Syndication",
                    "feeds/{repo}/commits",
                    new { controller = "Syndication", action = "Commits" });

                routes.MapRoute(
                    "All Repo Last Commits Syndication",
                    "feeds/last",
                    new { controller = "Syndication", action = "LastCommits" });
            }
        }
    }
}
