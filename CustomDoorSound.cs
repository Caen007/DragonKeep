using Jotunn.Managers;
using UnityEngine;

namespace DragonKeep
{
    public class CustomDoorSound : MonoBehaviour
    {
        private const string MainGateDoorID = "MainGate";

        private static readonly string[] VanillaDoorPrefabNames =
        {
            "wood_door",
            "piece_wood_door",
            "wood_gate",
            "Door"
        };

        private CustomChildDoor m_door;

        private Door m_vanillaDoor;

        private bool m_warnedMissingOpenSfx;

        private bool m_warnedMissingCloseSfx;

        public void Configure(CustomChildDoor door)
        {
            if (door != null)
            {
                m_door = door;
            }
        }

        public void PlayOpen()
        {
            PlayEffects(GetOpenEffects(), true);
        }

        public void PlayClose()
        {
            PlayEffects(GetCloseEffects(), false);
        }

        private EffectList GetOpenEffects()
        {
            if (!IsMainGate())
            {
                Door vanillaDoor = GetVanillaDoor();
                if (vanillaDoor != null && HasEffects(vanillaDoor.m_openEffects))
                {
                    return vanillaDoor.m_openEffects;
                }
            }

            return m_door != null ? m_door.m_openEffects : null;
        }

        private EffectList GetCloseEffects()
        {
            if (!IsMainGate())
            {
                Door vanillaDoor = GetVanillaDoor();
                if (vanillaDoor != null && HasEffects(vanillaDoor.m_closeEffects))
                {
                    return vanillaDoor.m_closeEffects;
                }
            }

            return m_door != null ? m_door.m_closeEffects : null;
        }

        private Door GetVanillaDoor()
        {
            if (m_vanillaDoor != null)
            {
                return m_vanillaDoor;
            }

            if (ZNetScene.instance == null && PrefabManager.Instance == null)
            {
                return null;
            }

            for (int i = 0; i < VanillaDoorPrefabNames.Length; i++)
            {
                GameObject prefab = ZNetScene.instance != null
                    ? ZNetScene.instance.GetPrefab(VanillaDoorPrefabNames[i])
                    : null;

                if (prefab == null && PrefabManager.Instance != null)
                {
                    prefab = PrefabManager.Instance.GetPrefab(VanillaDoorPrefabNames[i]);
                }

                if (prefab == null)
                {
                    continue;
                }

                m_vanillaDoor = prefab.GetComponent<Door>();
                if (m_vanillaDoor == null)
                {
                    m_vanillaDoor = prefab.GetComponentInChildren<Door>(true);
                }

                if (m_vanillaDoor != null)
                {
                    return m_vanillaDoor;
                }
            }

            return null;
        }

        private bool IsMainGate()
        {
            return m_door != null && m_door.m_doorID == MainGateDoorID;
        }

        private static bool HasEffects(EffectList effects)
        {
            return effects != null &&
                   effects.m_effectPrefabs != null &&
                   effects.m_effectPrefabs.Length > 0;
        }

        private void PlayEffects(EffectList effects, bool opening)
        {
            if (!HasEffects(effects))
            {
                WarnMissingSound(opening);
                return;
            }

            effects.Create(transform.position, transform.rotation, transform);
        }

        private void WarnMissingSound(bool opening)
        {
            if (opening)
            {
                if (m_warnedMissingOpenSfx)
                {
                    return;
                }

                m_warnedMissingOpenSfx = true;
                Debug.LogWarning("[DragonKeep] Door open SFX was not found for: " + name);
                return;
            }

            if (m_warnedMissingCloseSfx)
            {
                return;
            }

            m_warnedMissingCloseSfx = true;
            Debug.LogWarning("[DragonKeep] Door close SFX was not found for: " + name);
        }
    }
}