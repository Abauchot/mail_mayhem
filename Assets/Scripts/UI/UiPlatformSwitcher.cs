using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    public class UiPlatformSwitcher : MonoBehaviour
    {
        private enum Platform
        {
            Auto,
            PC,
            Mobile
        }

        [Header("Mode")]
        [SerializeField] private Platform platform = Platform.Auto;

        [Header("UI Roots")]
        [SerializeField] private GameObject uiPcRoot;
        [SerializeField] private GameObject uiMobileRoot;

        [Header("Optional: tell spawner where to spawn letters")]
        [SerializeField] private Letters.LetterSpawner spawner;

        [SerializeField] private RectTransform pcSpawnParent;
        [SerializeField] private RectTransform mobileSpawnParent;

        [SerializeField] private Inputs.LetterInputRouter inputRouter;

        private void Awake()
        {
            Apply();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            EditorApplication.delayCall += DelayedApplyInEditor;
        }

        private void DelayedApplyInEditor()
        {
            if (this == null) return;
            ApplyEditorPreview();
        }

        private void ApplyEditorPreview()
        {
            var target = platform;
            if (target == Platform.Auto)
            {
                target = Platform.PC;
            }

            bool isMobile = target == Platform.Mobile;

            if (uiPcRoot) uiPcRoot.SetActive(!isMobile);
            if (uiMobileRoot) uiMobileRoot.SetActive(isMobile);
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

            if (!inputRouter)
                inputRouter = GetComponentInChildren<Inputs.LetterInputRouter>(true);

            if (inputRouter)
                inputRouter.UseMobile(isMobile);
        }

        public void SetPlatformPC()
        {
            platform = Platform.PC;
            if (Application.isPlaying) Apply();
#if UNITY_EDITOR
            else ApplyEditorPreview();
#endif
        }

        public void SetPlatformMobile()
        {
            platform = Platform.Mobile;
            if (Application.isPlaying) Apply();
#if UNITY_EDITOR
            else ApplyEditorPreview();
#endif
        }

        public void SetPlatformAuto()
        {
            platform = Platform.Auto;
            if (Application.isPlaying) Apply();
#if UNITY_EDITOR
            else ApplyEditorPreview();
#endif
        }
    }
}
