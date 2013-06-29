﻿//-----------------------------------------------------------------------
// <copyright file="ImpactWeek.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System;
    using System.Collections.Generic;

    public class ImpactWeek
    {
        public DateTime Week { get; set; }

        public List<UserImpact> Impacts { get; set; }
    }
}
