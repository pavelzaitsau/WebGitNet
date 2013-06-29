﻿//-----------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    public class ObjectInfo
    {
        private readonly ObjectType objectType;
        private readonly string hash;
        private readonly int? size;
        private readonly string name;

        public ObjectInfo(ObjectType objectType, string hash, int? size, string name)
        {
            this.objectType = objectType;
            this.hash = hash;
            this.size = size;
            this.name = name;
        }

        public ObjectType ObjectType
        {
            get { return this.objectType; }
        }

        public string Hash
        {
            get { return this.hash; }
        }

        public int? Size
        {
            get { return this.size; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
