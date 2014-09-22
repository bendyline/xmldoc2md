using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bendyline.Base;
using System.IO;
using System.Xml;

namespace Bendyline.Project
{
    /// <summary>
    /// A type within a project namespace.
    /// </summary>
    public class ProjectType
    {
        private ProjectNamespace projectNamespace;
        private Dictionary<String, ProjectMember> fields;
        private Dictionary<String, ProjectMember> properties;
        private Dictionary<String, ProjectMember> methods;
        private String name;
        private String summary;

        public ProjectNamespace Namespace
        {
            get
            {
                return this.projectNamespace;
            }
        }

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

        public ProjectType(ProjectNamespace projectNamespace)
        {
            this.projectNamespace = projectNamespace;

            this.fields = new Dictionary<string, ProjectMember>();
            this.properties = new Dictionary<string, ProjectMember>();
            this.methods = new Dictionary<string, ProjectMember>();
        }

        public ProjectMember GetMethod(String methodName)
        {
            if (this.methods.ContainsKey(methodName.ToLower()))
            {
                return this.methods[methodName.ToLower()];
            }

            return null;
        }

        public ProjectMember EnsureMethod(String methodName)
        {
            ProjectMember pm = this.GetMethod(methodName);

            if (pm == null)
            {
                pm = new ProjectMember(this);
                pm.Name = methodName;

                this.methods.Add(methodName.ToLower(), pm);
            }

            return pm;
        }

        public ProjectMember GetProperty(String propertyName)
        {
            if (this.properties.ContainsKey(propertyName.ToLower()))
            {
                return this.properties[propertyName.ToLower()];
            }

            return null;
        }

        public ProjectMember EnsureProperty(String propertyName)
        {
            ProjectMember pm = this.GetProperty(propertyName);

            if (pm == null)
            {
                pm = new ProjectMember(this);
                pm.Name = propertyName;

                this.properties.Add(propertyName.ToLower(), pm);
            }

            return pm;
        }

        public ProjectMember GetField(String fieldName)
        {
            if (this.fields.ContainsKey(fieldName.ToLower()))
            {
                return this.fields[fieldName.ToLower()];
            }

            return null;
        }

        public ProjectMember EnsureField(String fieldName)
        {
            ProjectMember pm = this.GetField(fieldName);

            if (pm == null)
            {
                pm = new ProjectMember(this);
                pm.Name = fieldName;

                this.fields.Add(fieldName.ToLower(), pm);
            }

            return pm;
        }

        public void ExportMarkdownFile(String folderPath, String pageTemplate)
        {
            StringBuilder methodList = new StringBuilder();

            if (this.methods.Values.Count > 0)
            {
                methodList.AppendLine("### Methods\r\n");

                SortedList<String, ProjectMember> sortedMembers = new SortedList<string, ProjectMember>();

                foreach (ProjectMember pm in this.methods.Values)
                {
                    sortedMembers.Add(pm.Name, pm);
                }

                foreach (ProjectMember pm in sortedMembers.Values)
                {
                    methodList.AppendLine("#### " + pm.Name);
                    methodList.AppendLine(CleanText(pm.Summary));

                    if (pm.Returns != null)
                    {
                        methodList.AppendLine("_returns: " + pm.Returns);
                    }
                }
            }

            StringBuilder propertyList = new StringBuilder();

            if (this.properties.Count > 0)
            {
                propertyList.AppendLine("### Properties\r\n");

                SortedList<String, ProjectMember> sortedMembers = new SortedList<string, ProjectMember>();

                foreach (ProjectMember pm in this.properties.Values)
                {
                    sortedMembers.Add(pm.Name, pm);
                }

                foreach (ProjectMember pm in sortedMembers.Values)
                {
                    propertyList.AppendLine("#### " + pm.Name);
                    propertyList.AppendLine(CleanText(pm.Summary));
                }
            }

            String text = String.Format(@"
# {0}
_namespace: [{1}](N{1}.md)_

{2}

{3}

{4}
", this.Name, this.Namespace.Path, CleanText(this.summary), methodList.ToString(),propertyList.ToString());

            if (pageTemplate != null)
            {
                text = pageTemplate.Replace("[content]", text);
            }

            FileUtilities.SetTextToFile(FileUtilities.EnsurePathEndsWithBackSlash(folderPath) + "T" + this.Namespace.Path + "." + this.Name + ".md", text);
        }

        public void LoadFromNode(XmlNode xn)
        {
            XmlNode summaryNode = xn.SelectSingleNode("summary");

            if (summaryNode != null)
            {
                this.summary = summaryNode.InnerText;
            }
        }


        private String CleanText(String incomingText)
        {
            if (incomingText == null)
            {
                return String.Empty;
            }

            incomingText = incomingText.Replace("\t", "").Trim();

            string results = String.Empty;
            bool lastCharWasSpace = false;
            foreach (char c in incomingText)
            {
                if (c != ' ')
                {
                    lastCharWasSpace = false;
                    results += c;
                }
                else if (!lastCharWasSpace)
                {
                    lastCharWasSpace = true;
                    results += c;
                }
            }

            return results;
        }
    }
}
