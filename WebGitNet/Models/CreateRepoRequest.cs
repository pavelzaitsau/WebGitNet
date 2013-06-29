﻿//-----------------------------------------------------------------------
// <copyright file="CreateRepoRequest.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateRepoRequest
    {
        [Required]
        public string RepoName { get; set; }

        public string Description { get; set; }
    }
}
