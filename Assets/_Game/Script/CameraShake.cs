using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Singleton para acesso rápido em qualquer lugar do jogo
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Chama este método para tremer. 
    // Ex: CameraShake.Instance.Tremer(0.2f, 0.1f);
    public void Tremer(float duracao, float magnitude)
    {
        // 1. ACESSIBILIDADE: Checa a preferência do jogador.
        // Se a chave "TremerTela" for 0 (Desligado), aborta o tremor imediatamente!
        // O valor padrão '1' garante que o tremor venha ligado na primeira vez que o cara jogar.
        if (PlayerPrefs.GetInt("TremerTela", 1) == 0) return;

        // Se estiver ligado (1), roda o tremor normal
        StopAllCoroutines(); 
        StartCoroutine(ExecutarTremor(duracao, magnitude));
    }

    private IEnumerator ExecutarTremor(float duracao, float magnitude)
    {
        // Salva a posição original da câmera
        Vector3 posicaoOriginal = transform.localPosition;
        float tempoGasto = 0.0f;

        while (tempoGasto < duracao)
        {
            // Sorteia uma posição aleatória num círculo
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Move a câmera
            transform.localPosition = new Vector3(posicaoOriginal.x + x, posicaoOriginal.y + y, posicaoOriginal.z);

            tempoGasto += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Devolve a câmera exatamente para o lugar original quando o tempo acabar
        transform.localPosition = posicaoOriginal;
    }
}