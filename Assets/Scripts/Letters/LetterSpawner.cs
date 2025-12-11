using UnityEngine;

namespace Letters
{



    public class LetterSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] public Letter letterPrefab;
        [SerializeField] public RectTransform spawnParent;

        [Header("Spawn Settings")]
        [SerializeField] public float spawnInterval = 1.5f;
        [SerializeField] public bool spawnOnStart = true;

        private float _timer;
        private Letter _currentLetter;
       
        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnLetter();
            }
        }
        
        private void Update()
        {
            if (_currentLetter)
            {
                return;
            }
            
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
            
            _currentLetter = Instantiate(letterPrefab, spawnParent);
            RectTransform rt = _currentLetter.GetComponent<RectTransform>();
            
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 260f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 180f);
            
            int symbolCount = System.Enum.GetNames(typeof(SymbolType)).Length;
            SymbolType randomSymbol = (SymbolType)Random.Range(0, symbolCount);
            _currentLetter.Setup(randomSymbol, this);
        }
        
        public void OnLetterDestroyed(Letter letter)
        {
            if (_currentLetter == letter)
            {
                _currentLetter = null;
                _timer = 0f;
            }
        }
    }
}
