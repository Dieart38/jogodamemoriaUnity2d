using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Configurações de Áudio")]
    public AudioSource musicSource;
    public List<AudioClip> listaDeMusicas;

    [Header("Opções")]
    public bool tocarAoIniciar = true;

    private void Awake()
    {
        // Padrão Singleton: Garante que só exista um AudioManager e ele não morra ao trocar de cena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (tocarAoIniciar && listaDeMusicas.Count > 0)
        {
            TocarMusicaAleatoria();
        }
    }

    public void TocarMusicaAleatoria()
    {
        if (listaDeMusicas.Count == 0) return;

        // Escolhe um índice aleatório da lista
        int indiceAleatorio = Random.Range(0, listaDeMusicas.Count);
        AudioClip musicaSelecionada = listaDeMusicas[indiceAleatorio];

        // Configura e toca
        musicSource.clip = musicaSelecionada;
        musicSource.loop = true; // Mantém a música em loop até o próximo "Reiniciar"
        musicSource.Play();
    }

    // Método para ser chamado quando o jogo reiniciar
    public void ReiniciarMusica()
    {
        musicSource.Stop();
        TocarMusicaAleatoria();
    }
}