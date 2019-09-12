using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD41
{
    public class EndofLevelPanel : MonoBehaviour
    {
        private GameManager gameManager;
        [SerializeField] private Text totalScoreText;

        private void Awake()
        {
            if (totalScoreText == null)
                totalScoreText = transform.GetChild(0).GetComponent<Text>();
        }

        private void OnEnable()
        {
            if (GameManager.Singleton != null)
            {
                gameManager = GameManager.Singleton;
                gameManager.TotalScoreChanged += UpdateTotalScore;
                UpdateTotalScore(gameManager.TotalScore);
            }
            else
                Debug.LogError("Missing GameManager Singleton");
        }

        private void UpdateTotalScore(int newValue)
        {
            totalScoreText.text = newValue.ToString();
        }

        public void RestartLevel()
        {
            gameManager.RestartLevel();
        }

        public void NextLevel()
        {
            gameManager.NextLevel();
        }
    }
}
