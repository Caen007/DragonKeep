using System.Reflection;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Jotunn.Managers;

namespace DragonKeep
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class DragonKeep : BaseUnityPlugin
    {
        public const string PluginGUID = "DragonKeep";
        public const string PluginName = "Dragon Keep";
        public const string PluginVersion = "1.0.0";

        internal static ConfigFile ModConfig;

        private AssetBundle dragonPenBundle;

        private void Awake()
        {
            ModConfig = Config;

            string resourcePath = "DragonKeep.dragonpen";
            dragonPenBundle = EmbeddedAssetBundleLoader.LoadBundle(resourcePath);

            if (dragonPenBundle == null)
            {
                Logger.LogError("Failed to load embedded AssetBundle: dragonpen");
                return;
            }

            foreach (var category in DragonKingdomRegistrar.GetAllCategories())
                PieceManager.Instance.AddPieceCategory(category);

            PrefabManager.OnPrefabsRegistered += () =>
            {
                DragonKingdomRegistrar.RegisterAllPieces(dragonPenBundle);
            };
        }
    }

    public static class EmbeddedAssetBundleLoader
    {
        public static AssetBundle LoadBundle(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    Debug.LogError("AssetBundle resource not found: " + resourcePath);
                    return null;
                }
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return AssetBundle.LoadFromMemory(buffer);
            }
        }
    }
}