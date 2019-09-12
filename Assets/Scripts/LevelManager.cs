using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LD41
{
    [RequireComponent(typeof(AudioManager))]
    public class LevelManager : MonoBehaviour
    {
        private GameManager gameManager;
        private static LevelManager m_Singleton;

        private AudioManager audioManager;
        [SerializeField] private List<AudioClip> deathAudioClips = new List<AudioClip>();
        [SerializeField] private List<AudioClip> reachedExitAudioClips = new List<AudioClip>();
        [SerializeField] private List<AudioClip> levelCompletedAudioClips = new List<AudioClip>();
        [SerializeField] private List<AudioClip> foodCollectedAudioClips = new List<AudioClip>();


        private int m_CurrentLives = 0;
        private int completedRats = 0;
        [SerializeField] private int m_CurrentScore = 0;
        [SerializeField] private int m_UncookedFoodScore = 100;
        [SerializeField] private int m_ImperfectCookedFoodScore = 500; // Score for cake
        [SerializeField] private int m_PerfectCookedFoodScore = 500; // Score for cake
        [SerializeField] private int m_ScoreRatsSurvivedBonus = 100;
        [SerializeField] private int m_TimerScoreBonus = 10; // Score given for each second left on the timer
        [SerializeField] private float m_LevelTimer = 60;
        private List<FoodToken> foodCollected = new List<FoodToken>();
        private List<FoodToken> foodCooked= new List<FoodToken>();
        private List<FoodToken> foodLost = new List<FoodToken>(); // enemy drop, kill volume lost
        private IEnumerator timerCoroutine;

        public static LevelManager Singleton { get { return m_Singleton; } }
        public int CurrentLives { get { return m_CurrentLives; } }
        public int CurrentScore { get { return m_CurrentScore; } }
        public int FoodTokenScore { get { return m_UncookedFoodScore; } }
        public int FoodImperfectCookedScore { get { return m_PerfectCookedFoodScore; } }
        public int FoodPerfectCookedScore { get { return m_PerfectCookedFoodScore; } }

        public Action NewRound = delegate { };
        public Action<bool> EndOfRound = delegate { }; // True = win
        public Action<int> LivesChanged = delegate { };
        public Action<int> CurrentScoreChanged = delegate { };
        public Action<int> TimerChanged = delegate { };
        public Action<Enums.FoodTokenType> FoodTokenDroppedEvent = delegate { };
        public Action<Enums.FoodTokenType> FoodTokenLostEvent = delegate { };
        public Action<Enums.FoodTokenType> FoodTokenPickedUpEvent = delegate { };
        public Action<Enums.FoodTokenType> FoodTokenCookedEvent = delegate { };
        public Action<Enums.FoodTokenType> FoodTokenCollectedEvent = delegate { };

        private void Awake()
        {
            m_Singleton = this;
            audioManager = GetComponent<AudioManager>();
        }

        private void OnEnable()
        {
            EndOfRound += RoundOver;
        }

        private void OnDisable()
        {
            EndOfRound -= RoundOver;
        }

        private void Start()
        {
            if (GameManager.Singleton != null)
                gameManager = GameManager.Singleton;
            else
                Debug.LogError("Missing GameManager.Singleton");

            timerCoroutine = LevelTimerCountdown();
            StartCoroutine(timerCoroutine);
        }   

        // Reset anything we need
        public void StartLevel()
        {

        }

        public void UnitReachedExit(Transform unit)
        {
            audioManager.PickRandomClip(reachedExitAudioClips);
            audioManager.PlayClip();

            completedRats++;
            UpdateLives(-1);
            UpdatCurrentScore(m_ScoreRatsSurvivedBonus);
        }

        public void UnitDied(Transform unit)
        {
            audioManager.PickRandomClip(deathAudioClips);
            audioManager.PlayClip();
            UpdateLives(-1);            
        }

        public void FoodTokenDropped(FoodToken foodTokenDropped)
        {
            if (foodTokenDropped != null)
            {
                FoodTokenDroppedEvent(foodTokenDropped.FoodType);
            }
        }

        public void FoodTokenLost(FoodToken foodTokenLost)
        {
            if (foodTokenLost != null)
            {
                foodLost.Add(foodTokenLost);
                FoodTokenLostEvent(foodTokenLost.FoodType);
            }
        }

        public void FoodTokenCooked(FoodToken foodToken)
        {
            if (foodToken != null)
            {
                if (foodToken.FoodType != Enums.FoodTokenType.Null && !foodToken.IsCooked)
                {
                    foodCooked.Add(foodToken);
                    FoodTokenCookedEvent(foodToken.FoodType);
                }
            }
        }

        public void FoodTokenCollected(FoodToken foodToken)
        {
            if (foodToken != null)
            {
                if (foodToken.FoodType != Enums.FoodTokenType.Null)
                {
                    int score = FoodTokenScore;

                    if (foodToken.FoodType == Enums.FoodTokenType.CookedPerfect)
                    {
                        score = FoodPerfectCookedScore;
                    }

                    if (foodToken.FoodType == Enums.FoodTokenType.CookedImperfect)
                    {
                        score = FoodImperfectCookedScore;
                    }

                    audioManager.PickRandomClip(foodCollectedAudioClips);
                    audioManager.PlayClip();
                    FoodTokenCollectedEvent(foodToken.FoodType);
                    foodCollected.Add(foodToken);
                    UpdatCurrentScore(score);
                }
            }
        }        

        public void UpdateLives(int value)
        {
            m_CurrentLives += value;
            LivesChanged(CurrentLives);

            if (CurrentLives <= 0)
            {
                if(completedRats == 0)
                    EndOfRound(false);
                else
                    EndOfRound(true);
            }
        }

        public void UpdatCurrentScore(int value)
        {
            m_CurrentScore += value;
            CurrentScoreChanged(m_CurrentScore);
        }
                      
        private void RoundOver(bool won)
        {
            StopCoroutine(timerCoroutine);

            if (won)
            {
                audioManager.PickRandomClip(levelCompletedAudioClips);
                audioManager.PlayClip();
                gameManager.UpdateTotalScore(CurrentScore + ((int)m_LevelTimer * m_TimerScoreBonus));
            }
        }

        IEnumerator LevelTimerCountdown()
        {
            bool loop = true;

            while (loop)
            {
                TimerChanged((int)m_LevelTimer);

                yield return new WaitForSecondsRealtime(1f);
                m_LevelTimer--;

                if (m_LevelTimer <= 0)
                    loop = false;
            }

            EndOfRound(false);
        }
    }
}

