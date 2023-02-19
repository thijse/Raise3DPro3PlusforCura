using System.Text;
using static ProfileGenerator.ProfileGenerator;

namespace ProfileGenerator
{
    public struct IniConfig
    {
        public int PadLeftOfEqual = 0;
        public int PadRightOfEqual = 1;
        public bool DoNotSetEmptyValues = true;
        public IniConfig() {}
    }
    

    public class Ini
    {
        Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
        private IniConfig _iniconfig;
        string fileName = null;

        public string FileName => fileName;

        /// <summary>
        /// Initialize an INI file
        /// Load it if it exists
        /// </summary>
        /// <param name="file">Full path where the INI file has to be read from or written to</param>
        public Ini(string fileName, IniConfig? iniConfig= null)
        {
            _iniconfig = iniConfig??new IniConfig();
            this.fileName = fileName;
            if (!File.Exists(fileName))
                return;

            Load();
        }

        public Ini(IniConfig? iniConfig = null)
        {
            _iniconfig = iniConfig ?? new IniConfig();
        }


        /// <summary>
        /// Load the INI file content
        /// </summary>
        public void Load(string fileName)
        {
            this.fileName=fileName;
            Load();
        }

        /// <summary>
        /// Load the INI file content
        /// </summary>
        public void Load()
        {
            var txt = File.ReadAllText(fileName);

            Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            ini[""] = currentSection;

            foreach (var l in txt.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                              .Select((t, i) => new
                              {
                                  idx = i,
                                  text = t.Trim()
                              }))
            // .Where(t => !string.IsNullOrWhiteSpace(t) && !t.StartsWith(";")))
            {
                var line = l.text;

                if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
                {
                    currentSection.Add(";" + l.idx.ToString(), line);
                    continue;
                }

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    ini[line.Substring(1, line.Length - 2).Trim()] = currentSection;
                    continue;
                }

                var idx = line.IndexOf("=");
                if (idx == -1)
                    currentSection[line.Trim()] = "";
                else
                    currentSection[line.Substring(0, idx).Trim()] = line.Substring(idx + 1).Trim();
            }
        }

        /// <summary>
        /// Get a parameter value at the root level
        /// </summary>
        /// <param name="key">parameter key</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return GetValue(key, "", "");
        }

        /// <summary>
        /// Get a parameter value in the section
        /// </summary>
        /// <param name="key">parameter key</param>
        /// <param name="section">section</param>
        /// <returns></returns>
        public string GetValue(string key, string section)
        {
            return GetValue(key, section, "");
        }

        /// <summary>
        /// Returns a parameter value in the section, with a default value if not found
        /// </summary>
        /// <param name="key">parameter key</param>
        /// <param name="section">section</param>
        /// <param name="default">default value</param>
        /// <returns></returns>
        public string GetValue(string key, string section, string @default)
        {
            if (!ini.ContainsKey(section))
                return @default;

            if (!ini[section].ContainsKey(key))
                return @default;

            return ini[section][key];
        }

        /// <summary>
        /// Save the INI file
        /// </summary>
        public void Save(string fileName)
        {
            this.fileName = fileName;
            Save();
        }

        /// <summary>
        /// Save the INI file
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var keyValueMarkup = @"{0}"+ new String(' ', _iniconfig.PadLeftOfEqual)+@"=" + new String(' ', _iniconfig.PadRightOfEqual) + @"{1}";
            var sb = new StringBuilder();
            foreach (var section in ini)
            {
                if (section.Key != "")
                {
                    sb.AppendFormat("[{0}]", section.Key);
                    sb.AppendLine();
                }

                foreach (var keyValue in section.Value)
                {
                    if (keyValue.Key.StartsWith(";"))
                    {
                        sb.Append(keyValue.Value);
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendFormat(keyValueMarkup, keyValue.Key, keyValue.Value);
                        sb.AppendLine();
                    }
                }

                if (!endWithCRLF(sb))
                    sb.AppendLine();
            }

            File.WriteAllText(fileName, sb.ToString());
        }

        bool endWithCRLF(StringBuilder sb)
        {
            if (sb.Length < 2) return false;
            if (sb.Length < 4)
                return sb[sb.Length - 2] == '\r' &&
                       sb[sb.Length - 1] == '\n';
            else
                return sb[sb.Length - 4] == '\r' &&
                       sb[sb.Length - 3] == '\n' &&
                       sb[sb.Length - 2] == '\r' &&
                       sb[sb.Length - 1] == '\n';
        }

        /// <summary>
        /// Write a parameter value at the root level
        /// </summary>
        /// <param name="key">parameter key</param>
        /// <param name="value">parameter value</param>
        public void WriteValue(string key, string value)
        {
            SetValue(key, "", value);
        }

        /// <summary>
        /// Write a parameter value in a section
        /// </summary>
        /// <param name="key">parameter key</param>
        /// <param name="section">section</param>
        /// <param name="value">parameter value</param>
        public void SetValue(string key, string section, string value)
        {
            key     = key.Trim();
            section = section.Trim();
            value   = value.Trim();
            if (string.IsNullOrEmpty(key)) return;
            if (_iniconfig.DoNotSetEmptyValues && !key.StartsWith(";") && string.IsNullOrEmpty(value) )return;
            Dictionary<string, string> currentSection;
            if (!ini.ContainsKey(section))
            {
                currentSection = new Dictionary<string, string>();
                ini.Add(section, currentSection);
            }
            else
                currentSection = ini[section];

            currentSection[key] = value;
        }

        /// <summary>
        /// Get all the keys names in a section
        /// </summary>
        /// <param name="section">section</param>
        /// <returns></returns>
        public string[] GetKeys(string section)
        {
            if (!ini.ContainsKey(section))
                return new string[0];

            return ini[section].Keys.ToArray();
        }

        /// <summary>
        /// Get all the section names of the INI file
        /// </summary>
        /// <returns></returns>
        public string[] GetSections()
        {
            return ini.Keys.Where(t => t != "").ToArray();
        }

        /// <summary>      
        /// Return if contains section 
        /// </summary>
        /// <returns></returns>
        public bool ContainsSection(string section)
        {
            return ini.ContainsKey(section);
        }

        /// <summary>      
        /// Return if contains section and key 
        /// </summary>
        /// <returns></returns>
        public bool ContainsKey(string section, string key)
        {
            if (!ini.ContainsKey(section)) return false;
            return ini[section].ContainsKey(key);
        }

        public Ini Clone()
        {
            var iniConfig = new IniConfig() { DoNotSetEmptyValues = _iniconfig.DoNotSetEmptyValues, PadLeftOfEqual = _iniconfig.PadLeftOfEqual, PadRightOfEqual = _iniconfig.PadRightOfEqual };
            var clone     = new Ini(iniConfig);

            // Go through all sections
            foreach (var section in GetSections())
            {
                // Go through all keys in the section
                foreach (var key in GetKeys(section))
                {
                    clone.SetValue(key, section, ini[section][key]);
                }
            }
            return clone;
        }

    }
}
