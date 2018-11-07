using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace StarsectorLTool.Src.Mod
{
    /// <summary>
    /// mod信息的基类
    /// </summary>
    class BaseModInfo
    {
        protected string modPath;
        protected string modName;
        protected string modVersion;
        private List<ModConfigData> configDatas;
        internal List<ModConfigData> ConfigDatas { get => configDatas; }

        protected BaseModInfo(string modPath, string modName, string modVersion)
        {
            this.modPath = modPath;
            this.modName = modName;
            this.modVersion = modVersion;
            configDatas = new List<ModConfigData>();
        }


        protected void LoadConfig(string path, List<string> configKeys)
        {
            var config = new ModConfigData(path, configKeys);
            configDatas.Add(config);
        }
        protected virtual void LoadConfigs()
        {

        }
        /// <summary>
        /// 检查是否有设置被改变过了
        /// </summary>
        /// <returns></returns>
        protected bool CheckConfigChanged()
        {
            foreach (var config in configDatas)
            {
                if (config.IfChanged())
                {
                    return true;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// mod的一个配置文件，注意，只是一个，一个mod可能对应多个配置文件
    /// </summary>
    class ModConfigData
    {
        private string configName;
        private string configPath;
        private string configRDME;
        private string rawData;
        private List<string> configKeys;
        private Dictionary<string, string> configKeyToValues;
        private Dictionary<string, bool> configKeyIfChanges;
        private Dictionary<string, string> configKeyToRDMEs;

        public string ConfigName { get => configName; }

        public string this[string key]
        {
            get
            {
                return configKeyToValues[key];
            }
            set
            {
                configKeyToValues[key] = value;
                configKeyIfChanges[key] = !configKeyIfChanges[key];
            }
        }

        public ModConfigData(string configPath, List<string> configKeys)
        {
            this.configPath = configPath;
            this.configKeys = configKeys;
            configKeyToValues = new Dictionary<string, string>();
            configKeyIfChanges = new Dictionary<string, bool>();
            ParseConfig();
        }
        /// <summary>
        /// 设定指定Config条目的说明
        /// </summary>
        /// <param name="itemKey"></param>
        /// <param name="content"></param>
        public void SetConfigItemRDME(string itemKey, string content)
        {
            if (configKeyToRDMEs == null)
            {
                configKeyToRDMEs = new Dictionary<string, string>();
            }
            configKeyToRDMEs[itemKey] = content;
        }
        public string GetConfigItemRDME(string itemKey)
        {
            if (configKeyToRDMEs == null)
            {
                configKeyToRDMEs = new Dictionary<string, string>();
            }
            string rdme = configKeyToRDMEs[itemKey];
            if (string.IsNullOrEmpty(rdme))
            {
                return "暂无说明。";
            }
            return rdme;
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        public void SaveConfig()
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader reader = new StringReader(rawData))
            {
                string line;
                int lineIndex = 0;
                Regex regex = new Regex(@"""(\w*)"":(true|false|\d*.?,)");
                while ((line = reader.ReadLine()) != null)
                {
                    string rawLine = string.Copy(line);
                    //每行满足正则表达式的内容
                    foreach (Match match in regex.Matches(line))
                    {
                        var key = match.Groups[1].Value;
                        //如果这是需要设置的，并且设置过
                        if (configKeys.Contains(key) && configKeyIfChanges[key])
                        {
                            Regex replaceRegex = new Regex(@"(?<=""" + key + @""":)(true|false|\d*.?,)");
                            rawLine = replaceRegex.Replace(rawLine, configKeyToValues[key]);
                            configKeyIfChanges[key] = false;
                            MessageBox.Show(rawLine);
                        }
                    }
                    sb.AppendLine(rawLine);
                    ++lineIndex;
                }
            }
            rawData = sb.ToString();
            File.WriteAllText(configPath, rawData);
        }

        /// <summary>
        /// 解析文件
        /// </summary>
        private void ParseConfig()
        {
            try
            {
                //获取config文件名
                configName = Path.GetFileName(configPath);
                rawData = File.ReadAllText(configPath);
                //先正则获取所有".":.,格式的文字
                Regex regex = new Regex(@"""(\w*)"":(true|false|\d*.?,)");
                foreach (Match match in regex.Matches(rawData))
                {
                    var key = match.Groups[1].Value;
                    //如果第一捕获组的Key在需要的Key之中，则选择他们
                    if (configKeys.Contains(key))
                    {
                        var value = match.Groups[2].Value;
                        configKeyToValues[key] = value;
                        configKeyIfChanges[key] = false;

                    }
                }
            }
            catch
            {
                throw new Exception("读取Mod的Config类文件时出错！");
            }
        }
        /// <summary>
        /// 返回这个ConfigData是否变化过。
        /// </summary>
        /// <returns></returns>
        public bool IfChanged()
        {
            bool changed = false;
            foreach (var kv in configKeyIfChanges)
            {
                if (kv.Value)
                {
                    changed = true;
                    break;
                }
            }
            return changed;
        }
    }
}
