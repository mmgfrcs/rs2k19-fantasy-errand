using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FantasyErrand
{

    public enum EnemySoundsType
    {
        Bite,
        Growl,
        Growl2,
        LowGrowl,
        LowGrowl2
    };

    public enum PowerUpsSoundsType
    {
        Boost,
        Boost2,
        Boost3,
        Gulp,
        Gulp2
    };

    public class SoundManager : MonoBehaviour
    {

        public AudioClip[] EnemyClipList;
        public AudioClip[] PowerUpsClipList;

        // Audio players components.
        public AudioSource EffectsSource;
        public AudioSource MusicSource;

        // Random pitch adjustment range.
        public float LowPitchRange = .95f;
        public float HighPitchRange = 1.05f;

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

        public void PlayEnemySound(EnemySoundsType type)
        {
            int index = 0;
            AudioSource audio = GetComponent<AudioSource>();
            if (type.Equals(EnemySoundsType.Bite))
                index = 0;
            else if (type.Equals(EnemySoundsType.Growl))
                index = 1;
            else if (type.Equals(EnemySoundsType.Growl2))
                index = 2;
            else if (type.Equals(EnemySoundsType.LowGrowl))
                index = 3;
            else if (type.Equals(EnemySoundsType.LowGrowl2))
                index = 4;

            audio.clip = EnemyClipList[index];
            audio.Play();
        }

        public void PlayPowerUpsSound(PowerUpsSoundsType type)
        {
            int index = 0;
            AudioSource audio = GetComponent<AudioSource>();
            if (type.Equals(PowerUpsSoundsType.Boost))
                index = 0;
            else if (type.Equals(PowerUpsSoundsType.Boost2))
                index = 1;
            else if (type.Equals(PowerUpsSoundsType.Boost3))
                index = 2;
            else if (type.Equals(PowerUpsSoundsType.Gulp))
                index = 3;
            else if (type.Equals(PowerUpsSoundsType.Gulp2))
                index = 4;

            audio.clip = PowerUpsClipList[index];
            audio.Play();
        }

        // Play a random clip from an array, and randomize the pitch slightly.
        public void RandomSoundEffect(params AudioClip[] clips)
        {
            int randomIndex = Random.Range(0, clips.Length);
            float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

            EffectsSource.pitch = randomPitch;
            EffectsSource.clip = clips[randomIndex];
            EffectsSource.Play();
        }
    }
}