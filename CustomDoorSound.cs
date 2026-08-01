using UnityEngine;

namespace DragonKeep
{
    public class CustomDoorSound : MonoBehaviour
    {
        [SerializeField]
        private EffectList m_openEffects = new EffectList();

        [SerializeField]
        private EffectList m_closeEffects = new EffectList();

        private bool m_warnedMissingOpenSfx;

        private bool m_warnedMissingCloseSfx;

        public void Configure(EffectList openEffects, EffectList closeEffects)
        {
            m_openEffects = openEffects ?? new EffectList();
            m_closeEffects = closeEffects ?? new EffectList();
        }

        public void Configure(CustomChildDoor door)
        {
            Configure(door != null ? door.m_openEffects : null, door != null ? door.m_closeEffects : null);
        }

        public void PlayOpen()
        {
            PlayEffects(m_openEffects, true);
        }

        public void PlayClose()
        {
            PlayEffects(m_closeEffects, false);
        }

        private static bool HasEffects(EffectList effects)
        {
            return effects != null &&
                   effects.m_effectPrefabs != null &&
                   effects.m_effectPrefabs.Length > 0;
        }

        private void PlayEffects(EffectList effects, bool opening)
        {
            string action = opening ? "OPEN" : "CLOSE";

            if (!HasEffects(effects))
            {
                WarnMissingSound(opening);
                return;
            }

            if (IsBundledSmallDoorSound(effects))
            {
                PlayBundledSmallDoorClips(effects, action, opening);
                return;
            }

            effects.Create(transform.position, transform.rotation);
        }

        private static bool IsBundledSmallDoorSound(EffectList effects)
        {
            if (!HasEffects(effects))
            {
                return false;
            }

            for (int i = 0; i < effects.m_effectPrefabs.Length; i++)
            {
                EffectList.EffectData effectData = effects.m_effectPrefabs[i];
                if (effectData == null || effectData.m_prefab == null)
                {
                    continue;
                }

                string prefabName = effectData.m_prefab.name;
                if (prefabName == "sfx_door_open" || prefabName == "sfx_door_close")
                {
                    return true;
                }
            }

            return false;
        }

        private void PlayBundledSmallDoorClips(EffectList effects, string action, bool opening)
        {
            bool playedAnyClip = false;

            for (int effectIndex = 0; effectIndex < effects.m_effectPrefabs.Length; effectIndex++)
            {
                EffectList.EffectData effectData = effects.m_effectPrefabs[effectIndex];
                if (effectData == null || !effectData.m_enabled || effectData.m_prefab == null)
                {
                    continue;
                }

                ZSFX[] soundEffects = effectData.m_prefab.GetComponentsInChildren<ZSFX>(true);
                for (int soundIndex = 0; soundIndex < soundEffects.Length; soundIndex++)
                {
                    ZSFX soundEffect = soundEffects[soundIndex];
                    if (soundEffect == null || soundEffect.m_audioClips == null || soundEffect.m_audioClips.Length == 0)
                    {
                        continue;
                    }

                    AudioClip clip = soundEffect.m_audioClips[UnityEngine.Random.Range(0, soundEffect.m_audioClips.Length)];
                    if (clip == null)
                    {
                        continue;
                    }

                    if (clip.loadState == AudioDataLoadState.Unloaded)
                    {
                        clip.LoadAudioData();
                    }

                    GameObject soundObject = new GameObject("DragonKeep_" + action + "_DoorSound");
                    soundObject.transform.position = transform.position;

                    AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    audioSource.spatialBlend = 1f;
                    audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                    audioSource.minDistance = 5f;
                    audioSource.maxDistance = 50f;
                    audioSource.dopplerLevel = 0f;
                    audioSource.volume = 1f;
                    audioSource.pitch = 1f;
                    audioSource.clip = clip;
                    audioSource.Play();

                    float destroyDelay = Mathf.Max(1f, clip.length + 0.25f);
                    Destroy(soundObject, destroyDelay);
                    playedAnyClip = true;
                }
            }

            if (!playedAnyClip)
            {
                WarnMissingSound(opening);
            }
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