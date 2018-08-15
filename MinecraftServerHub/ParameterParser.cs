using System.Collections.Generic;
using System.IO;
using System.Text;
using MinecraftServerHub.ParamGroup;

namespace MinecraftServerHub
{
    public class ParameterParser
    {
        private Dictionary<string, IParameterGroup> groups;
        
        public ParameterParser(IEnumerable<KeyValuePair<string, IParameterGroup>> parameterGroups)
        {
            groups = new Dictionary<string, IParameterGroup>();
            foreach (var group in parameterGroups)
            {
                groups.Add(group.Key, group.Value);
            }
        }

        public string Parse(FileInfo file)
        {
            var builder = new StringBuilder();
            using (var reader = file.OpenText())
            {
                int c;
                while ((c = reader.Read()) >= 0)
                {
                    if (c == '$')
                    {
                        c = reader.Read();
                        if (c < 0)
                            break;
                        else if (c == '{')
                        {
                            string variable = ReadTil(reader, ch => ch != '}');
                            var pair = GetGroupAndParameter(variable);
                            builder.Append(GetValue(pair));
                            continue;
                        }
                        else if (c != '$')
                        {
                            string variable = (char)c + ReadTil(reader, ch =>
                            {
                                c = ch;
                                return char.IsLetter(ch) || char.IsDigit(ch) || ch == '_';
                            });
                            var pair = GetGroupAndParameter(variable);
                            builder.Append(GetValue(pair));
                        }
                    }
                    builder.Append((char)c);
                }
            }
            return builder.ToString();
        }

        private delegate bool Checker(char c);

        private static string ReadTil(TextReader reader, Checker checker)
        {
            var builder = new StringBuilder(256);
            int c;
            while ((c = reader.Read()) >= 0 && checker((char)c))
            {
                builder.Append((char)c);
            }
            return builder.ToString();
        }

        private KeyValuePair<string, string> GetGroupAndParameter(string variable)
        {
            int delimeter = variable.IndexOf(':');
            if (delimeter >= 0)
            {
                string group = variable.Substring(0, delimeter).Trim();
                string param = variable.Substring(delimeter + 1).Trim();
                return new KeyValuePair<string, string>(group, param);
            }
            else
                return new KeyValuePair<string, string>("main", variable.Trim());
        }

        private string GetValue(KeyValuePair<string, string> pair)
        {
            if (groups.ContainsKey(pair.Key))
                return groups[pair.Key].GetValueFor(pair.Value) ?? "{NULL}";
            else
                return "{NO GROUP}";
        }
    }
}
