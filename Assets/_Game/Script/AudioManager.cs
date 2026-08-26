using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Configurações de Áudio")]
    public AudioSource musicSource;
    public AudioClip musicaDoMenu; // <- Nova variável para a música fixa do menu
    public List<AudioClip> listaDeMusicas;

    [Header("Opções")]
    public bool tocarAoIniciar = true;

    private void Awake()
    {
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
        // Se estiver iniciando o jogo pelo menu, toca a música fixa do menu
        if (tocarAoIniciar)
        {
            TocarMusicaDoMenu();
        }
    }

    // Método específico para o Menu Iniciar
    // public void TocarMusicaDoMenu()
    // {
    //     if (musicaDoMenu == null) return;

    //     // Só troca a música se ela já não estiver tocando (evita reiniciar do zero sem querer)
    //     if (musicSource.clip == musicaDoMenu && musicSource.isPlaying) return;

    //     musicSource.clip = musicaDoMenu;
    //     musicSource.loop = true;
    //     musicSource.pitch = 1f; // Reseta a velocidade caso tenha mudado na gameplay
    //     musicSource.Play();
    // }

    // Método para as fases (escolhe uma aleatória da lista)
    public void TocarMusicaAleatoria()
    {
        if (listaDeMusicas.Count == 0) return;

        int indiceAleatorio = Random.Range(0, listaDeMusicas.Count);
        AudioClip musicaSelecionada = listaDeMusicas[indiceAleatorio];

        musicSource.clip = musicaSelecionada;
        musicSource.loop = true; 
        musicSource.Play();
    }

    public void ReiniciarMusica()
    {
        musicSource.Stop();
        TocarMusicaAleatoria();
    }

    public void DefinirVelocidadeMusica(float velocidade)
    {
        if (musicSource != null)
        {
            musicSource.pitch = velocidade;
        }
    }

    public void PararMusica()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void TocarMusicaDoMenu()
    {
        // Nota: Troque "audioSource" pelo nome da sua variável de AudioSource de música, se for diferente (ex: bgmSource, musicaSource)
        if (musicaDoMenu != null)
        {
            // Só troca a música se não estiver tocando ela já
            if (musicSource.clip != musicaDoMenu) 
            {
                musicSource.clip = musicaDoMenu;
                musicSource.Play();
            }
        }
    }
}