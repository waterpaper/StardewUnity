using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WATP
{
    public enum SoundTrack
    {
        BGM,
        SFX,
    }

    /// <summary>
    /// 전체 사운드를 관리하는 매니저
    /// </summary>
    public class SoundManager
    {
        private Transform transform;
        Dictionary<SoundTrack, uint> trackCounts = new Dictionary<SoundTrack, uint>()
        {
            { SoundTrack.BGM, 1},
            { SoundTrack.SFX, 5},
        };

        Dictionary<SoundTrack, List<AudioSource>> audioSoruces;

        public void Init()
        {
            var obj = new GameObject("[Sound]");
            transform = obj.transform;

            audioSoruces = new Dictionary<SoundTrack, List<AudioSource>>();
            audioSoruces.Add(SoundTrack.BGM, new());
            audioSoruces.Add(SoundTrack.SFX, new());

            CreateTrack(SoundTrack.BGM, 1);
            CreateTrack(SoundTrack.SFX, 5);

            Root.GameDataManager.Preferences.OnBgmChange += OnPreferencesBGMEvent;
            Root.GameDataManager.Preferences.OnSfxChange += OnPreferencesSFXEvent;
        }

        public void Dispose()
        {
            Root.GameDataManager.Preferences.OnBgmChange -= OnPreferencesBGMEvent;
            Root.GameDataManager.Preferences.OnSfxChange -= OnPreferencesSFXEvent;
        }


        void CreateTrack(SoundTrack track, int count)
        {
            var soruces = new List<AudioSource>();
            var obj = new GameObject($"{track}");
            obj.transform.SetParent(transform);

            var trackCount = trackCounts[track];
            for (int i = 0; i < trackCount; i++)
            {
                var sourceObj = new GameObject($"{track}AudioSoruce {i}");
                var source = sourceObj.AddComponent<AudioSource>();
                soruces.Add(source);

                sourceObj.transform.SetParent(obj.transform);
            }

            audioSoruces[track] = soruces;
        }

        public AudioSource PlaySound(SoundTrack track, string clipName, bool isLoop = false)
        {
            if (!string.IsNullOrEmpty(clipName) && TryGetAudioSource(track, out var source))
            {
                var clip = AssetLoader.Load<AudioClip>($"Address/Sound/{clipName}.mp3");
                source.clip = clip;
                source.loop = isLoop;
                source.Play();
                return source;
            }
            return null;
        }

        public void PlaySound(SoundTrack track, AudioClip clip, bool isLoop)
        {
            if (TryGetAudioSource(track, out var source))
            {
                source.clip = clip;
                source.loop = isLoop;
                source.Play();
            }
        }
        public bool TryGetAudioSource(SoundTrack track, out AudioSource audioSource)
        {
            for (int i = audioSoruces[track].Count - 1; i > -1; i--)
            {
                var source = audioSoruces[track][i];

                // 없으면 0번으로 반환
                if (!source.isPlaying || i == 0)
                {
                    audioSource = source;
                    return true;
                }
            }

            audioSource = null;
            return false;
        }

        private void OnPreferencesBGMEvent(int level)
        {
            var list = audioSoruces[SoundTrack.BGM];

            foreach (var sound in list)
                sound.volume = level / 100.0f;
        }

        private void OnPreferencesSFXEvent(int level)
        {
            var list = audioSoruces[SoundTrack.SFX];

            foreach (var sound in list)
                sound.volume = level / 100.0f;
        }
    }
}
