using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD41
{
    [RequireComponent(typeof(UnitCollision))]
    [RequireComponent(typeof(AudioSource))]
    public class FoodManager : MonoBehaviour
    {
        // Perfect recipe token
        // Imperfect recipe token
        // give them scoretoken tag
        // on level exit they give score
        // finishing level with food tokens still gives score but less than before
        // Score calculation = number of correct ingredients * (foodtoken score * bonus) + number of incorrect ingredients * (foodtoken score / 2)

        // Looping Boil or Sizzle while it has 1+ ingredient
        // Chopping while rats are within range
        [SerializeField] AudioSource audioManagerCooking;
        [SerializeField] AudioSource audioManagerRatProgress;
        [SerializeField] private List<AudioClip> cookingAudioClips = new List<AudioClip>();
        [SerializeField] private List<AudioClip> progressAudioClips = new List<AudioClip>();
        [SerializeField] private List<AudioClip> finishedAudioClips = new List<AudioClip>();

        private UnitCollision unitCollision;
        [SerializeField] private Transform tokenSpawnLocation;
        [SerializeField] private GameObject perfectScoreToken;
        [SerializeField] private GameObject imperfectScoreToken;

        [SerializeField] private List<Enums.FoodTokenType> recipeIngredients = new List<Enums.FoodTokenType>();
        private List<FoodToken> currentIngredients = new List<FoodToken>();
        [SerializeField] private int ingredientsToBeginCooking = 3;
        [SerializeField] private float progressSpeed = 1f;
        [SerializeField] private float progressValue = 0.0f;
        [SerializeField] private Scrollbar progressBarSlider;
        private bool unitInRange = false;
        private bool m_IsActive = true;
        private bool startedProgressAudio = false;

        public bool IsActive { get { return m_IsActive; } }

        private void Awake()
        {
            audioManagerCooking.spatialBlend = 0f;
            audioManagerRatProgress.spatialBlend = 0f;
            unitCollision = GetComponent<UnitCollision>();
        }

        private void OnEnable()
        {
            if (progressBarSlider == null)
                progressBarSlider = GetComponentInChildren<Scrollbar>();

            if (progressBarSlider != null)
            {
                progressBarSlider.gameObject.SetActive(false);
                UpdateSlider(0.0f);
            }
            
            unitCollision.CollisionEvent += CollisionHandler;
        }

        private void OnDisable()
        {
            unitCollision.CollisionEvent -= CollisionHandler;
        }

        private void Update()
        {
            if (m_IsActive && unitInRange && currentIngredients.Count >= ingredientsToBeginCooking)
            {
                if (!startedProgressAudio)
                {
                    startedProgressAudio = true;
                    AudioClip clip = progressAudioClips[PickRandomClip(progressAudioClips)];
                    PlayClip(audioManagerRatProgress, clip);
                }
                else
                {
                    if (!audioManagerRatProgress.isPlaying)
                        audioManagerRatProgress.UnPause();
                }

                progressValue += (Time.smoothDeltaTime * progressSpeed) / 100;
                UpdateProgress();
            }
            else if (m_IsActive && !unitInRange && startedProgressAudio)
            {
                if (audioManagerRatProgress.isPlaying)
                    audioManagerRatProgress.Pause();
            }

            unitInRange = false;
        }

        public void IngredientAdded(FoodToken newFood)
        {
            if (m_IsActive && newFood.FoodType != Enums.FoodTokenType.Null && !newFood.IsCooked)
            {
                currentIngredients.Add(newFood);
                newFood.transform.position = transform.position;

                if(!progressBarSlider.IsActive())
                    progressBarSlider.gameObject.SetActive(true);
            }

            // Is it first ingredient?
            if (currentIngredients.Count == 1)
            {
                // Play looping random cooking sound
                AudioClip clip = cookingAudioClips[PickRandomClip(cookingAudioClips)];
                PlayClip(audioManagerCooking, clip);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            CollisionHandler(collision.transform);
        }

        private void CollisionHandler(Transform other)
        {
            if (other.CompareTag("Player"))
                unitInRange = true;
        }       

        private void UpdateSlider(float value)
        {
            if (progressBarSlider != null)
                progressBarSlider.size = value;
        }

        private void UpdateProgress()
        {
            UpdateSlider(progressValue);

            if (progressValue >= 1)
            {
                m_IsActive = false;
                // Stop cooking audio
                audioManagerCooking.Stop();
                // Stop any current rat progress audio
                audioManagerRatProgress.Stop();
                // Play completed audio
                AudioClip clip = finishedAudioClips[PickRandomClip(finishedAudioClips)];
                PlayClip(audioManagerRatProgress, clip);

                if (progressBarSlider != null)
                    progressBarSlider.gameObject.SetActive(false);

                GameObject cookedToken = null;

                if (IngredientsCheck())
                {
                    cookedToken = Instantiate(perfectScoreToken);
                }
                else
                {
                    cookedToken = Instantiate(imperfectScoreToken);
                }

                cookedToken.transform.position = tokenSpawnLocation.position;
                cookedToken.GetComponent<FoodToken>().IsCooked = true;

                int x = currentIngredients.Count;
                GameObject food = null;
                for (int y = 0; y < x; y++)
                {
                    food = currentIngredients[0].gameObject;
                    currentIngredients.RemoveAt(0);
                    Destroy(food);
                }

                currentIngredients.Clear();
            }
        }

        // Are all the correct ingredients in the pie
        private bool IngredientsCheck()
        {
            // correct number + correct type
            bool isCorrect = true;

            if (recipeIngredients.Count == currentIngredients.Count)
            {
                foreach (FoodToken t in currentIngredients)
                {
                    if (!recipeIngredients.Contains(t.FoodType))
                        isCorrect = false;
                }
            }
            else
                isCorrect = false;

            return isCorrect;
        }

        public int PickRandomClip(List<AudioClip> clips)
        {
            int index = 0;

            if (clips != null && clips.Count > 1)
                    index = (int)Random.Range(0, clips.Count);

            return index;
        }

        public virtual void PlayClip(AudioSource audioSource, AudioClip clip)
        {
            if (clip != null && !audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}