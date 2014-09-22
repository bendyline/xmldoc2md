/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bendyline.Project
{
    /// <summary>
    /// Base class for a method or property.
    /// </summary>
    public class ProjectMember
    {
        private String name;
        private ProjectType projectType;
        private String summary;
        private String returns;

        public String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        public String Summary
        {
            get
            {
                return this.summary;
            }

            set
            {
                this.summary = value;
            }
        }
        public String Returns
        {
            get
            {
                return this.returns;
            }

            set
            {
                this.returns = value;
            }
        }

        public ProjectType Type
        {
            get
            {
                return this.projectType;
            }
        }

        public ProjectMember(ProjectType projectType)
        {
            this.projectType = projectType;
        }

        public void LoadFromNode(XmlNode xn)
        {
            XmlNode summaryNode = xn.SelectSingleNode("summary");

            if (summaryNode != null)
            {
                this.summary = summaryNode.InnerText;
            }

            XmlNode returnsNode = xn.SelectSingleNode("returns");

            if (returnsNode != null)
            {
                this.returns = returnsNode.InnerText;
            }
        }
    }
}
