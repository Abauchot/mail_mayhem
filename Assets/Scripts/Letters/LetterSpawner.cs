using UnityEngine;
using GameModes;

namespace Letters
{
    public class LetterSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Letter letterPrefab;
        [SerializeField] private RectTransform spawnParent;
        [SerializeField] private Boxes.BoxesRegistry boxesRegistry;
        [SerializeField] private Inputs.LetterInputRouter inputRouter;
        [SerializeField] private GameSessionController session;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 0.2f;
        [SerializeField] private bool spawnOnStart = true;

        private float _timer;
        private Letter _currentLetter;

        private ISymbolProvider _symbolProvider;
        private const int SymbolCount = 4;

        public void SetSymbolProvider(ISymbolProvider provider) => _symbolProvider = provider;

        private void Start()
        {
            if (session != null && !session.IsRunning) return;
            if (spawnOnStart) SpawnLetter();
        }

        private void Update()
        {
            if (session != null && !session.IsRunning) return;
            if (_currentLetter) return;

            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                _timer = 0f;
                SpawnLetter();
            }
        }

        private void SpawnLetter()
        {
            if (!letterPrefab || !spawnParent)
            {
                Debug.LogWarning("LetterSpawner: Letter prefab or spawn parent is not assigned.");
                return;
            }

            if (!boxesRegistry)
            {
                boxesRegistry = FindFirstObjectByType<Boxes.BoxesRegistry>();
            }

            _currentLetter = Instantiate(letterPrefab, spawnParent);

            var rt = _currentLetter.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 260f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 180f);

            var symbol = _symbolProvider != null
                ? _symbolProvider.Next()
                : (SymbolType)Random.Range(0, SymbolCount);

            _currentLetter.Setup(symbol, this, boxesRegistry);

            if (inputRouter)
            {
                inputRouter.SetCurrentLetter(_currentLetter);
            }
        }

        public void OnLetterDestroyed(Letter letter)
        {
            if (_currentLetter == letter)
            {
                _currentLetter = null;
                _timer = 0f;
            }
        }

        public void SetSpawnParent(RectTransform parent) => spawnParent = parent;

        public void ClearCurrentLetter()
        {
            if (!_currentLetter)
            {
                return;
            }
            Destroy(_currentLetter.gameObject);
            _currentLetter = null;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (!enabled) ClearCurrentLetter();
        }
    }
}
