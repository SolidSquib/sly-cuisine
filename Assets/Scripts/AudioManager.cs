using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD41
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        protected AudioSource audioSource;

        protected void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
        }

        public void PickRandomClip(List<AudioClip> clips)
        {
            if (clips != null && clips.Count > 0)
            {
                int index = 0;

                if (clips.Count > 1)
                    index = (int)Random.Range(0, clips.Count);

                audioSource.clip = clips[index];
            }            
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