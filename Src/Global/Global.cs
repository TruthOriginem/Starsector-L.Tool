﻿namespace StarsectorLTool.Src.Global
{
    static class Global
    {
        public const string TEST_VM_PATH = "E:\\Starsector08\\Starsector";
        public static string STARSECTOR_EXE_PATH;
        public static string VM_PATH;
        public static VmparamsData VM_DATA;

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
    
}
