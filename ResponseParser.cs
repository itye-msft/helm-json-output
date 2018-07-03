using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace jsonplugin
{
    public class ReponseParser
    {
        ///Entry point to the plugin
        public void HandleCmd(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower().Equals("help"))
            {
                PrintUsage();
            }
            else
            {
                var cmd = String.Join(" ", args);
                var output = cmd.Helm();
                var releaseName = ExtractReleaseName(output);
                var unFormattedResources = ParseResources(ExtractResources(output));
                var stucturedResources = unFormattedResources.Select(x => ConvertToObject(x));
                var json = JObject.FromObject(new {releaseName=releaseName, resources=JArray.FromObject(stucturedResources)}).ToString();
                Console.WriteLine(json);
            }
        }

        // Prints pretty help
        void PrintUsage()
        {
            Console.WriteLine(@"Converts helm's output to be in json format.
Works for commands: install, status

Example usage:
    helm json install stable/rabbitmq
    helm json status my-release-name
");
        }

        // Extracts the resources section from the response
        string ExtractReleaseName(string output)
        {
            if(!String.IsNullOrWhiteSpace(output)){
                var reader = new StringReader(output);
                var firstLine = reader.ReadLine();
                if(!String.IsNullOrWhiteSpace(firstLine)){
                    if(firstLine.StartsWith("NAME:")){
                        var name = firstLine.Replace("NAME:","").Trim();
                        return name;
                    }
                }
            }
            return String.Empty;
        }

        // Extracts the resources section from the response
        string ExtractResources(string output)
        {
            Regex r = new Regex("RESOURCES:(.*?)NOTES:", RegexOptions.Singleline);
            var m = r.Match(output);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return null;
        }

        // Parses out the a list of resources as unformatted text
        List<string> ParseResources(string res)
        {
            Regex r = new Regex("==> ", RegexOptions.Singleline);
            var matches = r.Matches(res);
            var lastIndex = 0;
            var list = new List<string>();
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    if (lastIndex == 0)
                    {
                        lastIndex = m.Index + m.Length;
                    }
                    else
                    {
                        var s = res.Substring(lastIndex, m.Index - lastIndex - m.Length);
                        list.Add(s);
                        lastIndex = m.Index + m.Length;
                    }
                }
            }
            if (matches.Count>0)
            {
                var s = res.Substring(lastIndex);
                list.Add(s);
            }
            return list;
        }

        // Converts unfommatted text to structured object file
        dynamic ConvertToObject(string unformattedText)
        {
            Regex r = new Regex("(.*?)\\s", RegexOptions.Singleline);
            
            StringReader reader = new StringReader(unformattedText);
            string line, name = reader.ReadLine();
            var resources = new List<string>();
            while((line = reader.ReadLine()) != null)
            {
                if (!String.IsNullOrWhiteSpace(line) && !line.StartsWith("NAME"))
                {
                    var m = r.Match(line);
                    if (m.Success)
                    {
                        resources.Add(m.Groups[1].Value);
                    }
                }
            }
            return new {
                name =name,
                resources = resources
            };
        }
    }
}