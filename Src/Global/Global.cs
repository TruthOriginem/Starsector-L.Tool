using Microsoft.Win32;
using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;

namespace StarsectorLTool.Src.Global
{
    static class Global
    {
        public const string VERSION_URL = "https://raw.githubusercontent.com/TruthOriginem/Starsector-L.Tool/master/Properties/verison.xml";
        public const string TEST_VM_PATH = "E:\\Starsector08\\Starsector";
        public static string STARSECTOR_EXE_PATH;
        public static string VM_PATH;
        public static VmparamsData VM_DATA;
        public static SSRegisterData SS_REGISTER_DATA;

        public static bool InitRegisterData()
        {
            SS_REGISTER_DATA = new SSRegisterData();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(SSRegisterData.REGISTER_PATH);
            string enableModString = (string)key.GetValue("enabled_mods");
            string[] enable_mods = enableModString.Split(new char[] { '|' });
            SS_REGISTER_DATA.modAmount = enable_mods.Length;
            string serialKey = (string)key.GetValue("serial");
            if (!string.IsNullOrWhiteSpace(serialKey))
            {
                serialKey = serialKey.Replace("/", "");
                SS_REGISTER_DATA.isGameLegal = true;
            }
            else
            {
                SS_REGISTER_DATA.isGameLegal = false;
            }
            SS_REGISTER_DATA.serialKey = "无法获取到序列码...";
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                SS_REGISTER_DATA.serialKey = serialKey;
            }
            return true;
        }
    }
    struct VmparamsData
    {
        /// <summary>
        /// vmparams文件字符串。
        /// </summary>
        public string rawVm;
        public string xmsx;

        /// <summary>
        /// 将文件内容解析为VmparamsData
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static VmparamsData ParseStringToData(string text)
        {
            VmparamsData data = new VmparamsData();
            data.rawVm = text;
            Regex regex = new Regex(@"(?:\-[xX][mM][sS])(.+?)\b");
            data.xmsx = regex.Match(text).Groups[1].Value;
            return data;
        }
        /// <summary>
        /// 从指定VmparamsData中将指定的内存字符串（如4096m）替换进应有的位置
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetSetting"></param>
        /// <returns></returns>
        public static string ReplaceXMSetting(VmparamsData data, string targetSetting)
        {
            Regex regex = new Regex(@"(\-[xX][mM][sSxX])(.+?)\b");
            return regex.Replace(data.rawVm, "${1}" + targetSetting);
        }
    }

    struct SSRegisterData
    {
        public const string REGISTER_PATH = "Software\\JavaSoft\\Prefs\\com\\fs\\starfarer";
        public int modAmount;
        public string serialKey;
        public bool isGameLegal;
    }

}
