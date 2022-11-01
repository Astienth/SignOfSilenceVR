using System.Diagnostics;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Valve.VR;

namespace SignOfSilenceVR
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.Astien.SignOfSilenceVR";
        public const string PLUGIN_NAME = "SignOfSilenceVR";
        public const string PLUGIN_VERSION = "0.0.1";

        public static string gameExePath = Process.GetCurrentProcess().MainModule.FileName;
        public static string gamePath = Path.GetDirectoryName(gameExePath);

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            new AssetLoader();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            InitSteamVR();
            gameObject.AddComponent<UIPatches>();
        }

        private static void InitSteamVR()
        {
            SteamVR_Actions.PreInitialize();
            SteamVR.Initialize();
            SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;
            VRInputManager MyVRInputManager = new VRInputManager();
        }

        private void Start()
        {
            //CameraManager.SpawnHands();
        }
    }
}
