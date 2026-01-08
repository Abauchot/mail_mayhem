using UnityEngine;

namespace UI
{
    public class UiPlatformSwitcher : MonoBehaviour
    {
        [Header("UI Root")]
        [SerializeField] private GameObject uiPcRoot;

        [Header("Spawner Setup")]
        [SerializeField] private Letters.LetterSpawner spawner;
        [SerializeField] private RectTransform pcSpawnParent;

        [SerializeField] private Inputs.LetterInputRouter inputRouter;
        [SerializeField] private Boxes.BoxesRegistry registry;

        private void Awake()
        {
            Apply();
        }

        public void Apply()
        {
            if (uiPcRoot) uiPcRoot.SetActive(true);

            if (spawner && pcSpawnParent)
                spawner.SetSpawnParent(pcSpawnParent);

            if (!inputRouter)
                inputRouter = GetComponentInChildren<Inputs.LetterInputRouter>(true);

            if (!registry && inputRouter)
                registry = inputRouter.BoxesRegistry;

            if (registry && uiPcRoot)
                registry.RebuildFromRoot(uiPcRoot.transform);
        }
    }
}
