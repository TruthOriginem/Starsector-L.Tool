using StarsectorLTool.Src.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace StarsectorLTool.Src.Global
{
    class ModManager
    {
        public const string GRAPHIC_CONFIG_NAME = "GRAPHICS_OPTIONS.ini";
        public static Dictionary<string, List<string>> CONFIGS = new Dictionary<string, List<string>>
        {
            {"GRAPHICS_OPTIONS.ini",new List<string>
                {
                    "enableShaders",
                    "use64BitBuffer"
                }
            },
        };

        private static List<ModConfigData> currentConfigDatas = new List<ModConfigData>();

        public static bool IsLoad = false;
        private static ModManager instance;
        internal static ModManager Instance
        {
            get
            {
                if (instance == null)
                {
                    return new ModManager();
                }
                else
                {
                    return instance;
                }
            }
        }

        internal static List<ModConfigData> CurrentConfigDatas { get => currentConfigDatas; }

        public ModManager()
        {
            instance = this;
        }
        //public bool CheckAllAvailableConfigs()
        //{

        //}

        /// <summary>
        /// 预读所有存在的内置config选项
        /// </summary>
        /// <returns></returns>
        public static void LoadAllBuiltInConfigPath()
        {
            if (IsLoad)
            {
                return;
            }
            string ssPath = Global.GetCurrentDirectoryPath();
            string modPath = ssPath + @"\mods";
            //MessageBox.Show(modPath);
            if (!Directory.Exists(modPath)) return;
            foreach (var modFolderPath in Directory.GetDirectories(modPath))
            {
                foreach (var configKV in CONFIGS)
                {
                    string path = modFolderPath + @"\" + configKV.Key;
                    if (File.Exists(path))
                    {
                        //MessageBox.Show("存在");
                        List<string> configKeys = configKV.Value;
                        ModConfigData modConfigData = new ModConfigData(path, configKeys);
                        currentConfigDatas.Add(modConfigData);
                    }
                }
            }

            IsLoad = true;
        }

    }
}
