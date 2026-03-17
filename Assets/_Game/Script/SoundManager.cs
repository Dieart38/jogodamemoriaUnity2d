using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get; private set;}

    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip winClip;

    private AudioSource _source;

    private void Awake()
    {
        if(Instance != null){Destroy(gameObject); return;}
        Instance = this;
        _source = GetComponent<AudioSource>();
    }

    public void PlayFlip() => _source.PlayOneShot(flipClip);
    public void playMatch() => _source.PlayOneShot(matchClip);
    public void playWin() => _source.PlayOneShot(winClip);
}
