using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bendyline.Project
{
    /// <summary>
    /// A collection of one or more projects put together, and their attendant namespaces.
    /// </summary>
    public class ProjectSpace
    {
        private List<Project> projects;

        public ProjectSpace()
        {
            this.projects = new List<Project>();
        }

        public Project GetProject(String name)
        {
            foreach (Project p in this.projects)
            {
                if (p.Name.Equals(name))
                {
                    return p;
                }
            }

            return null;
        }

        private Project EnsureProject(String name)
        {
            Project p = this.GetProject(name);

            if (p == null)
            {
                p = new Project(name);

                this.projects.Add(p);
            }

            return p;
        }

        public void ImportFromXmlDocFolder(String path)
        {
            if (!Directory.Exists(path))
            {
                throw new InvalidOperationException();
            }

            DirectoryInfo di = new DirectoryInfo(path);

            FileInfo[] files = di.GetFiles();

            foreach (FileInfo fi in files)
            {
                this.LoadFile(fi);    
            }
        }

        public void ImportFromXmlDocFile(String path)
        {
            if (!File.Exists(path))
            {
                throw new InvalidOperationException();
            }

            FileInfo fi = new FileInfo(path);

            this.LoadFile(fi);
        }

        private void LoadFile(FileInfo fi)
        {
            if (fi.Extension.ToLower() == ".xml")
            {
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open))
                {
                    using (XmlReader xr = XmlReader.Create(fs))
                    {
                        XmlDocument xd = new XmlDocument();

                        xd.Load(xr);

                        XmlNode nameNode = xd.DocumentElement.SelectSingleNode("assembly/name");

                        if (nameNode != null)
                        {
                            Project p = this.EnsureProject(nameNode.InnerText);

                            p.ProcessXmlDoc(xd);
                        }
                    }
                }
            }
        }
        public void ExportMarkdownFiles(String folderPath, String pageTemplate)
        {
            foreach (Project p in this.projects)
            {
                foreach (ProjectNamespace pn in p.Namespaces)
                {
                    pn.ExportMarkdownFile(folderPath, pageTemplate);

                    foreach (ProjectType pt in pn.Types)
                    {
                        pt.ExportMarkdownFile(folderPath, pageTemplate);
                    }
                }
            }
        }
    }
}
