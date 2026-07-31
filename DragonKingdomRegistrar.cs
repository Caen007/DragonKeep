using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Jotunn.Entities;
using Jotunn.Configs;
using UnityEngine;
using Jotunn.Managers;

namespace DragonKeep
{
    public class DragonKingdomRegistration
    {
        public string PrefabName;
        public string DisplayName;
        public RequirementConfig[] Requirements;
        public string Description;
        public string Category;
        public int Comfort;

        public DragonKingdomRegistration(string prefab, string display, RequirementConfig[] reqs, string desc, string cat, int comfort = 0)
        {
            PrefabName = prefab;
            DisplayName = display;
            Requirements = reqs;
            Description = desc;
            Category = cat;
            Comfort = comfort;
        }
    }

    public static class DragonKingdomRegistrar
    {
        private static bool wasAlreadyRegistered = false; // HOTFIX stays untouched

        public static readonly List<DragonKingdomRegistration> AllRegistrations = new()
        {
            // ---------------------- Dragon Kingdom ----------------------

            // 1.
            new DragonKingdomRegistration("DP_DragonPen","Dragon Pen", new[]{
                new RequirementConfig("Iron",400), new RequirementConfig("RoundLog",400), new RequirementConfig("Obsidian",400), new RequirementConfig("Coins",5000)
            },"","dragonkingdom"),

            // 2.
            new DragonKingdomRegistration("DP_DragonPen_Base","Dragon Pen no roof", new[]{
                new RequirementConfig("Iron",400), new RequirementConfig("RoundLog",400), new RequirementConfig("Obsidian",400), new RequirementConfig("Coins",4000)
            },"","dragonkingdom"),

            // 3.
            new DragonKingdomRegistration("DP_Roof","Dragon Pen roof", new[]{
                new RequirementConfig("Bronze",40), new RequirementConfig("FineWood",100), new RequirementConfig("Crystal",200), new RequirementConfig("Resin",400)
            },"","dragonkingdom"),

            // 4.
          //  new DragonKingdomRegistration("DP_Long_Wall","Dragon Pen Long Wall", new[]{
            //    new RequirementConfig("Iron",50), new RequirementConfig("Wood",100), new RequirementConfig("Resin",40)
           // },"","dragonkingdom"),

            // 5.
            new DragonKingdomRegistration("DP_Original_Wall","Dragon Pen Wall", new[]{
                new RequirementConfig("Obsidian",8), new RequirementConfig("Stone",8)
            },"","dragonkingdom"),

            // 6.
            new DragonKingdomRegistration("DP_Original_High_Wall","Dragon Pen Original High Wall", new[]{
                new RequirementConfig("Obsidian",20), new RequirementConfig("Stone",20), new RequirementConfig("Iron",20), new RequirementConfig("RoundLog",20)
            },"","dragonkingdom"),

            // 7.
            new DragonKingdomRegistration("DP_6m_Wall","Dragon Pen 6m Wall", new[]{
                new RequirementConfig("Obsidian",20), new RequirementConfig("Stone",20)
            },"","dragonkingdom"),

            // 8.
            new DragonKingdomRegistration("DP_2m_Wall","Dragon Pen Short Wall", new[]{
                new RequirementConfig("Obsidian",2), new RequirementConfig("Stone",2)
            },"","dragonkingdom"),

            // 9.
            new DragonKingdomRegistration("DP_Castle_Tower","Dragon Pen Castle Tower", new[]{
                new RequirementConfig("Obsidian",100), new RequirementConfig("Stone",100), new RequirementConfig("Crystal",10), new RequirementConfig("Coins",210)
            },"","dragonkingdom"),

            // 10.
            new DragonKingdomRegistration("DP_Corner_Tower","Dragon Pen Corner Tower", new[]{
                new RequirementConfig("Iron",50), new RequirementConfig("Stone",50), new RequirementConfig("RoundLog",50), new RequirementConfig("Coins",150)
            },"","dragonkingdom"),

                        // 11.
            new DragonKingdomRegistration("DP_Corner_Piece","Dragon Pen Corner Piece", new[]{
                new RequirementConfig("Obsidian",10)
            },"","dragonkingdom"),

                                    // 12.
            new DragonKingdomRegistration("DP_Corner_Piece_Small","Dragon Pen Small Corner Piece", new[]{
                new RequirementConfig("Obsidian",5)
            },"","dragonkingdom"),

                                                // 13.
            new DragonKingdomRegistration("DP_Gate","Dragon Pen Main Gate", new[]{
                new RequirementConfig("Obsidian",50), new RequirementConfig("Iron",100), new RequirementConfig("RoundLog",100), new RequirementConfig("Coins",250)
            },"","dragonkingdom"),

                          // 14.
            new DragonKingdomRegistration("DP_Throne","Dragon Throne", new[]{
                new RequirementConfig("TrophyDragonQueen",1), new RequirementConfig("TrophySerpent",1), new RequirementConfig("Bronze",50), new RequirementConfig("Coins",500)
            },"","dragonkingdom"),

                                      // 15.
            new DragonKingdomRegistration("DP_B_Throne","Dragon Throne Large", new[]{
                new RequirementConfig("TrophyDragonQueen",1), new RequirementConfig("TrophyFader",1), new RequirementConfig("Bronze",200), new RequirementConfig("Coins",2000)
            },"","dragonkingdom"),
        };

        public static IEnumerable<string> GetAllCategories() =>
            AllRegistrations.Select(r => CategoryToTab(r.Category)).Distinct();

        private static string CategoryToTab(string category) =>
            category.ToLower() switch
            {
                "dragonkingdom" => "Dragon Keep",
                _ => category
            };

        public static void RegisterAllPieces(AssetBundle bundle)
        {
            if (wasAlreadyRegistered) return;

            HashSet<string> registeredPrefabNames = new HashSet<string>();

            foreach (var reg in AllRegistrations)
            {
                RegisterPiece(bundle, reg);
                registeredPrefabNames.Add(reg.PrefabName);
            }

            foreach (GameObject prefab in bundle.LoadAllAssets<GameObject>())
            {
                if (prefab == null || !prefab.name.StartsWith("DP_") || registeredPrefabNames.Contains(prefab.name))
                {
                    continue;
                }

                string displayName = prefab.name.Substring(3).Replace("_", " ");

                DragonKingdomRegistration registration = new DragonKingdomRegistration(prefab.name, displayName, new[]{
                    new RequirementConfig("Iron",50), new RequirementConfig("Wood",100), new RequirementConfig("Resin",40)
                }, "", "dragonkingdom");

                RegisterPiece(bundle, registration);
                registeredPrefabNames.Add(prefab.name);
            }

            wasAlreadyRegistered = true; // HOTFIX preserved
        }

        private static void RegisterPiece(AssetBundle bundle, DragonKingdomRegistration reg)
        {
            if (bundle == null) return;
            GameObject prefab = bundle.LoadAsset<GameObject>(reg.PrefabName);
            if (prefab == null) return;

            prefab.name = reg.PrefabName;
            RequirementConfig[] configuredRequirements = GetConfiguredRequirements(reg);

            var znv = prefab.GetComponent<ZNetView>() ?? prefab.AddComponent<ZNetView>();
            znv.m_persistent = true;
            znv.m_syncInitialScale = true;

            PrepareEternalBuildingPrefab(prefab);

            Piece piece = prefab.GetComponent<Piece>() ?? prefab.AddComponent<Piece>();
            piece.m_name = reg.DisplayName;
            piece.m_description = reg.Description;
            piece.m_groundOnly = false;
            piece.m_canBeRemoved = true;

            GameObject vfxPlace = ZNetScene.instance?.GetPrefab("vfx_Place_stone");
            GameObject sfxPlace = ZNetScene.instance?.GetPrefab("sfx_build_hammer_stone");
            GameObject destroyVFX = ZNetScene.instance?.GetPrefab("vfx_destroyed");
            GameObject destroySFX = ZNetScene.instance?.GetPrefab("sfx_rock_destroyed");

            var placeFX = new EffectList();
            var placeList = new List<EffectList.EffectData>();
            if (vfxPlace != null) placeList.Add(new EffectList.EffectData { m_prefab = vfxPlace });
            if (sfxPlace != null) placeList.Add(new EffectList.EffectData { m_prefab = sfxPlace });
            placeFX.m_effectPrefabs = placeList.ToArray();
            piece.m_placeEffect = placeFX;

            WearNTear wear = prefab.GetComponent<WearNTear>();
            if (wear != null)
            {
                wear.m_health = 1000000f;
                wear.m_noRoofWear = true;
            }

            Destructible destructible = prefab.GetComponent<Destructible>();
            if (destructible != null)
            {
                destructible.m_health = 1000000f;
            }

            TreeBase treeBase = prefab.GetComponent<TreeBase>();
            if (treeBase != null)
            {
                treeBase.m_health = 1000000f;
                treeBase.m_minToolTier = 1073741823;
            }

            var destroyFX = new EffectList();
            var destroyList = new List<EffectList.EffectData>();
            if (destroyVFX != null) destroyList.Add(new EffectList.EffectData { m_prefab = destroyVFX });
            if (destroySFX != null) destroyList.Add(new EffectList.EffectData { m_prefab = destroySFX });
            destroyFX.m_effectPrefabs = destroyList.ToArray();

            DragonKeepRemovalEffects removalEffects = prefab.GetComponent<DragonKeepRemovalEffects>() ?? prefab.AddComponent<DragonKeepRemovalEffects>();
            removalEffects.m_destroyedEffect = destroyFX;

            if (wear != null)
                wear.m_destroyedEffect = destroyFX;

            if (reg.Comfort > 0)
                piece.m_comfort = reg.Comfort;

            Sprite icon = bundle.LoadAsset<Sprite>(reg.PrefabName);
            if (icon != null)
                piece.m_icon = icon;
            else
                Debug.LogWarning($"[DragonKeep] Icon not found for prefab: {reg.PrefabName}");

            if (reg.PrefabName == "DP_DragonPen")
            {
                ConfigureDragonPenDoors(prefab, znv, bundle, true, true);
            }
            else if (reg.PrefabName == "DP_DragonPen_Base")
            {
                ConfigureDragonPenDoors(prefab, znv, bundle, true, false);
            }
            else if (reg.PrefabName == "DP_Roof")
            {
                ConfigureDragonPenDoors(prefab, znv, bundle, false, true);
            }
            else if (reg.PrefabName == "DP_Corner_Tower")
            {
                ConfigureCornerTowerDoors(prefab, znv, bundle);
            }

            var config = new PieceConfig
            {
                PieceTable = "Hammer",
                Category = CategoryToTab(reg.Category),
                CraftingStation = "piece_workbench",
                Requirements = configuredRequirements
            };

            PieceManager.Instance.AddPiece(new CustomPiece(prefab, true, config));
        }

        private static RequirementConfig[] GetConfiguredRequirements(DragonKingdomRegistration reg)
        {
            if (DragonKeep.ModConfig == null)
            {
                return reg.Requirements;
            }

            string defaultValue = FormatRequirements(reg.Requirements);
            ConfigEntry<string> configuredValue = DragonKeep.ModConfig.Bind(
                "Build Costs",
                reg.PrefabName,
                defaultValue,
                "Build cost format: [Valheim prefab name][amount]. Up to four requirements.");

            if (TryParseRequirements(configuredValue.Value, out RequirementConfig[] configuredRequirements))
            {
                return configuredRequirements;
            }

            Debug.LogWarning($"[DragonKeep] Invalid build cost for {reg.PrefabName}. Using the default value: {defaultValue}");
            return reg.Requirements;
        }

        private static string FormatRequirements(RequirementConfig[] requirements)
        {
            if (requirements == null || requirements.Length == 0)
            {
                return "";
            }

            return string.Join("", requirements.Select(requirement => $"[{requirement.Item}][{requirement.Amount}]"));
        }

        private static bool TryParseRequirements(string value, out RequirementConfig[] requirements)
        {
            List<RequirementConfig> parsedRequirements = new List<RequirementConfig>();
            requirements = new RequirementConfig[0];

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            int index = 0;

            while (index < value.Length)
            {
                while (index < value.Length && char.IsWhiteSpace(value[index]))
                {
                    index++;
                }

                if (index >= value.Length)
                {
                    break;
                }

                if (value[index] != '[')
                {
                    return false;
                }

                int itemEnd = value.IndexOf(']', index + 1);
                if (itemEnd < 0)
                {
                    return false;
                }

                string item = value.Substring(index + 1, itemEnd - index - 1).Trim();
                index = itemEnd + 1;

                while (index < value.Length && char.IsWhiteSpace(value[index]))
                {
                    index++;
                }

                if (index >= value.Length || value[index] != '[')
                {
                    return false;
                }

                int amountEnd = value.IndexOf(']', index + 1);
                if (amountEnd < 0)
                {
                    return false;
                }

                string amountText = value.Substring(index + 1, amountEnd - index - 1).Trim();
                if (string.IsNullOrWhiteSpace(item) ||
                    !int.TryParse(amountText, out int amount) ||
                    amount <= 0 ||
                    parsedRequirements.Count >= 4)
                {
                    return false;
                }

                parsedRequirements.Add(new RequirementConfig(item, amount));
                index = amountEnd + 1;
            }

            if (parsedRequirements.Count == 0)
            {
                return false;
            }

            requirements = parsedRequirements.ToArray();
            return true;
        }

        private static void PrepareEternalBuildingPrefab(GameObject prefab)
        {
            HashSet<GameObject> configuredSupportColliders = CaptureConfiguredSupportColliders(prefab);

            EnsureSolidColliders(prefab);
            RemoveComponentsInChildren<Destructible>(prefab);
            RemoveComponentsInChildren<TreeLog>(prefab);
            RemoveComponentsInChildren<Plant>(prefab);
            RemoveComponentsInChildren<WearNTear>(prefab);
            RemoveComponentsInChildren<Rigidbody>(prefab);
            RemoveComponentsInChildren<TreeBase>(prefab);
            RemoveComponentsInChildren<StaticPhysics>(prefab);
            RemoveComponentsInChildren<ZSyncTransform>(prefab);

            SetLayerRecursively(prefab, "piece");
            ApplyEternalSupportColliderLayer(prefab, configuredSupportColliders);
        }

        private static HashSet<GameObject> CaptureConfiguredSupportColliders(GameObject prefab)
        {
            HashSet<GameObject> supportColliders = new HashSet<GameObject>();
            if (prefab == null)
            {
                return supportColliders;
            }

            int staticSolidLayer = LayerMask.NameToLayer("static_solid");
            if (staticSolidLayer < 0)
            {
                return supportColliders;
            }

            Transform[] transforms = prefab.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform child = transforms[i];
                if (child != null && child.gameObject != prefab && child.gameObject.layer == staticSolidLayer)
                {
                    supportColliders.Add(child.gameObject);
                }
            }

            return supportColliders;
        }

        private static void ApplyEternalSupportColliderLayer(GameObject prefab, HashSet<GameObject> configuredSupportColliders)
        {
            if (prefab == null)
            {
                return;
            }

            int staticSolidLayer = LayerMask.NameToLayer("static_solid");
            if (staticSolidLayer < 0)
            {
                return;
            }

            if (configuredSupportColliders != null)
            {
                foreach (GameObject supportCollider in configuredSupportColliders)
                {
                    if (supportCollider == null)
                    {
                        continue;
                    }

                    SetLayerRecursively(supportCollider, "static_solid");
                    DisableRenderers(supportCollider);
                    EnsureColliderTree(supportCollider);
                }
            }

            Transform[] transforms = prefab.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform child = transforms[i];
                if (child == null || child.gameObject == prefab)
                {
                    continue;
                }

                string childName = child.name;
                bool supportCollider = string.Equals(childName, "Collider", System.StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(childName, "SupportCollider", System.StringComparison.OrdinalIgnoreCase) ||
                                       childName.IndexOf("support", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                                       childName.IndexOf("collider", System.StringComparison.OrdinalIgnoreCase) >= 0;

                if (!supportCollider)
                {
                    continue;
                }

                SetLayerRecursively(child.gameObject, "static_solid");
                DisableRenderers(child.gameObject);
                EnsureColliderTree(child.gameObject);
            }
        }

        private static void DisableRenderers(GameObject root)
        {
            if (root == null)
            {
                return;
            }

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].enabled = false;
                }
            }
        }

        private static void EnsureColliderTree(GameObject root)
        {
            if (root == null)
            {
                return;
            }

            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider == null)
                {
                    continue;
                }

                collider.enabled = true;
                collider.isTrigger = false;
            }
        }

        private static void EnsureSolidColliders(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);
            if (colliders != null && colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    Collider collider = colliders[i];
                    if (collider == null)
                    {
                        continue;
                    }

                    collider.enabled = true;
                    collider.isTrigger = false;
                }

                return;
            }

            BoxCollider box = prefab.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = prefab.AddComponent<BoxCollider>();
            }

            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            box.center = prefab.transform.InverseTransformPoint(bounds.center);
            box.size = bounds.size;
            box.enabled = true;
            box.isTrigger = false;
        }

        private static void SetLayerRecursively(GameObject root, string layerName)
        {
            if (root == null || string.IsNullOrWhiteSpace(layerName))
            {
                return;
            }

            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
            {
                return;
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child != null)
                {
                    child.gameObject.layer = layer;
                }
            }
        }

        private static void RemoveComponentsInChildren<T>(GameObject root) where T : Component
        {
            if (root == null)
            {
                return;
            }

            T[] components = root.GetComponentsInChildren<T>(true);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    UnityEngine.Object.DestroyImmediate(components[i], true);
                }
            }
        }

        private static void ConfigureDragonPenDoors(GameObject prefab, ZNetView rootZNetView, AssetBundle bundle, bool configureGroundDoors, bool configureRoofDoor)
        {
            if (prefab == null || rootZNetView == null) return;

            foreach (ZNetView childZNetView in prefab.GetComponentsInChildren<ZNetView>(true))
            {
                if (childZNetView != null && childZNetView != rootZNetView)
                {
                    UnityEngine.Object.DestroyImmediate(childZNetView, true);
                }
            }

            GameObject mainGateOpenSfx = bundle != null ? bundle.LoadAsset<GameObject>("sfx_M_Dragongate_Open") : null;
            GameObject mainGateCloseSfx = bundle != null ? bundle.LoadAsset<GameObject>("sfx_M_Dragongate_Close") : null;
            GameObject smallDoorOpenSfx = bundle != null ? bundle.LoadAsset<GameObject>("sfx_door_open") : null;
            GameObject smallDoorCloseSfx = bundle != null ? bundle.LoadAsset<GameObject>("sfx_door_close") : null;

            if (configureGroundDoors && mainGateOpenSfx == null)
            {
                Debug.LogWarning("[DragonKeep] Missing DragonPen main gate open SFX prefab: sfx_M_Dragongate_Open");
            }

            if (configureGroundDoors && mainGateCloseSfx == null)
            {
                Debug.LogWarning("[DragonKeep] Missing DragonPen main gate close SFX prefab: sfx_M_Dragongate_Close");
            }

            if ((configureGroundDoors || configureRoofDoor) && smallDoorOpenSfx == null)
            {
                Debug.LogWarning("[DragonKeep] Missing DragonPen small door open SFX prefab: sfx_door_open");
            }

            if ((configureGroundDoors || configureRoofDoor) && smallDoorCloseSfx == null)
            {
                Debug.LogWarning("[DragonKeep] Missing DragonPen small door close SFX prefab: sfx_door_close");
            }

            if (configureGroundDoors)
            {
                string[] doorNames =
                {
                    "MainGate",
                    "Left_F",
                    "Right_F",
                    "Left_B",
                    "Right_B",
                    "Left_F_B",
                    "Right_F_B",
                    "Left_B_B",
                    "Right_B_B"
                };

                foreach (string doorName in doorNames)
                {
                    Transform doorTransform = FindDeepChild(prefab.transform, doorName);
                    if (doorTransform == null)
                    {
                        Debug.LogWarning($"[DragonKeep] DragonPen child door not found: {doorName}");
                        continue;
                    }

                    if (doorName == "MainGate")
                    {
                        ConfigureChildDoor(doorTransform.gameObject, rootZNetView, doorName, mainGateOpenSfx, mainGateCloseSfx);
                    }
                    else
                    {
                        ConfigureChildDoor(doorTransform.gameObject, rootZNetView, doorName, smallDoorOpenSfx, smallDoorCloseSfx);
                    }
                }
            }

            if (configureRoofDoor)
            {
                Transform roofDoorTransform = FindDeepChild(prefab.transform, "Roof_Door");
                if (roofDoorTransform == null)
                {
                    Debug.LogWarning("[DragonKeep] DragonPen roof door not found: Roof_Door");
                }
                else
                {
                    CustomChildDoor roofDoor = ConfigureChildDoor(roofDoorTransform.gameObject, rootZNetView, "Roof_Door", smallDoorOpenSfx, smallDoorCloseSfx);

                    if (roofDoor != null)
                    {
                        roofDoor.m_name = "Roof Door";
                        roofDoor.m_checkGuardStone = false;
                        roofDoor.m_disableManualInteract = true;
                    }

                    RoofDoorAutoOpenerInstaller.Apply(roofDoorTransform.gameObject, rootZNetView);
                }
            }
        }

        private static CustomChildDoor ConfigureChildDoor(GameObject doorObject, ZNetView rootZNetView, string doorID, GameObject openSfxPrefab, GameObject closeSfxPrefab)
        {
            if (doorObject == null || rootZNetView == null) return null;

            Door sourceDoor = doorObject.GetComponent<Door>();
            if (sourceDoor == null)
            {
                sourceDoor = doorObject.GetComponentInChildren<Door>(true);
            }

            CustomChildDoor customDoor = doorObject.GetComponent<CustomChildDoor>();
            if (customDoor == null)
            {
                customDoor = doorObject.AddComponent<CustomChildDoor>();
            }

            customDoor.m_doorID = doorID;
            customDoor.m_rootZNetView = rootZNetView;
            customDoor.m_animator = CustomChildDoor.FindAnimatorComponent(doorObject);
            customDoor.m_disableManualInteract = false;

            if (sourceDoor != null)
            {
                customDoor.m_name = sourceDoor.m_name;
                customDoor.m_keyItem = sourceDoor.m_keyItem;
                customDoor.m_canNotBeClosed = sourceDoor.m_canNotBeClosed;
                customDoor.m_invertedOpenClosedText = sourceDoor.m_invertedOpenClosedText;
                customDoor.m_checkGuardStone = sourceDoor.m_checkGuardStone;
                customDoor.m_openEnable = sourceDoor.m_openEnable;
                customDoor.m_openEffects = sourceDoor.m_openEffects;
                customDoor.m_closeEffects = sourceDoor.m_closeEffects;
                customDoor.m_lockedEffects = sourceDoor.m_lockedEffects;
            }
            else
            {
                customDoor.m_name = doorObject.name;
            }

            if (openSfxPrefab != null)
            {
                customDoor.m_openEffects = CreateSingleEffectList(openSfxPrefab);
            }

            if (closeSfxPrefab != null)
            {
                customDoor.m_closeEffects = CreateSingleEffectList(closeSfxPrefab);
            }

            foreach (Door vanillaDoor in doorObject.GetComponentsInChildren<Door>(true))
            {
                if (vanillaDoor != null)
                {
                    UnityEngine.Object.DestroyImmediate(vanillaDoor, true);
                }
            }

            CustomDoorSound doorSound = doorObject.GetComponent<CustomDoorSound>();
            if (doorSound == null)
            {
                doorSound = doorObject.AddComponent<CustomDoorSound>();
            }

            doorSound.Configure(customDoor.m_openEffects, customDoor.m_closeEffects);
            customDoor.SetDoorSound(doorSound);
            customDoor.InitializeAfterConfiguration();

            return customDoor;
        }

        private static void ConfigureCornerTowerDoors(GameObject prefab, ZNetView rootZNetView, AssetBundle bundle)
        {
            if (prefab == null || rootZNetView == null) return;

            foreach (ZNetView childZNetView in prefab.GetComponentsInChildren<ZNetView>(true))
            {
                if (childZNetView != null && childZNetView != rootZNetView)
                {
                    UnityEngine.Object.DestroyImmediate(childZNetView, true);
                }
            }

            GameObject smallDoorOpenSfx = bundle != null ? bundle.LoadAsset<GameObject>("sfx_door_open") : null;
            GameObject smallDoorCloseSfx = bundle != null ? bundle.LoadAsset<GameObject>("sfx_door_close") : null;

            if (smallDoorOpenSfx == null)
            {
                Debug.LogWarning("[DragonKeep] Missing Corner Tower door open SFX prefab: sfx_door_open");
            }

            if (smallDoorCloseSfx == null)
            {
                Debug.LogWarning("[DragonKeep] Missing Corner Tower door close SFX prefab: sfx_door_close");
            }

            string[] doorNames =
            {
                "Left_F",
                "Left_B"
            };

            foreach (string doorName in doorNames)
            {
                Transform doorTransform = FindDeepChild(prefab.transform, doorName);
                if (doorTransform == null)
                {
                    Debug.LogWarning($"[DragonKeep] Corner Tower child door not found: {doorName}");
                    continue;
                }

                ConfigureChildDoor(doorTransform.gameObject, rootZNetView, doorName, smallDoorOpenSfx, smallDoorCloseSfx);
            }
        }

        private static EffectList CreateSingleEffectList(GameObject effectPrefab)
        {
            EffectList effectList = new EffectList();

            if (effectPrefab == null)
            {
                effectList.m_effectPrefabs = new EffectList.EffectData[0];
                return effectList;
            }

            effectList.m_effectPrefabs = new[]
            {
                new EffectList.EffectData
                {
                    m_prefab = effectPrefab
                }
            };

            return effectList;
        }

        private static GameObject GetRegisteredPrefab(string prefabName)
        {
            GameObject prefab = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab(prefabName) : null;

            if (prefab == null && PrefabManager.Instance != null)
            {
                prefab = PrefabManager.Instance.GetPrefab(prefabName);
            }

            return prefab;
        }

        private static Transform FindDeepChild(Transform parent, string childName)
        {
            if (parent == null) return null;

            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                Transform result = FindDeepChild(child, childName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}