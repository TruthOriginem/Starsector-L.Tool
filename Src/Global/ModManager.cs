using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StarsectorLTool.Src.Global
{
    class ModManager
    {
        public const string GRAPHIC_CONFIG_NAME = "GRAPHICS_OPTIONS.ini";
        public static ModManager Instance;
        public ModManager()
        {
            Instance = this;
        }
        /// <summary>
        /// 读取GRAPHICS_OPTIONS，返回是否成功
        /// </summary>
        /// <returns></returns>
        public static bool LoadGraphicsLibPath()
        {
            string ssPath = Global.GetCurrentDirectoryPath();
            string modPath = ssPath + @"\mod";
            try
            {
                foreach (var modFolderPath in Directory.GetDirectories(modPath))
                {
                    string path = modFolderPath + @"\" + GRAPHIC_CONFIG_NAME;
                    if (File.Exists(path))
                    {

                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
