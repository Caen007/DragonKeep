using UnityEngine;

namespace DragonKeep
{
    public static class RoofDoorAutoOpenerInstaller
    {
        private const float AutoOpenRadius = 10f;
        private const float AutoCloseDelay = 7f;

        public static void Apply(GameObject roofDoorObject, ZNetView rootZNetView)
        {
            if (roofDoorObject == null)
            {
                return;
            }

            RoofDoorAutoOpener autoOpener = roofDoorObject.GetComponent<RoofDoorAutoOpener>();

            if (autoOpener == null)
            {
                autoOpener = roofDoorObject.AddComponent<RoofDoorAutoOpener>();
            }

            autoOpener.rootZNetView = rootZNetView;
            autoOpener.detectionRadius = AutoOpenRadius;
            autoOpener.closeDelay = AutoCloseDelay;
            autoOpener.customDoor = roofDoorObject.GetComponent<CustomChildDoor>();

            autoOpener.detectionPoint = FindDeepChild(roofDoorObject.transform, "Roof_Detection_Point");

            if (autoOpener.detectionPoint == null)
            {
                autoOpener.detectionPoint = roofDoorObject.transform;
            }
        }

        private static Transform FindDeepChild(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                Transform found = FindDeepChild(child, childName);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }

    public class RoofDoorAutoOpener : MonoBehaviour
    {
        public CustomChildDoor customDoor;
        public ZNetView rootZNetView;
        public Transform detectionPoint;
        public float detectionRadius = 10f;
        public float closeDelay = 7f;

        private const float CheckInterval = 0.25f;

        private float _nextCheckTime;
        private float _lastPlayerSeenTime = -999f;
        private bool _wantedOpen;

        private void Awake()
        {
            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (customDoor == null)
            {
                customDoor = GetComponent<CustomChildDoor>();
            }

            if (rootZNetView == null)
            {
                rootZNetView = GetComponentInParent<ZNetView>();
            }

            if (rootZNetView == null && customDoor != null)
            {
                rootZNetView = customDoor.m_rootZNetView;
            }

            if (detectionPoint == null)
            {
                detectionPoint = FindDeepChild(transform, "Roof_Detection_Point");
            }

            if (detectionPoint == null)
            {
                detectionPoint = transform;
            }
        }

        private void Update()
        {
            if (Time.time < _nextCheckTime)
            {
                return;
            }

            _nextCheckTime = Time.time + CheckInterval;

            if (customDoor == null || rootZNetView == null || detectionPoint == null)
            {
                Refresh();
            }

            if (customDoor == null || rootZNetView == null || !rootZNetView.IsValid() || rootZNetView.GetZDO() == null)
            {
                return;
            }

            Player localPlayer = Player.m_localPlayer;

            if (localPlayer == null)
            {
                return;
            }

            bool playerNear = IsPlayerNear(localPlayer);

            if (playerNear)
            {
                _lastPlayerSeenTime = Time.time;
            }

            bool shouldBeOpen = playerNear || Time.time - _lastPlayerSeenTime <= closeDelay;

            if (shouldBeOpen == _wantedOpen && IsDoorOpen() == shouldBeOpen)
            {
                return;
            }

            _wantedOpen = shouldBeOpen;
            customDoor.AutoSetDoorState(shouldBeOpen);
        }

        private bool IsPlayerNear(Player localPlayer)
        {
            if (localPlayer == null)
            {
                return false;
            }

            Vector3 checkPosition = detectionPoint != null ? detectionPoint.position : transform.position;

            return Vector3.Distance(localPlayer.transform.position, checkPosition) <= detectionRadius;
        }

        private bool IsDoorOpen()
        {
            return GetDoorState() != 0;
        }

        private int GetDoorState()
        {
            if (customDoor == null || rootZNetView == null || !rootZNetView.IsValid() || rootZNetView.GetZDO() == null)
            {
                return 0;
            }

            return rootZNetView.GetZDO().GetInt(GetDoorStateKey(), 0);
        }

        private string GetDoorStateKey()
        {
            string doorID = customDoor != null && !string.IsNullOrEmpty(customDoor.m_doorID)
                ? customDoor.m_doorID
                : "Roof_Door";

            return "CustomChildDoor_" + doorID + "_state";
        }

        private static Transform FindDeepChild(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                Transform found = FindDeepChild(child, childName);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}