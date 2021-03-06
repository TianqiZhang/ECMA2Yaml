﻿using IntellisenseFileGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiffXML
{
    class Program
    {
        static void Main(string[] args)
        {
            var opt = new CommandLineOptions();
            if (opt.Parse(args))
            {
                OrderXML(opt.InFolder, opt.OutFolder);
            }
        }

        static void OrderXML(string inFolder, string outPutFolder)
        {
            var needOrderFiles = GetFiles(inFolder, "*.xml");
            ParallelOptions opt = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

            if (needOrderFiles != null)
            {
                Parallel.ForEach(needOrderFiles, opt, xfile =>
                {
                    XDocument fxDoc = XDocument.Load(xfile.FullName);
                    var membersEle = fxDoc.Root.Element("members");
                    var memberEles = membersEle?.Elements("member");
                    if (memberEles != null)
                    {
                        var orderedList = memberEles.OrderBy(member => member.Attribute("name").Value).ToList();
                        orderedList.ForEach(m =>
                        {
                            var child = m.Elements().ToList();
                            if (child != null && child.Count() > 1)
                            {
                                m.RemoveNodes();
                                m.Add(child.OrderBy(c => c.Attribute("name")?.Value).OrderBy(c => c.Name.LocalName));
                            }
                        });
                        membersEle.RemoveAll();
                        membersEle.Add(orderedList);

                        orderedList.ToList().ForEach(p =>
                        {
                            //if (p.Attribute("name").Value == "M:System.IO.Pipelines.FlushResult.#ctor(System.Boolean,System.Boolean)")
                            //{
                            SpecialProcessElement(p);

                            //if (IsEmpty(p))
                            //{
                            //    p.Remove();
                            //}
                            //}
                        });

                        string directoryName = xfile.DirectoryName.Replace(inFolder, outPutFolder);
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        fxDoc.Save(xfile.FullName.Replace(inFolder, outPutFolder));
                    }
                });
            }
            WriteLine(outPutFolder);
            WriteLine("done.");
        }

        private static void SpecialProcessElement(XElement ele)
        {
            if (ele != null)
            {
                var child = ele.Nodes();
                if (child != null && child.Count() > 0)
                {
                    List<XNode> toBeAddEles = new List<XNode>();
                    foreach (var e in child)
                    {
                        if (e.NodeType == System.Xml.XmlNodeType.Text)
                        {
                            SpecialProcessText((e as XText));
                        }
                        else if (e.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            SpecialProcessElement(e as XElement);
                        }
                    }
                }
            }
        }

        public static void SpecialProcessText(XText xText)
        {
            string content = xText.Value;
            if (Regex.IsMatch(content, @"\n") || Regex.IsMatch(content, @"\r\n"))
            {
                // remove blank line
                //content = Regex.Replace(content, @"^\s+|\s+$", string.Empty, RegexOptions.Multiline);
                content = Regex.Replace(content, @"\s+\n|\s+\r\n", " ", RegexOptions.Multiline);
                content = Regex.Replace(content, @"\n|\r\n", "", RegexOptions.Multiline);
                content = Regex.Replace(content, @"\s*-or-\s*", "-or-", RegexOptions.Multiline);
                content = Regex.Replace(content, @"\s{2,}", " ", RegexOptions.Multiline);

                xText.Value = content;
            }
        }

        public static bool IsEmpty(XElement ele)
        {
            bool isEmpty = true;
            if (ele != null)
            {
                var child = ele.Nodes().ToArray();
                if (child != null && child.Length > 0)
                {
                    for (int i = 0; i < child.Length; i++)
                    {
                        if (child[i].NodeType == System.Xml.XmlNodeType.Text)
                        {
                            if (!string.IsNullOrEmpty((child[i] as XText).Value))
                            {
                                isEmpty = false;
                            }
                        }
                        else if (child[i].NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (IsEmpty(child[i] as XElement) == false)
                            {
                                isEmpty = false;
                            }
                        }
                    }
                }
            }

            if (isEmpty == true)
            {
                string localName = ele.Name.LocalName.ToLower();
                if (localName == "param"
                    || localName == "summary"
                    || localName == "exception"
                    || localName == "typeparam"
                    || localName == "returns")
                {
                    ele.Remove();
                }
            }

            return isEmpty;
        }
        static FileInfo[] GetFiles(string path, string pattern)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles(pattern, SearchOption.AllDirectories).OrderBy(f => f.Name).ToArray();
        }

        static void WriteLine(string format, params object[] args)
        {
            string timestamp = string.Format("[{0}]", DateTime.Now.ToString());
            Console.WriteLine(timestamp + string.Format(format, args));
        }
    }
}
