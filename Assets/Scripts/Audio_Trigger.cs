using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
    public class Audio_Trigger : MonoBehaviour
    {
        protected AudioSource audioSource;
        [SerializeField] protected List<AudioClip> audioClipList = new List<AudioClip>();
        [SerializeField] protected bool distanceFalloff;
        protected Transform distanceTrackTarget;
        protected float maxDistance;

        protected void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
        }

        protected void Update()
        {
            if (distanceTrackTarget && distanceFalloff)
            {
                audioSource.volume = (1 - (VolumeDistance(distanceTrackTarget) / maxDistance));
            }
        }

        protected void OnEnable()
        {
            if (audioClipList.Count > 0)
                audioSource.clip = audioClipList[PickRandomClip()];
            else
                Debug.LogError("Missing Audio Clips in AudioClipList");
        }

        protected float VolumeDistance(Transform target)
        {
            return Vector3.Distance(target.position, transform.position);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            CollisonHandler(collision.transform);
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            CollisonHandler(collision.transform);
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            ExitCollisonHandler(collision.transform);
        }
        protected void CollisonHandler(Transform other)
        {
            Debug.Log("CollisonHandler");
            if (!distanceTrackTarget && other.CompareTag("Player"))
            {
                if (distanceFalloff)
                {
                    distanceTrackTarget = other;
                    maxDistance = VolumeDistance(distanceTrackTarget);
                }
            }

            if (!audioSource.loop && !audioSource.isPlaying)
            {
                audioSource.clip = audioClipList[PickRandomClip()];
                PlayClip();
            }
        }

        protected void ExitCollisonHandler(Transform other)
        {
            if (audioSource.loop && distanceTrackTarget == other)
            {
                StopClip();
                distanceTrackTarget = null;
            }
        }

        protected int PickRandomClip()
        {
            if (audioClipList.Count > 0)
            {
                return (int)Random.Range(0, audioClipList.Count);
            }
            else
                return 0;
        }

        public virtual void PlayClip()
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        public virtual void PauseClip()
        {
            audioSource.Pause();
        }

        public virtual void StopClip()
        {
            audioSource.Stop();
        }
    }
}