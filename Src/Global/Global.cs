using Microsoft.Win32;
using System.Security.Principal;

namespace StarsectorLTool.Src.Global
{
    static class Global
    {
        public const string UPDATE_URL = ""
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
        public string beforeXms;
        public string afterAndContainXss;
        public string xms;
        public string xmx;
        /// <summary>
        /// vmparams文件字符串。
        /// </summary>
        public string rawVm;
    }

    struct SSRegisterData
    {
        public const string REGISTER_PATH = "Software\\JavaSoft\\Prefs\\com\\fs\\starfarer";
        public int modAmount;
        public string serialKey;
        public bool isGameLegal;
    }

}
