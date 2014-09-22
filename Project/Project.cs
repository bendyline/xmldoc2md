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
    /// Describes a Project, a collection of related types and namespaces.  In this case, one Project = one DLL.
    /// </summary>
    public class Project
    {
        private String name;
        private Dictionary<String, ProjectNamespace> namespaces;

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

        public IEnumerable<ProjectNamespace> Namespaces
        {
            get
            {
                return this.namespaces.Values;
            }
        }

        public Project(String name)
        {
            this.name = name;
            this.namespaces = new Dictionary<string, ProjectNamespace>();
        }

        public ProjectNamespace GetNamespace(String namespacePath)
        {
            if (this.namespaces.ContainsKey(namespacePath.ToLower()))
            {
                return this.namespaces[namespacePath.ToLower()];
            }

            return null;
        }

        public ProjectNamespace EnsureNamespace(String namespacePath)
        {
            ProjectNamespace pn = this.GetNamespace(namespacePath);

            if (pn == null)
            {
                pn = new ProjectNamespace(this);
                pn.Path = namespacePath;

                this.namespaces.Add(namespacePath.ToLower(), pn);
            }

            return pn;
        }

        internal void ProcessXmlDoc(XmlDocument document)
        {
            XmlNodeList memberNodes = document.DocumentElement.SelectNodes("members/member");

            foreach (XmlNode memberNode in memberNodes)
            {
                String memberDescription = memberNode.Attributes.GetNamedItem("name").InnerText;

                int firstSemicolon = memberDescription.IndexOf(":");

                if (firstSemicolon == 1)
                {
                    char typeChar = memberDescription[0];

                    if (typeChar == 'T')
                    {
                        String typeFullName = memberDescription.Substring(2, memberDescription.Length - 2);
                        int lastPeriod = typeFullName.LastIndexOf(".");

                        lastPeriod = typeFullName.LastIndexOf(".");

                        if (lastPeriod > 0)
                        {
                            String namespaceFullName = typeFullName.Substring(0, lastPeriod);
               
                            String typeShortName = typeFullName.Substring(lastPeriod + 1, typeFullName.Length - (lastPeriod + 1));

                            this.EnsureNamespace(namespaceFullName).EnsureType(typeShortName).LoadFromNode(memberNode);
                        }
                    }
                    else
                    {
                        String memberFullName = memberDescription.Substring(2, memberDescription.Length - 2);

                        int firstParen = memberFullName.IndexOf("(");

                        if (firstParen > 0)
                        {
                            memberFullName = memberFullName.Substring(0, firstParen);
                        }

                        int lastPeriod = memberFullName.LastIndexOf(".");

                        if (lastPeriod > 0)
                        {
                            String typeFullName = memberFullName.Substring(0, lastPeriod);

                            lastPeriod = typeFullName.LastIndexOf(".");

                            if (lastPeriod > 0)
                            {
                                String namespaceFullName = typeFullName.Substring(0, lastPeriod);

                                lastPeriod = typeFullName.LastIndexOf(".");

                                if (lastPeriod > 0)
                                {
                                    String typeShortName = typeFullName.Substring(lastPeriod + 1, typeFullName.Length - (lastPeriod + 1));


                                    lastPeriod = memberFullName.LastIndexOf(".");

                                    if (lastPeriod > 0)
                                    {
                                        String memberShortName = memberFullName.Substring(lastPeriod + 1, memberFullName.Length - (lastPeriod + 1));

                                        ProjectNamespace pn = this.EnsureNamespace(namespaceFullName);

                                        ProjectType pt = pn.EnsureType(typeShortName);

                                        if (typeChar == 'M')
                                        {
                                            pt.EnsureMethod(memberShortName).LoadFromNode(memberNode);
                                        }
                                        else if (typeChar == 'P')
                                        {
                                            pt.EnsureProperty(memberShortName).LoadFromNode(memberNode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
