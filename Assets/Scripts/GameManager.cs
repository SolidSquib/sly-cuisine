using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace LD41
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int sceneIndex;

        private static GameManager m_Singleton;

        [SerializeField] private int m_TotalScore = 0;

        public int TotalScore { get { return m_TotalScore; } }

        public static GameManager Singleton { get { return m_Singleton; } }

        public Action<int> TotalScoreChanged = delegate { };

        private void Awake()
        {
            #region
            if (m_Singleton == null)
            {
                DontDestroyOnLoad(gameObject);
                m_Singleton = this;
            }
            else if (m_Singleton != this)
            {
                Destroy(gameObject);
            }
            #endregion
        }

        public void UpdateTotalScore(int value)
        {
            m_TotalScore += value;
            TotalScoreChanged(m_TotalScore);
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void NextLevel()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            index += 1;
            SceneManager.LoadScene(index);
        }
    }
}
   
