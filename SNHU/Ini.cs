using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Punk.Utils
{	
    public class Ini
    {
        private Dictionary<string, Dictionary<string, string>> _iniFileContent;
        private readonly Regex _sectionRegex = new Regex(@"(?<=\[)(?<SectionName>[^\]]+)(?=\])");
        private readonly Regex _keyValueRegex = new Regex(@"(?<Key>[^=]+)\s=\s(?<Value>.+)");

        public Ini() : this(null){}

        public Ini(string source)
        {
            _iniFileContent = new Dictionary<string, Dictionary<string, string>>();
            if (source != null) Parse(source);
        }
        
        public IDictionary<string, string> this[string sectionName]
        {
        	get { return GetSection(sectionName); }
        	set { SetSection(sectionName, value); }
        }
        
        public string this[string sectionName, string key]
        {
        	get { return GetValue(sectionName, key); }
        	set { SetValue(sectionName, key, value); }
        }

        /// <summary>
        /// Get a specific value from the .ini file
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <returns>The value of the given key in the given section, or NULL if not found</returns>
        public string GetValue(string sectionName, string key)
        {
            if (_iniFileContent.ContainsKey(sectionName) && _iniFileContent[sectionName].ContainsKey(key))
                return _iniFileContent[sectionName][key];
            else
                return null;
        }

        /// <summary>
        /// Set a specific value in a section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string sectionName, string key, string value)
        {
            if(!_iniFileContent.ContainsKey(sectionName)) _iniFileContent[sectionName] = new Dictionary<string, string>();
            _iniFileContent[sectionName][key] = value;
        }

        /// <summary>
        /// Get all the Values for a section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns>A Dictionary with all the Key/Values for that section (maybe empty but never null)</returns>
        public Dictionary<string, string> GetSection(string sectionName)
        {
            if (_iniFileContent.ContainsKey(sectionName))
                return new Dictionary<string, string>(_iniFileContent[sectionName]);
            else
                return new Dictionary<string, string>();
        }

        /// <summary>
        /// Set an entire sections values
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionValues"></param>
        public void SetSection(string sectionName, IDictionary<string, string> sectionValues)
        {
            if (sectionValues == null) return;
            _iniFileContent[sectionName] = new Dictionary<string, string>(sectionValues);
        }


        /// <summary>
        /// Load an .INI File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Parse(string source)
        {
            try
            {
            	var content = source.Split('\n');
                _iniFileContent = new Dictionary<string, Dictionary<string, string>>();
                string currentSectionName = string.Empty;
                foreach (var line in content)
                {
                    Match m = _sectionRegex.Match(line);
                    if (m.Success)
                    {
                        currentSectionName = m.Groups["SectionName"].Value;
                    }
                    else
                    {
                        m = _keyValueRegex.Match(line);
                        if (m.Success)
                        {
                            string key = m.Groups["Key"].Value;
                            string value = m.Groups["Value"].Value;

                            Dictionary<string, string> kvpList;
                            if (_iniFileContent.ContainsKey(currentSectionName))
                            {
                                kvpList = _iniFileContent[currentSectionName];
                            }
                            else
                            {
                                kvpList = new Dictionary<string, string>();
                            }
                            kvpList[key] = value;
                            _iniFileContent[currentSectionName] = kvpList;
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save the content of this class to an INI File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Save(string filename)
        {
            var sb = new StringBuilder();
            if (_iniFileContent != null)
            {
                foreach (var sectionName in _iniFileContent)
                {
                    sb.AppendFormat("[{0}]\r\n", sectionName.Key);
                    foreach (var keyValue in sectionName.Value)
                    {
                        sb.AppendFormat("{0}={1}\r\n", keyValue.Key, keyValue.Value);
                    }
                }
            }
            try
            {
                File.WriteAllText(filename, sb.ToString());
                return true;
            } catch
            {
                return false;
            }
        }
        
        public override string ToString()
		{
			var result = "";
			
			foreach (var section in _iniFileContent.Keys)
			{
				result += "[" + section + "]\n";
				foreach (var pair in _iniFileContent[section])
				{
					result += pair.Key + "=" + pair.Value + "\n";
				}
				
			}
			
			return result;
		}

    }
}