using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FantasyErrand
{


    public class SoundManager : MonoBehaviour
    {

        public AudioClip[] EnemyClipList;
        public AudioClip[] PowerUpsClipList;
        public AudioSource[] AudioSourceList;
        // Audio players components.
        public AudioSource EffectsSource;
        public AudioSource MusicSource;
        public float timeGapBackSound = 15f;
        int counter = 0;

        // Singleton instance.
        public static SoundManager Instance = null;

        // Initialize the singleton instance.
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        // Play a single clip through the sound effects source.
        public void Play(AudioClip clip)
        {
            EffectsSource.clip = clip;
            EffectsSource.Play();
        }

        // Play a single clip through the music source.
        public void PlayMusic(AudioClip clip)
        {
            MusicSource.clip = clip;
            MusicSource.Play();
        }


        public void PlaySound(string soundName)
        {
            AudioSource audio = AudioSourceList[counter];
            for (int i = 0; i < EnemyClipList.Length; i++)
            {
                if (soundName.Equals(EnemyClipList[i].name))
                {
                    audio.clip = EnemyClipList[i];
                }
                if (soundName.Equals(PowerUpsClipList[i].name))
                {
                    audio.clip = PowerUpsClipList[i];

                }
                audio.Play();
                counter++;
                if (counter >= AudioSourceList.Length)
                    counter = 0;
            }

        }

        IEnumerator PlayBackSound()
        {
            float currTime = 0;
            while (true)
            {
                currTime += Time.deltaTime;

                if (currTime >= timeGapBackSound)
                {
                    print("Growl Jalan");
                    currTime = 0;
                    PlaySound("Growl");
                }
                yield return null;
            }
        }

        public  void playBackSound()
        {
            StartCoroutine(PlayBackSound());
        }
    }
}