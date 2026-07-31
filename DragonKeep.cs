using System.Reflection;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
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
            new Harmony(PluginGUID).PatchAll();

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

    public class DragonKeepRemovalEffects : MonoBehaviour
    {
        public EffectList m_destroyedEffect = new EffectList();

        private bool effectsPlayed;

        public void Play()
        {
            if (effectsPlayed)
            {
                return;
            }

            effectsPlayed = true;
            m_destroyedEffect?.Create(transform.position, transform.rotation);
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Destroy), new[] { typeof(GameObject) })]
    internal static class DragonKeepZNetSceneDestroyPatch
    {
        private static void Prefix(GameObject __0)
        {
            if (__0 == null)
            {
                return;
            }

            DragonKeepRemovalEffects removalEffects = __0.GetComponent<DragonKeepRemovalEffects>();
            removalEffects?.Play();
        }
    }
}