using UnityEngine;

namespace UI
{


    public class UiPlatformSwitcher : MonoBehaviour
    {
        public enum Platform
        {
            Auto,
            PC,
            Mobile
        }

        [Header("Mode")] [SerializeField] private Platform platform = Platform.Auto;

        [Header("UI Roots")] [SerializeField] private GameObject uiPcRoot;
        [SerializeField] private GameObject uiMobileRoot;

        [Header("Optional: tell spawner where to spawn letters")] [SerializeField]
        private Letters.LetterSpawner spawner;

        [SerializeField] private RectTransform pcSpawnParent;
        [SerializeField] private RectTransform mobileSpawnParent;

        private void Awake()
        {
            Apply();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            Apply();
        }
#endif

        public void Apply()
        {
            var target = platform;

            if (target == Platform.Auto)
            {
#if UNITY_ANDROID || UNITY_IOS
            target = Platform.Mobile;
#else
                target = Platform.PC;
#endif
            }

            bool isMobile = target == Platform.Mobile;

            if (uiPcRoot) uiPcRoot.SetActive(!isMobile);
            if (uiMobileRoot) uiMobileRoot.SetActive(isMobile);
            if (spawner)
            {
                var parent = isMobile ? mobileSpawnParent : pcSpawnParent;
                if (parent) spawner.SetSpawnParent(parent);
            }
        }

        // Si tu veux l’appeler depuis un bouton/debug
        public void SetPlatformPC()
        {
            platform = Platform.PC;
            Apply();
        }

        public void SetPlatformMobile()
        {
            platform = Platform.Mobile;
            Apply();
        }

        public void SetPlatformAuto()
        {
            platform = Platform.Auto;
            Apply();
        }
    }
}