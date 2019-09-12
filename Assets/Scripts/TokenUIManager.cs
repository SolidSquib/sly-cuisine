using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD41
{
    public class TokenUIManager : MonoBehaviour
    {
        private Dictionary<Enums.FoodTokenType, Image> m_FoodImageDictionary;
        [SerializeField] private List<Image> m_FoodTokenImages;

        [SerializeField] private Color startingColour;
        [SerializeField] private Color destroyedColour;
        [SerializeField] private Color pickedUpColour;
        [SerializeField] private Color cookedColour;
        [SerializeField] private Color collectedColour;

        private void Awake()
        {
            PopulateFoodDictionary();
            ResetTokens();
        }

        private void OnEnable()
        {
            if (LevelManager.Singleton != null)
            {
                LevelManager.Singleton.FoodTokenDroppedEvent += TokenDropped;
                LevelManager.Singleton.FoodTokenLostEvent += TokenLost;
                LevelManager.Singleton.FoodTokenPickedUpEvent += TokenPickedUp;
                LevelManager.Singleton.FoodTokenCookedEvent += TokenCooked;
                LevelManager.Singleton.FoodTokenCollectedEvent += TokenCollected;
            }               
        }

        private void OnDisable()
        {
            if (LevelManager.Singleton != null)
            {
                LevelManager.Singleton.FoodTokenDroppedEvent -= TokenDropped;
                LevelManager.Singleton.FoodTokenLostEvent -= TokenLost;
                LevelManager.Singleton.FoodTokenPickedUpEvent -= TokenPickedUp;
                LevelManager.Singleton.FoodTokenCookedEvent -= TokenCooked;
                LevelManager.Singleton.FoodTokenCollectedEvent -= TokenCollected;
            }          
        }

        private void PopulateFoodDictionary()
        {
            if (m_FoodTokenImages.Count == 5)
            {
                m_FoodImageDictionary = new Dictionary<Enums.FoodTokenType, Image>
                {
                    { Enums.FoodTokenType.Tomato,   m_FoodTokenImages[0] },
                    { Enums.FoodTokenType.Cheese,   m_FoodTokenImages[1] },
                    { Enums.FoodTokenType.Bread,    m_FoodTokenImages[2] },
                    { Enums.FoodTokenType.Turnip,   m_FoodTokenImages[3] },
                    { Enums.FoodTokenType.Meat,     m_FoodTokenImages[4] },
                };
            }
            else
                Debug.LogError("FoodTokenImages not 5");
        }

        // When Picked Up
        private void TokenPickedUp(Enums.FoodTokenType foodToken)
        {
            if (TempTest(foodToken))
            {
                Image image = m_FoodImageDictionary[foodToken];
                ChangleIconColour(image, pickedUpColour);
                AnimateIcon(image);
            }           
        }

        // When Food Reaches ExitDoor
        private void TokenCollected(Enums.FoodTokenType foodToken)
        {
            if (TempTest(foodToken))
            {
                Image image = m_FoodImageDictionary[foodToken];
                ChangleIconColour(image, collectedColour);
                AnimateIcon(image);
            }
        }

        private void TokenDropped(Enums.FoodTokenType foodToken)
        {
            if (TempTest(foodToken))
            {
                Image image = m_FoodImageDictionary[foodToken];
                ChangleIconColour(image, startingColour);
                AnimateIcon(image);
            }
        }

        private void TokenCooked(Enums.FoodTokenType foodToken)
        {
            if (TempTest(foodToken))
            {
                Image image = m_FoodImageDictionary[foodToken];
                ChangleIconColour(image, cookedColour);
                AnimateIcon(image);
            }
        }

        private void TokenLost(Enums.FoodTokenType foodToken)
        {
            if (TempTest(foodToken))
            {
                Image image = m_FoodImageDictionary[foodToken];
                ChangleIconColour(image, destroyedColour);
                AnimateIcon(image);
            }
        }

        private void ResetTokens()
        {
            foreach (Image v in m_FoodTokenImages)
                ChangleIconColour(v, startingColour);
        }

        private void ChangleIconColour(Image image, Color color)
        {
            image.color = color;
        }

        private void AnimateIcon(Image image)
        {
            if (image.GetComponent<Animator>())
                image.GetComponent<Animator>().SetTrigger("ValueChanged");
        }

        private bool TempTest(Enums.FoodTokenType foodToken)
        {
            if (foodToken != Enums.FoodTokenType.Null && foodToken != Enums.FoodTokenType.CookedPerfect && foodToken != Enums.FoodTokenType.CookedImperfect)
                return true;
            else
                return false;
        }
    }
}  
