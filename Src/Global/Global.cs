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
        public static string Starsector_exe_path;
        public static string Vm_path;
        public static VmparamsData Vm_data;
        public static SSRegisterData Ss_registerData;
        /// <summary>
        /// 获取远行星号注册表信息
        /// </summary>
        /// <returns></returns>
        public static bool InitRegisterData()
        {
            Ss_registerData = new SSRegisterData();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(SSRegisterData.REGISTER_PATH);
            string enableModString = (string)key.GetValue("enabled_mods");
            string[] enable_mods = enableModString.Split(new char[] { '|' });
            Ss_registerData.modAmount = enable_mods.Length;
            string serialKey = (string)key.GetValue("serial");
            if (!string.IsNullOrWhiteSpace(serialKey))
            {
                serialKey = serialKey.Replace("/", "");
                Ss_registerData.isGameLegal = true;
            }
            else
            {
                Ss_registerData.isGameLegal = false;
            }
            Ss_registerData.serialKey = SSRegisterData.COULD_NOT_FIND_KEY;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Ss_registerData.serialKey = serialKey;
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
        /// 将文件内容解析为VmparamsData。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static VmparamsData ParseStringToData(string text)
        {
            VmparamsData data = new VmparamsData();
            data.rawVm = text;
            Regex regex = new Regex(@"(?<=\-xms)(.+?)\b", RegexOptions.IgnoreCase);
            data.xmsx = regex.Match(text).Value;
            return data;
        }
        /// <summary>
        /// 从指定VmparamsData中将指定的内存字符串（如4096m）替换进应有的位置。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetSetting"></param>
        /// <returns></returns>
        public static string ReplaceXMSetting(VmparamsData data, string targetSetting)
        {
            Regex regex = new Regex(@"(?<=\-xm[sx])(.+?)\b", RegexOptions.IgnoreCase);
            return regex.Replace(data.rawVm, targetSetting);
        }
    }

    struct SSRegisterData
    {
        public const string REGISTER_PATH = "Software\\JavaSoft\\Prefs\\com\\fs\\starfarer";
        public const string COULD_NOT_FIND_KEY = "无法获取到序列码...";
        public int modAmount;
        public string serialKey;
        public bool isGameLegal;
    }

}
