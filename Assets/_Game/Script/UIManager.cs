using System;
using TMPro;
using UnityEngine;

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
        txtTempo.color = s <= 10 ? Color.red : Color.white;
    }

    public void MostrarVitoria(int tentativas)
    {
        txtResultado.text = $"Completado em {tentativas} tentativas!";
        vitoriaPanel.SetActive(true);
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
        GameManager.Instance.Reiniciar();
    }

}
