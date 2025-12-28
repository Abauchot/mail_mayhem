using System;
using UnityEngine;
using Stats;

namespace GameModes
{
    public sealed class GameSessionController : MonoBehaviour
    {
        [Header("Mode")] [SerializeField] private GameModeDefinition currentMode;

        [Header("Runtime state")] [SerializeField]
        private bool isRunning;

        //Time Attack 
        private float _timeLeft;

        //Survival
        private int _mistakesLeft;

        private float _runStartTime;

        public bool IsRunning => isRunning;
        public float TimeLeft => _timeLeft;
        public int MistakesLeft => _mistakesLeft;
        
        
        // Events 
        public event Action<RunResult> onRunEnded; 
        [SerializeField] private RunStatsTracker runStatsTracker;

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            if (currentMode.mode == GameMode.TimeAttack)
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0f)
                {
                    EndRun();
                }
            }
        }

        // === Public API === 
        
        public void StartRun(GameModeDefinition mode)
        {
            runStatsTracker?.ResetStats();
            if (isRunning) return;
            if (!mode)
            {
                Debug.LogError("StartRun: mode is null");
                return;
            }

            currentMode = mode;
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

            _mistakesLeft--;
            Debug.Log($"Mistake registered. Mistakes left: {_mistakesLeft}");
            if (_mistakesLeft <= 0)
            {
                EndRun();
            }
        }

        public void EndRun()
        {
            if (!isRunning)
            {
                return;
            }

            isRunning = false;
            float duration = Time.time - _runStartTime;
            Debug.Log($"Run ended: Mode={currentMode.displayName}, Duration={duration:0.00}s");

            var runResult = new RunResult
            {
                mode = currentMode.mode,
                modeName = currentMode.displayName,
                score = 0, //TODO: calculate score
                maxCombo = 0, //TODO: track max combo
                correctCount = runStatsTracker ? runStatsTracker.CorrectCount : 0,
                wrongCount = runStatsTracker ? runStatsTracker.WrongCount : 0,
                durationSeconds = duration,
                endedAtIso = DateTime.UtcNow.ToString("O")
            };
            
            onRunEnded?.Invoke(runResult);

            //TODO :
            // - Build RunResult
            // - Send to Leaderboard
            // - Show Summary UI
        }
    }
}
