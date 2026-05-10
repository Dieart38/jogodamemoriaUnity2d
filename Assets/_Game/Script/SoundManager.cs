using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource audioSource;

    [Header("Configurações de Áudio")]
    // Mudamos de AudioClip para uma Array (lista)
    public AudioClip[] sonsDeAcerto; 
    public AudioClip somViraCarta;
    public AudioClip somVitoria;

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void playMatch()
    {
        if (sonsDeAcerto.Length > 0)
        {
            // Escolhe um número aleatório entre 0 e o tamanho da lista
            int indiceAleatorio = Random.Range(0, sonsDeAcerto.Length);
            
            // Toca o som escolhido sem interromper outros sons
            audioSource.PlayOneShot(sonsDeAcerto[indiceAleatorio]);
        }
    }

    public void PlayFlip()
    {
        audioSource.PlayOneShot(somViraCarta);
    }

    public void playWin()
    {
        audioSource.PlayOneShot(somVitoria);
    }
}