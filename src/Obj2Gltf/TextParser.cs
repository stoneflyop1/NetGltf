using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Obj2Gltf
{
    class TextParser
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        internal static void Lex(StreamReader reader, Action<string, IList<string>> action)
        {
            var args = new List<string>();
            var multiLine = new StringBuilder();
            string line;
            while((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }
                line = line.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries)[0];

                if (line.EndsWith(@"\"))
                {
                    multiLine.Append(line + " ");
                    continue;
                }
                multiLine.Append(line);

                var ss = multiLine.ToString();
                var sepIndex = ss.IndexOf(' ');
                string key;
                if (sepIndex != -1)
                {
                    key = ss.Substring(0, sepIndex).Trim();
                    args.Add(ss.Substring(sepIndex + 1).Trim());
                }
                else
                {
                    key = ss.Trim();
                }
                //var words = ss.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                //var key = words[0];
                //args.AddRange(words.Skip(1));
                action(key, args);
                args.Clear();

                multiLine.Clear();
            }
        }

        internal static IList<string> ParseArgs(IList<string> input)
        {
            return String.Join(" ", input).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
