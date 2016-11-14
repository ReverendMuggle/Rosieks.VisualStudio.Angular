using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rosieks.VisualStudio.Angular.Extensions;

namespace Rosieks.VisualStudio.Angular.Services.TypeScript
{
    [Export(typeof(IHierarchyElementsProvider<NgComponent>))]
    class TypeScriptComponentsProvider : IHierarchyElementsProvider<NgComponent>
    {
        public IReadOnlyList<NgComponent> GetElements(NgHierarchy hierarchy)
        {
            return FindComponents(hierarchy.RootPath);
        }

        private static IReadOnlyList<NgComponent> FindComponents(string rootPath)
        {
            return Directory
                .EnumerateFiles(rootPath, "*.ts", SearchOption.AllDirectories)
                .IsValidPath()
                .SelectMany(FindComponentInFile)
                .ToReadOnlyList();
        }

        private static IEnumerable<NgComponent> FindComponentInFile(string file)
        {
            var content = File.ReadAllText(file);
            var regex = new Regex(@"export\s+function\s+(\w[\w\d]+)\s*\(\)\s*\:\s*ng\.IComponent");
            return regex.Matches(content).Cast<Match>().Select(x => new NgComponent
            {
                Name = x.Groups[1].Value,
                DashedName = CreateDashedName(x.Groups[1].Value),
                CodeFileName = file,
                ViewFileName = file.Replace(".ts", ".html"),
                Restrict = NgComponentRestrict.All, // TODO: Add detection of restriction
                Attributes = ParseScope(content, x),
            });
        }

        private static NgComponentAttribute[] ParseScope(string content, Match componentMatch)
        {
            var regex = new Regex(@"scope\s*\:\s*\{([^\}]*)\}");
            var postComponentCode = content.Substring(componentMatch.Index + componentMatch.Length);
            var match = regex.Match(postComponentCode);
            if (match != null && match.Groups.Count == 2)
            {
                return match.Groups[1].Value
                    .Split(',')
                    .Select(keyValuePair =>
                    {
                        var parts = keyValuePair.Split(':').Select(x => x.Trim()).ToArray();
                        if (parts.Length == 2)
                        {
                            var key = parts[0];
                            var value = parts[1].Length >= 3 ? parts[1].Substring(2, parts[1].Length - 3) : null;
                            if (value == null)
                            {
                                return null;
                            }
                            else if (value == string.Empty)
                            {
                                return new NgComponentAttribute { Name = key, DashedName = CreateDashedName(key) };
                            }
                            else
                            {
                                return new NgComponentAttribute { Name = value, DashedName = CreateDashedName(value) };
                            }
                        }
                        else
                        {
                            return null;
                        }
                    })
                    .Where(x => x != null)
                    .ToArray();
            }
            else
            {
                return null;
            }
        }

        private static string CreateDashedName(string value)
        {
            return Regex.Replace(value, @"(?<!-)([A-Z])", "-$1").ToLower();
        }
    }
}
