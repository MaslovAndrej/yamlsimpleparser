using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace YamlSimple
{
    public static class Parser
    {
        public static Dictionary<string, string> ParseFile(string path)
        {
            if (!File.Exists(path))
                return new Dictionary<string, string>();

            var yaml = File.ReadAllText(path);
            return Parse(yaml);
        }

        public static Dictionary<string, string> Parse(string yaml)
        {
            var values = new Dictionary<string, string>();
            var lines = GetYamlLines(yaml);

            if (!lines.Any() || lines.Any(x => !x.Contains(":")))
                return values;

            var headKey = string.Empty;
            var previousKey = string.Empty;
            var previousIndent = 0;
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("-") || line.Trim().StartsWith("'"))
                    continue;

                var pair = line.Trim().Split(':');
                var key = pair[0].Trim();
                var indent = line.IndexOf(key);

                headKey = GetHeadKey(indent, previousIndent, headKey, previousKey);
                if (!string.IsNullOrEmpty(headKey))
                    key = $"{headKey}.{key}";

                if (values.ContainsKey(key))
                    throw new DuplicateNameException(key);

                var value = pair[1].Trim();
                if (line.Contains("'"))
                {
                    var quotes = line.Split('\'');
                    value = quotes[1];
                }

                previousKey = key;
                previousIndent = indent;

                values[key] = value;
            }

            return values;
        }

        private static string GetHeadKey(int indent, int previousIndent, string headKey, string previousKey)
        {
            if (indent == 0)
                return string.Empty;

            if (indent > previousIndent)
                return previousKey;

            if (indent < previousIndent)
            {
                var keys = headKey.Split('.');
                var i = indent;
                var count = keys.Length;
                while (i > 0)
                {
                    count--;
                    keys[count] = null;
                    i = i - 4;
                }

                return string.Join(".", keys.Where(x => x != null));
            }

            return headKey;
        }

        public static void UpdateFileStringValue(string path, string key, string value)
        {
            if (!File.Exists(path))
                return;

            var yaml = File.ReadAllText(path);
            yaml = UpdateStringValue(yaml, key, value);
            File.WriteAllText(path, yaml);
        }

        public static string UpdateStringValue(string yaml, string key, string value)
        {
            var lines = GetYamlLines(yaml);
            var values = Parse(yaml);
            var oldValue = values[key];
            if (string.IsNullOrWhiteSpace(oldValue))
            {
                oldValue = "''";
                value = $"'{value}'";
            }

            var currentKey = key.Split('.')[key.Count(x => x == '.')];
            var oldLine = lines.Find(x => x.Contains(currentKey) && x.Contains(oldValue));
            var newLine = oldLine.Replace(oldValue, value);

            return yaml.Replace(oldLine, newLine);
        }

        private static List<string> GetYamlLines(string yaml)
        {
            var separator = "\n";

            if (yaml.Contains(Environment.NewLine))
                separator = Environment.NewLine;

            return yaml.Split(new string[] { separator }, StringSplitOptions.None)
              .Where(x => !string.IsNullOrWhiteSpace(x) && !x.Trim().StartsWith("#")).ToList();
        }
    }
}
