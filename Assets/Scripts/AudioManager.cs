using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager Instance { get; set; }
    private AudioSource audioSource;
    [SerializeField] AudioClip backgroundMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
        else
        {
            Debug.Log("No background music");
        }
    }

    static public void PlayAudioClipOneShot(AudioClip audioClip)
    {
        Instance.audioSource.PlayOneShot(audioClip);
    }
}
