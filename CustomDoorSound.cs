using System;
using System.Reflection;
using System.Text;
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

        private void Awake()
        {
            LogDebug("Awake activeSelf=" + gameObject.activeSelf + " activeInHierarchy=" + gameObject.activeInHierarchy);
            LogEffectList("Awake OPEN", m_openEffects);
            LogEffectList("Awake CLOSE", m_closeEffects);
        }

        public void Configure(EffectList openEffects, EffectList closeEffects)
        {
            m_openEffects = openEffects ?? new EffectList();
            m_closeEffects = closeEffects ?? new EffectList();

            LogDebug("Configure(EffectList, EffectList)");
            LogEffectList("Configured OPEN", m_openEffects);
            LogEffectList("Configured CLOSE", m_closeEffects);
        }

        public void Configure(CustomChildDoor door)
        {
            LogDebug("Configure(CustomChildDoor) door=" + (door != null ? GetObjectPath(door.transform) : "<null>"));
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
            LogDebug("Play" + action + " reached");
            LogEffectList("Play " + action, effects);

            if (!HasEffects(effects))
            {
                WarnMissingSound(opening);
                return;
            }

            try
            {
                if (IsBundledSmallDoorSound(effects))
                {
                    PlayBundledSmallDoorClips(effects, action, opening);
                    return;
                }

                GameObject[] createdEffects = effects.Create(transform.position, transform.rotation);
                int createdCount = createdEffects != null ? createdEffects.Length : -1;
                LogDebug("Play " + action + " EffectList.Create returned " + createdCount + " object(s)");

                if (createdEffects != null)
                {
                    for (int i = 0; i < createdEffects.Length; i++)
                    {
                        LogSoundObject("Spawned " + action + "[" + i + "]", createdEffects[i]);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("[DragonKeep:SFX-DEBUG] " + GetIdentity() + " Play " + action + " threw: " + exception);
                throw;
            }
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

                    LogDebug("Direct bundle clip " + action +
                             " clip=" + clip.name +
                             " loadState=" + clip.loadState +
                             " length=" + clip.length +
                             " position=" + transform.position +
                             " volume=" + audioSource.volume +
                             " pitch=" + audioSource.pitch +
                             " minDistance=" + audioSource.minDistance +
                             " maxDistance=" + audioSource.maxDistance +
                             " isPlaying=" + audioSource.isPlaying);
                }
            }

            if (!playedAnyClip)
            {
                WarnMissingSound(opening);
            }
        }

        private void LogEffectList(string stage, EffectList effects)
        {
            if (effects == null)
            {
                LogDebug(stage + " EffectList=<null>");
                return;
            }

            EffectList.EffectData[] effectPrefabs = effects.m_effectPrefabs;
            int count = effectPrefabs != null ? effectPrefabs.Length : -1;
            LogDebug(stage + " effectCount=" + count);

            if (effectPrefabs == null)
            {
                return;
            }

            for (int i = 0; i < effectPrefabs.Length; i++)
            {
                EffectList.EffectData effectData = effectPrefabs[i];
                if (effectData == null)
                {
                    LogDebug(stage + "[" + i + "]=<null EffectData>");
                    continue;
                }

                LogDebug(stage + "[" + i + "] enabled=" + effectData.m_enabled +
                         " variant=" + effectData.m_variant +
                         " attach=" + effectData.m_attach +
                         " follow=" + effectData.m_follow +
                         " prefab=" + (effectData.m_prefab != null ? effectData.m_prefab.name : "<null>"));
                LogSoundObject(stage + " source[" + i + "]", effectData.m_prefab);
            }
        }

        private void LogSoundObject(string stage, GameObject soundObject)
        {
            if (soundObject == null)
            {
                LogDebug(stage + " object=<null>");
                return;
            }

            Component[] components = soundObject.GetComponentsInChildren<Component>(true);
            AudioSource[] audioSources = soundObject.GetComponentsInChildren<AudioSource>(true);
            StringBuilder componentNames = new StringBuilder();

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    continue;
                }

                if (componentNames.Length > 0)
                {
                    componentNames.Append(", ");
                }

                componentNames.Append(component.GetType().FullName);
                LogAudioClipFields(stage, component);
            }

            LogDebug(stage + " object=" + GetObjectPath(soundObject.transform) +
                     " activeSelf=" + soundObject.activeSelf +
                     " activeInHierarchy=" + soundObject.activeInHierarchy +
                     " audioSourceCount=" + audioSources.Length +
                     " components=[" + componentNames + "]");

            for (int i = 0; i < audioSources.Length; i++)
            {
                AudioSource audioSource = audioSources[i];
                LogDebug(stage + " AudioSource[" + i + "] object=" + GetObjectPath(audioSource.transform) +
                         " enabled=" + audioSource.enabled +
                         " clip=" + (audioSource.clip != null ? audioSource.clip.name : "<null>") +
                         " playOnAwake=" + audioSource.playOnAwake +
                         " mute=" + audioSource.mute +
                         " loop=" + audioSource.loop +
                         " volume=" + audioSource.volume +
                         " spatialBlend=" + audioSource.spatialBlend +
                         " isPlaying=" + audioSource.isPlaying);
            }
        }

        private void LogAudioClipFields(string stage, Component component)
        {
            FieldInfo[] fields;

            try
            {
                fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch (Exception exception)
            {
                LogDebug(stage + " could not inspect " + component.GetType().FullName + ": " + exception.Message);
                return;
            }

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                try
                {
                    if (field.FieldType == typeof(AudioClip))
                    {
                        AudioClip clip = field.GetValue(component) as AudioClip;
                        LogDebug(stage + " " + component.GetType().Name + "." + field.Name +
                                 "=" + (clip != null ? clip.name : "<null>"));
                    }
                    else if (field.FieldType == typeof(AudioClip[]))
                    {
                        AudioClip[] clips = field.GetValue(component) as AudioClip[];
                        int clipCount = clips != null ? clips.Length : -1;
                        StringBuilder clipNames = new StringBuilder();

                        if (clips != null)
                        {
                            for (int clipIndex = 0; clipIndex < clips.Length; clipIndex++)
                            {
                                if (clipNames.Length > 0)
                                {
                                    clipNames.Append(", ");
                                }

                                clipNames.Append(clips[clipIndex] != null ? clips[clipIndex].name : "<null>");
                            }
                        }

                        LogDebug(stage + " " + component.GetType().Name + "." + field.Name +
                                 " count=" + clipCount + " clips=[" + clipNames + "]");
                    }
                }
                catch (Exception exception)
                {
                    LogDebug(stage + " could not read " + component.GetType().Name + "." + field.Name +
                             ": " + exception.Message);
                }
            }
        }

        private void LogDebug(string message)
        {
            Debug.Log("[DragonKeep:SFX-DEBUG] " + GetIdentity() + " " + message);
        }

        private string GetIdentity()
        {
            return "door=" + GetObjectPath(transform) + " instance=" + GetInstanceID();
        }

        private static string GetObjectPath(Transform target)
        {
            if (target == null)
            {
                return "<null>";
            }

            string path = target.name;
            Transform current = target.parent;

            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
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