using UnityEngine;

using Uzil.Mod;
using Uzil.Util;

public static class LuaConst
{

    
    public static string luaDir = PathUtil.Combine(PathUtil.GetDataPath(), "");                //lua逻辑代码目录
    // public static string luaDir = string.Format("{0}/{1}/Lua", Application.persistentDataPath, osDir);      //手机运行时lua文件下载目录    

    public static string toluaDir = Application.dataPath + "/Import_out/Uzil/Lib/ToLua";        //tolua lua文件目录

    public static string luaDir_toLua = toluaDir + "/ToLua/Lua";        //tolua lua文件目录

    public static string mainFilePath = PathUtil.Combine(ModUtil.LUASCRIPT_FOLDERNAME, "Main.lua");

#if UNITY_STANDALONE
    public static string osDir = "Win";
#elif UNITY_ANDROID
    public static string osDir = "Android";
#elif UNITY_IPHONE
    public static string osDir = "iOS";
#else
    public static string osDir = "";
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN    
    public static string zbsDir = "D:/ZeroBraneStudio/lualibs/mobdebug";        //ZeroBraneStudio目录       
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	public static string zbsDir = "/Applications/ZeroBraneStudio.app/Contents/ZeroBraneStudio/lualibs/mobdebug";
#else
    public static string zbsDir = luaResDir + "/mobdebug/";
#endif    

    public static bool openLuaSocket = false;            //是否打开Lua Socket库
    public static bool openLuaDebugger = false;         //是否连接lua调试器
}