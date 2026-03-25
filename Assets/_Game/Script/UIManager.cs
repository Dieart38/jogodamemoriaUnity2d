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
    public TMP_Text txtTentativas;
    public TMP_Text txtPares;
    public TMP_Text txtTempo;

    [Header("Paineis")]
    public GameObject vitoriaPanel;
    public GameObject gameOverPanel;

    [Header("Textos do Paineis")]
    public TMP_Text txtResultado; // dentro do vitoriaPanel

    [Header("Painel de Recordes")]
    public GameObject recordesPanel;
    public TMP_Text txtListaRecordes; // Um único texto grande ou vários
    public InputField inputNome; // InputField configurado para 3 caracteres
    public GameObject inputScorePanel; // Painel que aparece na vitória para digitar o nome
    public Button btnConfirmar; // Botão para confirmar o nome e salvar o recorde

    [Header("Tela de Saída")]
    public GameObject painelAgradecimento;
    public float tempoDeEspera = 3.0f; // Quanto tempo a mensagem fica na tela

    private void Awake()
    {
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

    public void MostrarVitoria(int tentativas)
    {
        txtResultado.text = $"Parabéns! \n Você Completou em {tentativas} tentativas! e em {60 - int.Parse(txtTempo.text.Replace("Tempo: ", ""))} segundos!";
        vitoriaPanel.SetActive(true);
        inputScorePanel.SetActive(true);
        btnConfirmar.enabled = true; // Habilita o botão para confirmar o nome do recorde
    }

    public void MostrarGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void EsconderPaineis()
    {
        vitoriaPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }
    // Chamado pelo botao de reiniciar (configurado no inspector)
    public void BotaReiniciar()
    {
        // Chama a música aleatória antes de reiniciar a lógica do jogo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReiniciarMusica();
        }

        GameManager.Instance.Reiniciar();
        GameManager.Instance.Reiniciar();
    }

    public void SairDoJogo()
    {
        recordesPanel.SetActive(false);
        vitoriaPanel.SetActive(false);
        StartCoroutine(SequenciaDeSaida());

    }


    // Chame isso quando o jogador clicar no botão "OK" após digitar o nome
    public void ConfirmarRecorde()
    {
        string nome = inputNome.text.ToUpper();
        if (nome.Length < 1) nome = "AAA";

        int tempoAtual = 60 - int.Parse(txtTempo.text.Replace("Tempo: ", ""));
        int tentativasAtuais = int.Parse(txtTentativas.text.Replace("Tentativas: ", ""));

        ScoreManager.SalvarRecorde(nome, tempoAtual, tentativasAtuais);
        inputScorePanel.SetActive(false);
        btnConfirmar.enabled = false; // Desabilita o botão após confirmar
        MostrarPainelRecordes(); // Abre a lista atualizada
    }

    public void MostrarPainelRecordes()
    {
        EsconderPaineis();
        recordesPanel.SetActive(true);
        var lista = ScoreManager.CarregarRecordes();

        txtListaRecordes.text = "TOP 6 RECORDES\n\n";
        foreach (var s in lista)
        {
            txtListaRecordes.text += $"{s.nome} - {s.tempo} segundos - {s.tentativas} tentativas\n";
        }
    }

    public void FecharPainelRecordes()

    {
        recordesPanel.SetActive(false);
        vitoriaPanel.SetActive(true);
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
