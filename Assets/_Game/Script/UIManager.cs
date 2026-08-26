using System;
using TMPro;
using UnityEngine;
using UnityEditor; // Necessário para fechar o jogo no Editor
using UnityEngine.UI; // Necessário para o InputField
using System.Collections; // Necessário para as corrotinas

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Textos do HUD")]
    public Text txtTentativas;
    public Text txtPares;
    public Text txtTempo;
    public Text txtVidas;
    public Text txtVitorias;

    [Header("Sistema de Vidas (Modo 3)")]
    public GameObject containerCoracoes; // O objeto "pai" que segura todos os corações
    public Image[] arrayCoracoes; // Array que vai guardar as 7 imagens dos corações

    [Header("Elementos de Modos")]
    // Variável para agrupar tudo de vida (O texto "Vidas" e os corações)
    public GameObject containerHUDVidas;

    [Header("Paineis")]
    public GameObject vitoriaPanel;
    public GameObject gameOverPanel;
    public bool gameoverCont = false;
    public GameObject PainelDePause;

    [Header("Textos do Paineis")]
    public Text txtVitoria; // dentro do vitoriaPanel

    [Header("Painel de Recordes")]
    public GameObject recordesPanel;
    public Text txtListaRecordes; // Um único texto grande ou vários
    public InputField inputNome; // InputField configurado para 3 caracteres
    public GameObject inputScorePanel; // Painel que aparece na vitória para digitar o nome
    public Button btnConfirmar; // Botão para confirmar o nome e salvar o recorde

    [Header("Tela de Saída")]
    public GameObject painelAgradecimento;
    public float tempoDeEspera = 3.0f; // Quanto tempo a mensagem fica na tela

    private void Awake()
    {
        gameoverCont = false; // Garante que o gameoverCont comece como false
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AtualizarTentativas(int valor)
    {
        txtTentativas.text = $"Tentativas: {valor}";
    }

    public void AtualizarPares(int encontrados, int total)
    {
        txtPares.text = $"Pares: {encontrados} / {total}";
    }

    public void AtualizarTempo(float segundos)
    {
        int s = Mathf.CeilToInt(segundos);
        txtTempo.text = $"Tempo: {s}";
        // Fica vermelho nos ultimos 10 segundos
        txtTempo.color = s <= 10 ? Color.red : Color.yellow;
    }

    public void AtualizarVitorias(int valor)
    {
        txtVitorias.text = $"Vitórias: {valor}";
    }

    public void MostrarVitoria(int tentativas)
    {
        txtVitoria.text = $"Parabéns! \n Você Completou em {tentativas} tentativas!";
        vitoriaPanel.SetActive(true);
        //inputScorePanel.SetActive(true);
        //btnConfirmar.enabled = true; // Habilita o botão para confirmar o nome do recorde
    }

    public void MostrarGameOver()
    {

        gameoverCont = true;
        gameOverPanel.SetActive(true);
        inputScorePanel.SetActive(true);
        btnConfirmar.gameObject.SetActive(true); // Habilita o botão para confirmar o nome do recorde
        GameManager.Instance.musicaGameOver.Play();
    }


    public void EsconderPaineis()
    {
        vitoriaPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        recordesPanel.SetActive(false);
        //PainelDePause.SetActive(false);
    }
    // Chamado pelo botao de reiniciar (configurado no inspector)
    public void BotaReiniciar()
    {
        gameoverCont = false;
        // Chama a música aleatória antes de reiniciar a lógica do jogo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReiniciarMusica();
        }


        GameManager.Instance.Reiniciar();
    }

    public void SairDoJogo()
    {
        recordesPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        PainelDePause.SetActive(false); StartCoroutine(SequenciaDeSaida());


    }


    // Chame isso quando o jogador clicar no botão "OK" após digitar o nome
    public void ConfirmarRecorde()
    {
        string nome = inputNome.text.ToUpper();
        if (string.IsNullOrEmpty(nome)) nome = "AAA";

        // Use as variáveis estáticas que guardam o progresso de TODAS as fases
        int tempoFinal = GameManager.tempoTotalSessao;
        int tentFinal = GameManager.tentativasTotaisSessao;
        int fasesFinal = GameManager.vitoria;

        ScoreManager.SalvarRecorde(nome, tempoFinal, tentFinal, fasesFinal);

        inputScorePanel.SetActive(false);
        btnConfirmar.gameObject.SetActive(false);


        MostrarPainelRecordes();
    }

    public void MostrarPainelRecordes()
    {
        EsconderPaineis();
        recordesPanel.SetActive(true);
        var lista = ScoreManager.CarregarRecordes();

        txtListaRecordes.text = "<color=#FFD700>RANKING Weow-Moria</color>\n\n";

        foreach (var s in lista)
        {
            // Usando interpolação de string para um visual mais limpo
            txtListaRecordes.text += $"{s.nome} | {s.fasesJogadas} Fases | {s.tempo}s | {s.tentativas} Tent.\n";
        }
    }
    public void FecharPainelRecordes()

    {
        recordesPanel.SetActive(false);
        if (gameoverCont == true)
        {
            gameOverPanel.SetActive(true);
            GameManager.Instance.musicaGameOver.Play();
        }
        else if (vitoriaPanel == true) vitoriaPanel.SetActive(true);
    }
    public IEnumerator SequenciaDeSaida()
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
        // 4. conta 1 segundo antes de fechar o jogo
        yield return new WaitForSeconds(0.5f);
        // Encerra a aplicação/jogo

        Application.Quit();
        // O Application.Quit() não funciona dentro do Editor da Unity.
        // A linha abaixo serve para testar o fechamento enquanto você joga no Editor:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ProximoNivel()
    {
        // Parar/Reiniciar áudio se necessário
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReiniciarMusica();
        }

        // Chama o próximo nível através do GameManager (que vai rodar o TransitionManager)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProximoNivel();
        }
    }
    public void PausarJogo()
    {
        if (PainelDePause != null)
        {
            PainelDePause.SetActive(true);
        }
        Time.timeScale = 0f; // Congela o tempo do jogo
    }

    public void AtualizarVidas(int qtdVidas)
    {
        if (arrayCoracoes == null || arrayCoracoes.Length == 0) return;

        for (int i = 0; i < arrayCoracoes.Length; i++)
        {
            // Garante que o objeto esteja sempre ligado para não quebrar o layout
            arrayCoracoes[i].gameObject.SetActive(true);

            if (i < qtdVidas)
            {
                // VIDA CHEIA: Cor normal (Branco puro, 100% opaco)
                arrayCoracoes[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                // VIDA VAZIA: Fica escuro e meio transparente (Efeito de coração vazio)
                // Os valores são (Red, Green, Blue, Alpha). Tudo 0.2f é um cinza bem escuro, 0.5f é a transparência.
                arrayCoracoes[i].color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }
        }
    }
    // public void ConfigurarHUDPorModo(int modo)
    // {
    //     if (modo == 2) // Casual
    //     {
    //         if (txtTempo != null) txtTempo.gameObject.SetActive(false);
    //         if (containerCoracoes != null) containerCoracoes.SetActive(false);
    //     }
    //     else if (modo == 3) // Sobrevivência
    //     {
    //         if (txtTempo != null) txtTempo.gameObject.SetActive(false);

    //         // Liga o container com os corações!
    //         if (containerCoracoes != null) containerCoracoes.SetActive(true);
    //     }
    //     else // Modos 1 e 4 (Tempo)
    //     {
    //         if (txtTempo != null) txtTempo.gameObject.SetActive(true);
    //         if (containerCoracoes != null) containerCoracoes.SetActive(false);
    //     }
    // }

    public void ConfigurarHUDPorModo(int modo)
    {
        // Regras lógicas:
        // Modo 1 e 4 usam Tempo. Modos 2 e 3 NÃO usam tempo.
        bool usaTempo = (modo == 1 || modo == 4);
        
        // Apenas o Modo 3 usa Vidas.
        bool usaVidas = (modo == 3);

        // Liga ou desliga o texto de Tempo
        if (txtTempo != null)
        {
            txtTempo.gameObject.SetActive(usaTempo);
        }

        // Liga ou desliga as Vidas inteiras (Texto + Corações)
        if (containerHUDVidas != null)
        {
            containerHUDVidas.SetActive(usaVidas);
        }
    }


}
