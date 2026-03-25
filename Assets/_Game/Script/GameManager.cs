using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; // Necessário para fechar o jogo no Editor



public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Configuracao do Grid")]
    public int colunas = 4;
    public int linhas = 4;
    public float espacoHorizontal = 1.2f; // Novo 
    public float espacoVertical = 1.5f;   // Novo 

    [Header("Prefabs e Sprites")]
    public GameObject cardPrefab;
    public Sprite[] cardSprite; // 8 sprites, um por um

    [Header("Estado do Jogo")]
    public int tentativas = 0;
    public int paresEncontrados = 0;
    public float tempoRestante = 60f;
    public bool jogoAtivo = false;
    public int vitoria = 0;

    private Card primeiraCarta;
    private Card segundaCarta;
    private bool aguardandoComparação = false;
    private int totalPares;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); return;
        }
        Instance = this;

    }

    void Start()
    {
       
         if (vitoria == 0)
        {
            colunas = 3;
        }
        else if (vitoria == 1)
        {
            colunas = 4;
            linhas = 4;
        }
        else if (vitoria == 2)
        {
            colunas = 5;
            linhas = 4;
        }
        else if (vitoria == 3)
        {
            colunas = 6;
            linhas = 4;
        }
         totalPares = (colunas * linhas) / 2;
        IniciarJogo();
    }

    // Update is called once per frame
    void Update()
    {
        if (!jogoAtivo) return;
        tempoRestante -= Time.deltaTime;
        UIManager.Instance.AtualizarTempo(tempoRestante);
        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            GameOver();
        }
    }

    public void IniciarJogo()
    {
        LimparGrid();
        tentativas = 0;
        paresEncontrados = 0;
        tempoRestante = 60f;
        jogoAtivo = true;
        UIManager.Instance.AtualizarTentativas(tentativas);
        UIManager.Instance.AtualizarPares(paresEncontrados, totalPares);

        CriarGrid();

    }


    private void CriarGrid()
    {

        List<int> ids = new List<int>();
        for (int i = 0; i < totalPares; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        Embaralhar(ids);

        // Calcula usando os novos espaços
        float larguraTotal = (colunas - 1) * espacoHorizontal;
        float alturaTotal = (linhas - 1) * espacoVertical;

        float startX = -larguraTotal / 2f;
        float startY = alturaTotal / 2f;

        int index = 0;
        for (int l = 0; l < linhas; l++)
        {
           
            for (int c = 0; c < colunas; c++)
            {
                // X usa espacoHorizontal, Y usa espacoVertical
                float posX = startX + (c * espacoHorizontal);
                float posY = startY - (l * espacoVertical);

                Vector3 pos = new Vector3(posX, posY, 0f);

                GameObject obj = Instantiate(cardPrefab, pos, Quaternion.identity);
                Card card = obj.GetComponent<Card>();

                if (index < ids.Count && ids[index] < cardSprite.Length)
                {
                    card.cardID = ids[index];
                    card.fronSprite = cardSprite[ids[index]];
                }

                obj.name = $"Card_{ids[index]}_{index}";
                index++;
            }
        }
    }
    private void LimparGrid()
    {
        Card[] cartas = FindObjectsByType<Card>(
            FindObjectsSortMode.None);
        foreach (var c in cartas)
            Destroy(c.gameObject);
    }

    private void Embaralhar(List<int> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }

    public bool CanFlip()
    {
        return !aguardandoComparação && jogoAtivo;
    }

    public void CardFlipped(Card carta)
    {
        if (primeiraCarta == null)
        {
            primeiraCarta = carta;
        }
        else
        {
            segundaCarta = carta;
            tentativas++;
            UIManager.Instance.AtualizarTentativas(tentativas);
            aguardandoComparação = true;
            SoundManager.Instance.PlayFlip();
            StartCoroutine(CompararCartas());
        }
    }

    private IEnumerator CompararCartas()
    {
        yield return new WaitForSeconds(0.8f);
        if (primeiraCarta.cardID == segundaCarta.cardID)
        {
            // Par encontrado
            SoundManager.Instance.playMatch();
            primeiraCarta.SetMatched();
            segundaCarta.SetMatched();
            paresEncontrados++;
            UIManager.Instance.AtualizarPares(paresEncontrados, totalPares);

            if (paresEncontrados == totalPares)
            {
                Vitoria();
            }
        }
        else
        {
            //Sem par - vira de  volta
            primeiraCarta.FlipToBack();
            segundaCarta.FlipToBack();
        }

        primeiraCarta = null;
        segundaCarta = null;
        aguardandoComparação = false;
    }

    private void Vitoria()
    {

        SoundManager.Instance.playWin();
        jogoAtivo = false;
        UIManager.Instance.MostrarVitoria(tentativas);
        vitoria++;

    }

    private void GameOver()
    {
        jogoAtivo = false;
        UIManager.Instance.MostrarGameOver();
    }

    public void Reiniciar()
    {
        UIManager.Instance.EsconderPaineis();
        IniciarJogo();
    }


}
