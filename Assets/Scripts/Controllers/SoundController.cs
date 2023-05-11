using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;

    public AudioSource sound;

    public static UnityAction Init;

    public List<Sound> Sounds;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SetStatusMusic(GetStatusSound());

            Init?.Invoke();
        }
    }

    public void PlaySound(SoundClip name)
    {
        try
        {
            AudioClip clip = Sounds.Find(find => find.name == name).clip;
            sound.clip = clip;
            sound.Play();
        }
        catch
        {
            Debug.Log("Sound error");
        }
    }

    public void SetStatusMusic(bool status)
    {
        PlayerPrefs.SetInt("soundStatus", status ? 1 : 0);
        sound.mute = !status;
    }

    public bool GetStatusSound()
    {
        bool soundStatus;
        if (PlayerPrefs.GetInt("soundStatus", 1) == 1)
        {
            soundStatus = true;
        }
        else
        {
            soundStatus = false;
        }

        return soundStatus;
    }

    [Serializable]
    public class Sound
    {
        public SoundClip name;
        public AudioClip clip;
    }

    public enum SoundClip
    {
        Win,
        Lose,
        Click,
        Bomb
    }
}
