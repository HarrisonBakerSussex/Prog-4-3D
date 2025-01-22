using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType {
    DeathSound,
    Boulders,
    Button,
}
public class Audio_Controller : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicSounds;
    [SerializeField] private List<SoundList> sfxSounds;
    
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private float musicVolume = 1f;
    private static Audio_Controller instance;
    
    private void Awake(){
        // Need to not destroy on load
        if (instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update(){
        CycleMusic();
    }

    private void CycleMusic(){
        if (!instance.musicAudioSource.isPlaying){
            Audio_Controller.PlayMusic(instance.musicVolume);
        }
    }

    public static void PlayMusic(float volume){
        List<AudioClip> clips = instance.musicSounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Count)];
        instance.musicAudioSource.PlayOneShot(randomClip, volume);
    }

    public static void PlaySound(SoundType sound, float volume){
        List<AudioClip> clips = instance.sfxSounds[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Count)];
        instance.sfxAudioSource.PlayOneShot(randomClip, volume);
    }
}

[System.Serializable]
public struct SoundList {
    public SoundType Type;
    public List<AudioClip> Sounds;

}
