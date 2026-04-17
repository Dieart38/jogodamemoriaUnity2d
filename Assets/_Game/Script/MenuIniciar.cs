using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necessário para a Coroutine
using TMPro; // Necessário para o texto do agradecimento

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuIniciar : MonoBehaviour
{
    [Header("Configurações de Saída")]
    public GameObject painelAgradecimento;
    public TMP_Text txtAgradecimento; // Arraste o texto do painel aqui
    public float tempoDeEspera = 3.5f;

    void Awake()
    {
        // 1. ACESSO ESTÁTICO: Referenciamos a classe GameManager para zerar a vitória
        GameManager.vitoria = 0;

        if (painelAgradecimento != null)
        {
            painelAgradecimento.SetActive(false);
        }
    }

    public void Jogar()
    {
        // Opcional: Você também pode zerar aqui para garantir que 
        // toda vez que o botão "Jogar" for clicado, comece do nível 0
        GameManager.vitoria = 0;
        SceneManager.LoadScene("MainGame");
    }
    public void SairDoJogo()
    {
        // Iniciamos a sequência de despedida em vez de fechar direto
        StartCoroutine(SequenciaDeSaida());
    }
    void Update()
    {
        // Detecta o botão 'Voltar' do Android
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SairDoJogo();
        }
    }
    private IEnumerator SequenciaDeSaida()
    {
        // 1. Mostra o agradecimento
        if (painelAgradecimento != null)
        {
            painelAgradecimento.SetActive(true);
        }

        Debug.Log("Exibindo agradecimentos...");

        // 2. Espera o tempo definido (ex: 3 segundos)
        yield return new WaitForSeconds(tempoDeEspera);

        // 3. Fecha o jogo de verdade
        Debug.Log("Fechando o sistema.");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}