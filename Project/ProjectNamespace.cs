/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using Bendyline.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bendyline.Project
{
    /// <summary>
    /// A namespace within a project -- typically a collection of related types.  Equates to a .net Namespace.
    /// </summary>
    public class ProjectNamespace
    {
        private Project project;
        private String path;
        private Dictionary<String, ProjectType> types;

        public String Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;
            }
        }

        public IEnumerable<ProjectType> Types
        {
            get
            {
                return this.types.Values;
            }
        }
        public ProjectNamespace(Project project)
        {
            this.project = project;
            this.types = new Dictionary<string, ProjectType>();
        }

        public ProjectType GetType(String typeName)
        {
            if (this.types.ContainsKey(typeName.ToLower()))
            {
                return this.types[typeName.ToLower()];
            }

            return null;
        }

        public ProjectType EnsureType(String typeName)
        {
            ProjectType pt = this.GetType(typeName);

            if (pt == null)
            {
                pt = new ProjectType(this);
                pt.Name = typeName;

                this.types.Add(typeName.ToLower(), pt);
            }

            return pt;
        }

        public void ExportMarkdownFile(String folderPath, String pageTemplate)
        {
            StringBuilder typeList = new StringBuilder();

            SortedList<String, ProjectType> projectTypes = new SortedList<string, ProjectType>();
           
            foreach (ProjectType pt in this.Types)
            {
                projectTypes.Add(pt.Name, pt);
            }

            foreach (ProjectType pt in projectTypes.Values)
            {
                typeList.AppendLine("[" + pt.Name + "](T" + this.Path + "." + pt.Name + ".md)");
            }

            String text = String.Format(@"
# {0}

{1}
", this.Path, typeList.ToString());

            if (pageTemplate != null)
            {
                text = pageTemplate.Replace("[content]", text);
            }

            FileUtilities.SetTextToFile(FileUtilities.EnsurePathEndsWithBackSlash(folderPath) + "N" + this.Path + ".md", text);
        }
    }
}
