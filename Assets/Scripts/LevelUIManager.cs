using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD41
{
    public class LevelUIManager : MonoBehaviour
    {
        [SerializeField] private Text livesText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text currentScoreText;
        [SerializeField] private Transform panelChild;
        [SerializeField] private Transform endOfRoundPanel;

        private void Awake()
        {
            if (panelChild != null)
            {
                livesText = panelChild.GetChild(0).GetComponent<Text>();
                currentScoreText = panelChild.GetChild(1).GetComponent<Text>();
            }
        }

        void OnEnable()
        {
            if (LevelManager.Singleton != null)
            {
                LevelManager.Singleton.LivesChanged += UpdateLives;
                LevelManager.Singleton.CurrentScoreChanged += UpdateCurrentScore;
                LevelManager.Singleton.EndOfRound += ActivateEndOfRoundPanel;
                LevelManager.Singleton.TimerChanged += UpdateTimer;
                UpdateLives(LevelManager.Singleton.CurrentLives);
                UpdateCurrentScore(LevelManager.Singleton.CurrentScore);
            }
        }

        void OnDisable()
        {
            if (LevelManager.Singleton != null)
            {
                LevelManager.Singleton.LivesChanged -= UpdateLives;
                LevelManager.Singleton.CurrentScoreChanged -= UpdateCurrentScore;
                LevelManager.Singleton.EndOfRound -= ActivateEndOfRoundPanel;
                LevelManager.Singleton.TimerChanged -= UpdateTimer;
            }
        }       

        private void UpdateLives(int newValue)
        {
            livesText.text = newValue.ToString();
            AnimateText(livesText);
        }

        private void UpdateCurrentScore(int newValue)
        {
            currentScoreText.text = newValue.ToString();
            AnimateText(currentScoreText);
        }

        private void UpdateTimer(int newValue)
        {
            timerText.text = newValue.ToString();
            if (newValue < 10)
                AnimateText(timerText);
        }

        private void ActivateEndOfRoundPanel(bool x)
        {
            endOfRoundPanel.gameObject.SetActive(true);
        }

        private void AnimateText(Text text)
        {
            if(text.GetComponent<Animator>())
                text.GetComponent<Animator>().SetTrigger("ValueChanged");
        }
    }
}

