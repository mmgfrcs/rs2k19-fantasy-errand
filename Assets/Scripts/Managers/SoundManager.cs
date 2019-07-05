using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FantasyErrand
{
    public class SoundManager : MonoBehaviour
    {
        public GameObject audioSourceTarget;
        public AudioClip[] AudioClipList;
        public AudioMixerGroup bgmMixer, sfxMixer, voiceMixer;
        public float timeGapBackSound = 15f;

        private bool isPlayBackSound = false;
        Queue<AudioSource> bgmSource = new Queue<AudioSource>(), sfxSource = new Queue<AudioSource>(), voiceSource = new Queue<AudioSource>();
        List<AudioSource> playingBgmSource = new List<AudioSource>(), playingSfxSource = new List<AudioSource>(), playingVoiceSource = new List<AudioSource>();
        // Singleton instance.
        public static SoundManager Instance = null;

        public enum SoundChannel
        {
            BGM, SFX, Voice
        }

        // Initialize the singleton instance.
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);

            if (audioSourceTarget == null) audioSourceTarget = gameObject;
            GameManager.OnGameEnd += EndPlayBackSound;
            GameManager.OnGameStart += playBackSound;
        }

        private void Update()
        {
            for(int i = 0; i<playingBgmSource.Count; i++)
            {
                if (!playingBgmSource[i].isPlaying)
                {
                    bgmSource.Enqueue(playingBgmSource[i]);
                    playingBgmSource.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < playingSfxSource.Count; i++)
            {
                if (!playingSfxSource[i].isPlaying)
                {
                    sfxSource.Enqueue(playingSfxSource[i]);
                    playingSfxSource.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < playingVoiceSource.Count; i++)
            {
                if (!playingVoiceSource[i].isPlaying)
                {
                    voiceSource.Enqueue(playingVoiceSource[i]);
                    playingVoiceSource.RemoveAt(i);
                    break;
                }
            }
        }

        public int GetQueueCount(SoundChannel channel)
        {
            if (channel == SoundChannel.BGM) return bgmSource.Count;
            else if (channel == SoundChannel.Voice) return voiceSource.Count;
            else return sfxSource.Count;
        }

        public int GetPlayingListCount(SoundChannel channel)
        {
            if (channel == SoundChannel.BGM) return playingBgmSource.Count;
            else if (channel == SoundChannel.Voice) return playingVoiceSource.Count;
            else return playingSfxSource.Count;
        }

        public void PlaySound(string soundName, SoundChannel channel = SoundChannel.SFX)
        {
            AudioSource audio;
            AudioClip clip = null;
            for (int i = 0; i < AudioClipList.Length; i++)
            {
                if (soundName.Equals(AudioClipList[i].name))
                {
                    clip = AudioClipList[i];
                }
            }

            if (clip == null)
            {
                Debug.LogWarning("No clip found for " + soundName);
                return;
            }

            if(channel == SoundChannel.BGM)
            {
                if (bgmSource.Count == 0) audio = audioSourceTarget.AddComponent<AudioSource>();
                else audio = bgmSource.Dequeue();
                audio.outputAudioMixerGroup = bgmMixer;
                playingBgmSource.Add(audio);
            }
            else if(channel == SoundChannel.Voice)
            {
                if (voiceSource.Count == 0) audio = audioSourceTarget.AddComponent<AudioSource>();
                else audio = voiceSource.Dequeue();
                audio.outputAudioMixerGroup = voiceMixer;
                playingVoiceSource.Add(audio);
            }
            else
            {
                if (sfxSource.Count == 0) audio = audioSourceTarget.AddComponent<AudioSource>();
                else audio = sfxSource.Dequeue();
                audio.outputAudioMixerGroup = sfxMixer;
                playingSfxSource.Add(audio);
            }
            audio.clip = clip;
            audio.volume = 1;
            audio.Play();
            Debug.Log($"Playing {soundName} {channel.ToString()}");
        }

        IEnumerator PlayBackSound()
        {
            float currTime = 0;
            isPlayBackSound = true;
            while (isPlayBackSound)
            {
                currTime += Time.deltaTime;

                if (currTime >= timeGapBackSound)
                {
                    currTime = 0;
                    PlaySound("Growl");
                }
                yield return null;
            }
        }

        public void EndPlayBackSound(GameEndEventArgs args)
        {
            isPlayBackSound = false;
        }

        public  void playBackSound()
        {
            StartCoroutine(PlayBackSound());
        }
    }
}