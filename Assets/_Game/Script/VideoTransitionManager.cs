using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class VideoTransitionManager : MonoBehaviour
{
    public static VideoTransitionManager Instance;

    public VideoPlayer videoPlayer;
    public VideoClip[] videosDownload; // Arraste os seus MP4s para aqui no Inspector
    public GameObject painelVideo;     // O objeto da UI que contém a Raw Image

    void Awake()
    {
        Instance = this;
        painelVideo.SetActive(false);
        // Subscreve ao evento que avisa quando o vídeo termina
        videoPlayer.loopPointReached += AoTerminarVideo;
    }

    public void IniciarTransicao()
    {
        if (videosDownload.Length == 0) return;

        painelVideo.SetActive(true);

        // Escolhe um vídeo aleatório da lista
        int indiceAleatorio = Random.Range(0, videosDownload.Length);
        videoPlayer.clip = videosDownload[indiceAleatorio];

        videoPlayer.Play();
    }

    void AoTerminarVideo(VideoPlayer vp)
    {
        painelVideo.SetActive(false);
        // Aqui chamamos a lógica para começar a próxima fase
        GameManager.Instance.ProximoNivelAposVideo();
    }
}