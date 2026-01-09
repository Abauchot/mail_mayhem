using System;
using UnityEngine;
using Stats;
using Difficulty;
using Boxes;

namespace GameModes
{
    public sealed class GameSessionController : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private GameModeDefinition currentMode;
        public GameModeDefinition CurrentMode => currentMode;
        
        private GameModeDefinition _lastMode;

        [Header("Runtime state")] [SerializeField]
        private bool isRunning;
        
        [SerializeField] private Scoring.ScoreSystemListener scoreSystem;

        [SerializeField]
        private Letters.LetterSpawner spawner;

        [SerializeField]
        private SymbolPermutationController permutationController;

        [SerializeField]
        private BoxesRegistry boxesRegistry;

        //Time Attack 
        private float _timeLeft;

        //Survival
        private int _mistakesLeft;

        private float _runStartTime;

        public bool IsRunning => isRunning;
        public float TimeLeft => _timeLeft;
        public int MistakesLeft => _mistakesLeft;
        
        
        // Events 
        public event Action<RunResult> OnRunEnded; 
        [SerializeField] private RunStatsTracker runStatsTracker;

        private void Update()
        {
             if (!isRunning)
             {
                 return;
             }

             if (currentMode.mode == GameMode.TimeAttack)
             {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                 if (currentMode.debug.enabled && currentMode.debug.freezeTime)
                     return;
#endif

                 _timeLeft -= Time.deltaTime;
                 if (_timeLeft <= 0f) EndRun();
             }
        }

        // === Public API === 
        
        private void ApplyModeToSpawner(GameModeDefinition mode)
        {
            if (spawner == null) return;

            int? seed = null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (mode.debug.enabled && mode.debug.useSeed)
                seed = mode.debug.seed;
#endif

            Letters.ISymbolProvider provider = new Letters.RandomSymbolProvider(seed);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (mode.debug.enabled && mode.debug.forceSymbol)
                provider = new Letters.ForcedSymbolProvider(mode.debug.forcedSymbol);
#endif

            spawner.SetSymbolProvider(provider);
        }

        
        public void StartRun(GameModeDefinition mode)
        {
            if (isRunning) return;
            if (!mode)
            {
                Debug.LogError("StartRun: mode is null");
                return;
            }

            _lastMode = mode;
            currentMode = mode;

            ApplyModeToSpawner(mode);

            if (spawner)
            {
                spawner.SetEnabled(true);
            }

            scoreSystem?.ResetForRun(mode);
            runStatsTracker?.ResetStats();

            boxesRegistry?.ResetMapping();
            if (permutationController)
                permutationController.SetConfig(mode.permutationConfig);

            isRunning = true;
            _runStartTime = Time.time;

            switch (currentMode.mode)
            {
                case GameMode.TimeAttack:
                    _timeLeft = currentMode.timeAttackDurationSeconds;
                    Debug.Log($"Run started: Mode={mode.displayName} (TimeAttack) timeLeft={_timeLeft:0.00}s");
                    break;
                case GameMode.Survival:
                    _mistakesLeft = currentMode.maxMistakes;
                    Debug.Log($"Run started: Mode={mode.displayName} (Survival) mistakesLeft={_mistakesLeft}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void RegisterMistake()
        {
            if (!isRunning || currentMode.mode != GameMode.Survival)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (currentMode.debug.enabled && currentMode.debug.infiniteMistakes)
                return;
#endif

            _mistakesLeft--;
            Debug.Log($"Mistake registered. Mistakes left: {_mistakesLeft}");
            if (_mistakesLeft <= 0)
            {
                EndRun();
            }
        }

        private void EndRun()
        {
            if (!isRunning)
            {
                return;
            }

            isRunning = false;
            
            if (spawner)
            {
                spawner.SetEnabled(false);
            }
            
            float duration = Time.time - _runStartTime;
            Debug.Log($"Run ended: Mode={currentMode.displayName}, Duration={duration:0.00}s");

            var runResult = new RunResult
            {
                mode = currentMode.mode,
                modeName = currentMode.displayName,
                score = scoreSystem ? scoreSystem.Score : 0,
                maxCombo = scoreSystem ? scoreSystem.MaxCombo : 0,
                correctCount = runStatsTracker ? runStatsTracker.CorrectCount : 0,
                wrongCount = runStatsTracker ? runStatsTracker.WrongCount : 0,
                durationSeconds = duration,
                endedAtIso = DateTime.UtcNow.ToString("O")
            };
            Debug.Log($"Run result: mode={runResult.modeName} score={runResult.score} maxCombo={runResult.maxCombo} " +
                      $"correct={runResult.correctCount} wrong={runResult.wrongCount} duration={runResult.durationSeconds:0.00}s " +
                      $"endedAt={runResult.endedAtIso}");
            
            OnRunEnded?.Invoke(runResult);

            //TODO :
            // - Build RunResult
            // - Send to Leaderboard
            // - Show Summary UI
        }
        
        public void RestartRun()
        {
            if (_lastMode == null)
            {
                Debug.LogWarning("RestartRun: No last mode to restart.");
                return;
            } 
            StartRun(_lastMode);
        }
    }
}
