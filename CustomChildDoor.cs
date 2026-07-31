using System;
using System.Reflection;
using UnityEngine;

namespace DragonKeep
{
    public class CustomChildDoor : MonoBehaviour, Hoverable, Interactable
    {
        public string m_name = "door";

        public string m_doorID = "";

        public ItemDrop m_keyItem;

        public bool m_canNotBeClosed;

        public bool m_invertedOpenClosedText;

        public bool m_checkGuardStone = true;

        public bool m_disableManualInteract;

        public GameObject m_openEnable;

        public EffectList m_openEffects = new EffectList();

        public EffectList m_closeEffects = new EffectList();

        public EffectList m_lockedEffects = new EffectList();

        public ZNetView m_rootZNetView;

        public Component m_animator;

        public string m_animatorStateParameter = "state";

        private ZNetView m_nview;

        private CustomDoorSound m_doorSound;

        private uint m_lastDataRevision = uint.MaxValue;

        private string m_stateKey;

        private string m_rpcName;

        private string m_autoRpcName;

        private string m_registeredRpcName;

        private string m_registeredAutoRpcName;

        private static Type localizationType;

        private static FieldInfo localizationInstanceField;

        private static MethodInfo localizationLocalizeMethod;

        private void Awake()
        {
            if (m_rootZNetView != null && !string.IsNullOrEmpty(m_doorID))
            {
                InitializeAfterConfiguration();
            }
        }

        public void InitializeAfterConfiguration()
        {
            if (string.IsNullOrEmpty(m_doorID))
            {
                m_doorID = BuildDoorID();
            }

            m_stateKey = "CustomChildDoor_" + m_doorID + "_state";
            m_rpcName = "CustomChildDoor_" + m_doorID + "_UseDoor";
            m_autoRpcName = "CustomChildDoor_" + m_doorID + "_AutoSetDoor";

            if (m_animator == null)
            {
                m_animator = FindAnimatorComponent(gameObject);
            }

            ZNetView resolvedZNetView = m_rootZNetView != null ? m_rootZNetView : GetComponentInParent<ZNetView>();

            if (m_nview != resolvedZNetView)
            {
                m_nview = resolvedZNetView;
                m_registeredRpcName = null;
                m_registeredAutoRpcName = null;
            }

            if (m_nview == null)
            {
                Debug.LogWarning("[CustomChildDoor] No root ZNetView found for door: " + name);
                return;
            }

            if (m_registeredRpcName != m_rpcName)
            {
                m_nview.Register<bool>(m_rpcName, RPC_UseDoor);
                m_registeredRpcName = m_rpcName;
            }

            if (m_registeredAutoRpcName != m_autoRpcName)
            {
                m_nview.Register<int>(m_autoRpcName, RPC_AutoSetDoor);
                m_registeredAutoRpcName = m_autoRpcName;
            }

            if (!IsInvoking(nameof(UpdateState)))
            {
                InvokeRepeating(nameof(UpdateState), 0f, 0.2f);
            }
        }

        private string BuildDoorID()
        {
            string path = name;
            Transform current = transform.parent;

            while (current != null)
            {
                path = current.name + "_" + path;

                if (current.GetComponent<ZNetView>() != null)
                {
                    break;
                }

                current = current.parent;
            }

            return SanitizeKey(path);
        }

        private string SanitizeKey(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "Door";
            }

            return value
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace("(", "_")
                .Replace(")", "_")
                .Replace("[", "_")
                .Replace("]", "_")
                .Replace(".", "_")
                .Replace("-", "_");
        }

        private bool IsValid()
        {
            return m_nview != null && m_nview.IsValid() && m_nview.GetZDO() != null;
        }

        private int GetState()
        {
            if (!IsValid())
            {
                return 0;
            }

            return m_nview.GetZDO().GetInt(m_stateKey, 0);
        }

        private void SetSavedState(int state)
        {
            if (!IsValid())
            {
                return;
            }

            m_nview.GetZDO().Set(m_stateKey, state);
        }

        private void UpdateState()
        {
            if (!IsValid())
            {
                return;
            }

            uint dataRevision = m_nview.GetZDO().DataRevision;

            if (m_lastDataRevision != dataRevision)
            {
                m_lastDataRevision = dataRevision;
                SetState(GetState());
            }
        }

        private void SetState(int state)
        {
            if (m_animator != null)
            {
                int currentState = GetAnimatorInteger(m_animator, m_animatorStateParameter);

                if (currentState != state)
                {
                    if (state != 0)
                    {
                        GetDoorSound().PlayOpen();
                    }
                    else
                    {
                        GetDoorSound().PlayClose();
                    }

                    SetAnimatorInteger(m_animator, m_animatorStateParameter, state);
                }
            }

            if (m_openEnable != null)
            {
                m_openEnable.SetActive(state != 0);
            }
        }

        public void SetDoorSound(CustomDoorSound doorSound)
        {
            m_doorSound = doorSound;

            if (m_doorSound != null)
            {
                m_doorSound.Configure(this);
            }
        }

        private CustomDoorSound GetDoorSound()
        {
            if (m_doorSound == null)
            {
                m_doorSound = GetComponent<CustomDoorSound>();
            }

            if (m_doorSound == null)
            {
                m_doorSound = gameObject.AddComponent<CustomDoorSound>();
            }

            m_doorSound.Configure(this);
            return m_doorSound;
        }

        private bool CanInteract()
        {
            if (!IsValid())
            {
                return false;
            }

            if (m_animator == null)
            {
                return false;
            }

            if ((m_keyItem != null || m_canNotBeClosed) && GetState() != 0)
            {
                return false;
            }

            return !IsAnimatorInTransition(m_animator);
        }

        public void AutoSetDoorState(bool open)
        {
            if (!IsValid())
            {
                return;
            }

            int wantedState = open ? 1 : 0;

            if (GetState() == wantedState)
            {
                return;
            }

            if (string.IsNullOrEmpty(m_autoRpcName))
            {
                m_autoRpcName = "CustomChildDoor_" + m_doorID + "_AutoSetDoor";
            }

            m_nview.InvokeRPC(m_autoRpcName, wantedState);
        }

        private void RPC_AutoSetDoor(long uid, int wantedState)
        {
            if (!IsValid())
            {
                return;
            }

            wantedState = wantedState != 0 ? 1 : 0;

            if (GetState() == wantedState)
            {
                return;
            }

            SetSavedState(wantedState);
            UpdateState();
        }

        public string GetHoverText()
        {
            if (m_disableManualInteract)
            {
                return "";
            }

            string cleanName = GetCleanDoorName();

            if (!IsValid())
            {
                return "";
            }

            if (m_canNotBeClosed && !CanInteract())
            {
                return "";
            }

            if (m_checkGuardStone && !PrivateArea.CheckAccess(transform.position, 0f, false))
            {
                return cleanName + "\n[<color=yellow><b>E</b></color>] No access";
            }

            if (CanInteract())
            {
                bool isOpen = GetState() != 0;
                string action;

                if (isOpen)
                {
                    action = m_invertedOpenClosedText ? "Open" : "Close";
                }
                else
                {
                    action = m_invertedOpenClosedText ? "Close" : "Open";
                }

                return cleanName + "\n[<color=yellow><b>E</b></color>] " + action;
            }

            return cleanName;
        }

        private string GetCleanDoorName()
        {
            if (string.IsNullOrEmpty(m_name))
            {
                return CleanDoorName(name);
            }

            return CleanDoorName(m_name);
        }

        private static string CleanDoorName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "Door";
            }

            if (value == "MainGate")
            {
                return "Main Gate";
            }

            if (value == "Roof_Door")
            {
                return "Roof Door";
            }

            return value.Replace("_", " ");
        }

        public string GetHoverName()
        {
            if (m_disableManualInteract)
            {
                return "";
            }

            return GetCleanDoorName();
        }

        public bool Interact(Humanoid character, bool hold, bool alt)
        {
            if (m_disableManualInteract)
            {
                return false;
            }

            if (hold)
            {
                return false;
            }

            if (!CanInteract())
            {
                return false;
            }

            if (m_checkGuardStone && !PrivateArea.CheckAccess(transform.position))
            {
                return true;
            }

            if (m_keyItem != null)
            {
                if (!HaveKey(character))
                {
                    m_lockedEffects.Create(transform.position, transform.rotation);

                    if (Game.m_worldLevel > 0 && HaveKey(character, false))
                    {
                        character.Message(MessageHud.MessageType.Center, Localize("$msg_ng_the_x") + m_keyItem.m_itemData.m_shared.m_name + Localize("$msg_ng_x_is_too_low"));
                    }
                    else
                    {
                        character.Message(MessageHud.MessageType.Center, Localize("$msg_door_needkey", m_keyItem.m_itemData.m_shared.m_name));
                    }

                    return true;
                }

                character.Message(MessageHud.MessageType.Center, Localize("$msg_door_usingkey", m_keyItem.m_itemData.m_shared.m_name));
            }

            Vector3 userDirection = (character.transform.position - transform.position).normalized;

            Game.instance.IncrementPlayerStat(GetState() == 0 ? PlayerStatType.DoorsOpened : PlayerStatType.DoorsClosed);

            Open(userDirection);

            return true;
        }

        private void Open(Vector3 userDirection)
        {
            if (!IsValid())
            {
                return;
            }

            bool forward = Vector3.Dot(transform.forward, userDirection) < 0f;
            m_nview.InvokeRPC(m_rpcName, forward);
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            if (m_disableManualInteract)
            {
                return false;
            }

            if (m_keyItem == null)
            {
                return false;
            }

            if (m_keyItem.m_itemData.m_shared.m_name != item.m_shared.m_name)
            {
                return false;
            }

            if (!CanInteract())
            {
                return false;
            }

            if (m_checkGuardStone && !PrivateArea.CheckAccess(transform.position))
            {
                return true;
            }

            user.Message(MessageHud.MessageType.Center, Localize("$msg_door_usingkey", m_keyItem.m_itemData.m_shared.m_name));

            Vector3 userDirection = (user.transform.position - transform.position).normalized;

            Open(userDirection);

            return true;
        }

        private bool HaveKey(Humanoid player, bool matchWorldLevel = true)
        {
            if (m_keyItem == null)
            {
                return true;
            }

            return player.GetInventory().HaveItem(m_keyItem.m_itemData.m_shared.m_name, matchWorldLevel);
        }

        private void RPC_UseDoor(long uid, bool forward)
        {
            if (!CanInteract())
            {
                return;
            }

            int state = GetState();

            if (state == 0)
            {
                SetSavedState(forward ? 1 : -1);
            }
            else
            {
                SetSavedState(0);
            }

            UpdateState();
        }

        public static Component FindAnimatorComponent(GameObject owner)
        {
            if (owner == null)
            {
                return null;
            }

            Component[] components = owner.GetComponentsInChildren<Component>(true);

            foreach (Component component in components)
            {
                if (component == null)
                {
                    continue;
                }

                Type type = component.GetType();

                if (type.Name == "Animator" || type.FullName == "UnityEngine.Animator")
                {
                    return component;
                }
            }

            return null;
        }

        private static int GetAnimatorInteger(Component animator, string parameterName)
        {
            if (animator == null || string.IsNullOrEmpty(parameterName))
            {
                return int.MinValue;
            }

            try
            {
                MethodInfo method = animator.GetType().GetMethod("GetInteger", new[] { typeof(string) });

                if (method == null)
                {
                    return int.MinValue;
                }

                object result = method.Invoke(animator, new object[] { parameterName });

                return result is int value ? value : int.MinValue;
            }
            catch
            {
                return int.MinValue;
            }
        }

        private static void SetAnimatorInteger(Component animator, string parameterName, int value)
        {
            if (animator == null || string.IsNullOrEmpty(parameterName))
            {
                return;
            }

            try
            {
                MethodInfo method = animator.GetType().GetMethod("SetInteger", new[] { typeof(string), typeof(int) });

                if (method != null)
                {
                    method.Invoke(animator, new object[] { parameterName, value });
                }
            }
            catch
            {
            }
        }

        private static bool IsAnimatorInTransition(Component animator)
        {
            if (animator == null)
            {
                return false;
            }

            try
            {
                MethodInfo method = animator.GetType().GetMethod("IsInTransition", new[] { typeof(int) });

                if (method == null)
                {
                    return false;
                }

                object result = method.Invoke(animator, new object[] { 0 });

                return result is bool value && value;
            }
            catch
            {
                return false;
            }
        }

        private static string Localize(string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            try
            {
                EnsureLocalizationReflection();

                object instance = localizationInstanceField != null ? localizationInstanceField.GetValue(null) : null;

                if (instance == null || localizationLocalizeMethod == null)
                {
                    return FormatFallback(text, args);
                }

                if (args != null && args.Length > 0)
                {
                    try
                    {
                        return localizationLocalizeMethod.Invoke(instance, new object[] { text, args }) as string ?? FormatFallback(text, args);
                    }
                    catch
                    {
                        return localizationLocalizeMethod.Invoke(instance, new object[] { text }) as string ?? FormatFallback(text, args);
                    }
                }

                return localizationLocalizeMethod.Invoke(instance, new object[] { text }) as string ?? text;
            }
            catch
            {
                return FormatFallback(text, args);
            }
        }

        private static void EnsureLocalizationReflection()
        {
            if (localizationType != null)
            {
                return;
            }

            Assembly valheimAssembly = typeof(Game).Assembly;
            localizationType = valheimAssembly.GetType("Localization");

            if (localizationType == null)
            {
                return;
            }

            localizationInstanceField = localizationType.GetField("instance", BindingFlags.Public | BindingFlags.Static);

            foreach (MethodInfo method in localizationType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.Name != "Localize")
                {
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length >= 1 && parameters[0].ParameterType == typeof(string))
                {
                    localizationLocalizeMethod = method;
                    return;
                }
            }
        }

        private static string FormatFallback(string text, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return text;
            }

            try
            {
                return string.Format(text, args);
            }
            catch
            {
                return text;
            }
        }
    }
}