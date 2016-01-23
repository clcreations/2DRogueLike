using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    void Start(){
        if (instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip){
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips){
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        sfxSource.pitch = randomPitch;
        sfxSource.clip = clips[randomIndex];
        sfxSource.Play();
    }

} // public class SoundManager : MonoBehaviour {
