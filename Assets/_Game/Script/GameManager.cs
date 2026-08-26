using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; // Necessário para fechar o jogo no Editor



public class GameManager : MonoBehaviour
{
    [Header("Modos de Jogo")]
    public int modoAtual = 1; // 1=Clássico, 2=Casual, 3=Sobrevivência, 4=Time Attack
    public int vidas = 7;
    public int maxVidas = 7;
    public int vitorias = 0; // Contador de vitórias para o modo 3 (Sobrevivência)


    [Header("Rastreio de Objetos")]
    private List<Card> cartasEmJogo = new List<Card>();
    public static GameManager Instance { get; private set; }

    [Header("Configuracao do Grid")]
    public Transform pontoDeOrigemGrid;
    public int colunas;
    public int linhas;
    public float espacoHorizontal = 1.2f; // Novo 
    public float espacoVertical = 1.5f;   // Novo 

    [Header("Prefabs e Sprites")]
    public GameObject cardPrefab;
    public Sprite[] cardSprite; // 8 sprites, um por um

    [Header("Estado do Jogo")]
    public int tentativas = 0;
    public int paresEncontrados = 0;
    public float tempoRestante = 60f;
    public float tempoFaseInicial;
    public bool jogoAtivo = false;
    public static int vitoria = 0;
    [Header("Acumuladores de Sessão")]
    public static int tempoTotalSessao = 0;
    public static int tentativasTotaisSessao = 0;

    [Header("Efeitos Visuais")]
    public GameObject matchEffectPrefab; // Arraste seu prefab de explosão aqui no Inspector
    private Card primeiraCarta;
    private Card segundaCarta;
    private bool aguardandoComparação = false;
    private int totalPares;

    private bool musicaAcelerada = false; // Flag para controlar a aceleração da música

    public AudioSource musicaGameOver; // Referência para o AudioSource da música

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        Cursor.visible = false;


    }

    void Start()
    {
        // Lê o modo que o jogador escolheu
        modoAtual = PlayerPrefs.GetInt("ModoDeJogo", 1);

        if (modoAtual == 3) vidas = maxVidas;

        // NOVO: Pede para o UIManager ligar e desligar as coisas certas na tela!
        UIManager.Instance.ConfigurarHUDPorModo(modoAtual);

        tempoRestante = 60f;
        vitoria = 0;
        ConfigurarDificuldade();
        IniciarJogo();
    }
    // Update is called once per frame
    void Update()
    {
        if (!jogoAtivo) return;

        // O tempo SÓ DIMINUI se for o Modo 1 (Clássico) ou Modo 4 (Time Attack)
        if (modoAtual == 1 || modoAtual == 4)
        {
            tempoRestante -= Time.deltaTime;
            UIManager.Instance.AtualizarTempo(tempoRestante);

            if (tempoRestante <= 10f && !musicaAcelerada)
            {
                musicaAcelerada = true;
                if (AudioManager.Instance != null) AudioManager.Instance.DefinirVelocidadeMusica(1.25f);
            }

            if (tempoRestante <= 0f)
            {
                tempoRestante = 0f;
                if (AudioManager.Instance != null) AudioManager.Instance.PararMusica();
                GameOver();
            }
        }
    }

    private void ConfigurarDificuldade()
    {
        linhas = 4; // Padrão garantido

        if (vitoria <= 3)
        {
            // Fases Iniciais (Curva de aprendizado fixa)
            switch (vitoria)
            {
                case 0: colunas = 3; tempoRestante = 60f; break; // 12 cartas
                case 1: colunas = 4; tempoRestante = 60f; break; // 16 cartas
                case 2: colunas = 5; tempoRestante = 60f; break; // 20 cartas
                case 3: colunas = 6; tempoRestante = 80f; break; // 24 cartas
            }
        }
        else
        {
            // Fases Avançadas (Dificuldade Infinita a partir da vitória 4)

            // Sorteia a quantidade de colunas entre 3 e 6 (resultado sempre par com as 4 linhas)
            colunas = Random.Range(3, 7);

            // CÁLCULO DINÂMICO DE TEMPO: 
            // Começa com 50 segundos na vitória 4, e vai caindo 2 segundos a cada vitória nova.
            // O Mathf.Max garante que o tempo NUNCA fique menor que 20 segundos (para não ficar impossível).
            float tempoCalculado = 50f - ((vitoria - 4) * 2f);
            tempoRestante = Mathf.Max(20f, tempoCalculado);
        }

        // Guarda o tempo inicial para os cálculos de pontuação no final da fase
        tempoFaseInicial = tempoRestante;

        // Calcula o total de pares
        totalPares = (colunas * linhas) / 2;

        Debug.Log($"Dificuldade configurada: {colunas}x{linhas} com {tempoRestante}s | Vitória atual: {vitoria}");
    }
    public void IniciarJogo()
    {
        LimparGrid();
        musicaAcelerada = false;

        // Verificação de segurança original sua
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DefinirVelocidadeMusica(1.0f);
            AudioManager.Instance.TocarMusicaAleatoria();
        }
        else
        {
            Debug.LogWarning("AudioManager não encontrado. Lembre-se de iniciar o jogo pela cena do Menu para a música funcionar!");
        }

        tentativas = 0;
        paresEncontrados = 0;

        // 1. A MUDANÇA ESTÁ AQUI: Antes era true, agora começa false!
        jogoAtivo = false;

        UIManager.Instance.AtualizarTentativas(tentativas);
        UIManager.Instance.AtualizarPares(paresEncontrados, totalPares);
        UIManager.Instance.AtualizarVitorias(vitoria);

        if (modoAtual == 3)
        {
            UIManager.Instance.AtualizarVidas(vidas);

        }

        CriarGrid();

        // 2. A MUDANÇA ESTÁ AQUI: Iniciamos a memorização no final
        StartCoroutine(RotinaDeMemorizacao());
    }
    private void LimparGrid()
    {
        // Apaga todas as cartas que estão na nossa lista
        foreach (Card c in cartasEmJogo)
        {
            if (c != null) Destroy(c.gameObject);
        }
        cartasEmJogo.Clear(); // Limpa a lista para a próxima fase
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

        // 1. Pega a posição do nosso guia
        Vector3 centroBase = pontoDeOrigemGrid != null ? pontoDeOrigemGrid.position : Vector3.zero;

        float larguraTotal = (colunas - 1) * espacoHorizontal;
        float alturaTotal = (linhas - 1) * espacoVertical;

        // 2. A CORREÇÃO ESTÁ AQUI: Agora usamos o centroBase.x e centroBase.y!
        float startX = centroBase.x - (larguraTotal / 2f);
        float startY = centroBase.y + (alturaTotal / 2f);

        int index = 0;
        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                float posX = startX + (c * espacoHorizontal);
                float posY = startY - (l * espacoVertical);
                Vector3 pos = new Vector3(posX, posY, 0f);

                GameObject obj = Instantiate(cardPrefab, pos, Quaternion.identity);
                Card card = obj.GetComponent<Card>();

                if (index < ids.Count && ids[index] < cardSprite.Length)
                {
                    card.cardID = ids[index];
                    card.frontSprite = cardSprite[ids[index]];
                }

                obj.name = $"Card_{ids[index]}_{index}";

                cartasEmJogo.Add(card);

                index++;
            }
        }
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
            if (modoAtual == 4)
            {
                tempoRestante += 3f; // Modo 4: Ganha 3 segundos extras no acerto!
                UIManager.Instance.AtualizarTempo(tempoRestante);
            }

            // INSTANCIAR EXPLOSÃO EM CADA CARTA
            Instantiate(matchEffectPrefab, primeiraCarta.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab, segundaCarta.transform.position, Quaternion.identity);

            // Chamar o método para a carta sumir
            primeiraCarta.RecolherCarta();
            segundaCarta.RecolherCarta();

            paresEncontrados++;
            UIManager.Instance.AtualizarPares(paresEncontrados, totalPares);

            if (paresEncontrados == totalPares)
            {
                Vitoria();
            }
        }
        else
        {
            // Erro tremer a tela e tocar som de erro

            SoundManager.Instance.playLose();
            if (modoAtual == 3)
            {
                vidas--; // Modo 3: Perde vida no erro!
                UIManager.Instance.AtualizarVidas(vidas);
                if (vidas <= 0)
                {
                    GameOver();
                }
            }
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Tremer(0.2f, 0.1f); // Duração e magnitude do tremor
            }
            primeiraCarta.FlipToBack();
            segundaCarta.FlipToBack();
        }

        primeiraCarta = null;
        segundaCarta = null;
        aguardandoComparação = false;
    }



    private void Vitoria()
    {
        // 1. PRIMEIRO: Calcula e acumula
        tentativasTotaisSessao += tentativas; // tentativas da fase atual

        float tempoGastoNestaFase = tempoFaseInicial - tempoRestante;
        tempoTotalSessao += Mathf.RoundToInt(tempoGastoNestaFase);

        // 2. DEPOIS: Aumenta o nível e para o jogo
        vitoria++;
        jogoAtivo = false;

        // 3. POR FIM: Mostra a UI
        SoundManager.Instance.playWin();
        if (modoAtual == 3)
        {
            vidas += 2; // Recupera 2 vidas ao passar de fase
            if (vidas > maxVidas) vidas = maxVidas; // Limita ao máximo de 7
            UIManager.Instance.AtualizarVidas(vidas);
        }
        UIManager.Instance.MostrarVitoria(tentativas);



    }

    private void GameOver()
    {
        jogoAtivo = false;
        UIManager.Instance.MostrarGameOver();
        // tocar musica de game over
        if (musicaGameOver != null)
        {
            if (UIManager.Instance.gameoverCont == true) { musicaGameOver.Play(); }

        }

    }

    public void Reiniciar()
    {
        vitoria = 0;
        tempoTotalSessao = 0;
        tentativasTotaisSessao = 0;
        vidas = maxVidas;

        if (AudioManager.Instance != null) AudioManager.Instance.DefinirVelocidadeMusica(1.0f);
        UIManager.Instance.EsconderPaineis();

        // Se reiniciou, queremos resetar a memória das cartas que já apareceram no loading
        if (TransitionManager.Instance != null) TransitionManager.Instance.ResetarSacola();

        // Dispara o loading e, quando terminar, executa o método ConfigurarEIniciar
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.IniciarTransicao(ConfigurarEIniciar);
        }
        else
        {
            // Fallback caso esqueça de colocar o script na cena
            ConfigurarEIniciar();
        }
    }

    public void ProximoNivel()
    {
        UIManager.Instance.EsconderPaineis();

        // Garante que a velocidade da música volte ao normal (caso estivesse acelerada)
        if (AudioManager.Instance != null)
            AudioManager.Instance.DefinirVelocidadeMusica(1.0f);

        // Chama a transição com a tela de loading para TODOS os modos
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.IniciarTransicao(ConfigurarEIniciar);
        }
        else
        {
            // Fallback de segurança se o TransitionManager não for encontrado
            ConfigurarEIniciar();
        }
    }

    // NOVO MÉTODO: Agrupa as ações pós-loading
    private void ConfigurarEIniciar()
    {
        ConfigurarDificuldade();
        IniciarJogo();
    }

    private IEnumerator RotinaDeMemorizacao()
    {
        yield return new WaitForSeconds(0.5f);

        SoundManager.Instance.PlayFlip();

        // Usamos a nossa própria lista em vez de forçar a engine a procurar na cena!
        foreach (Card carta in cartasEmJogo) carta.FlipToFront();

        yield return new WaitForSeconds(2.5f);
        SoundManager.Instance.PlayFlip();

        foreach (Card carta in cartasEmJogo) carta.FlipToBack();

        yield return new WaitForSeconds(0.35f);
        jogoAtivo = true;
    }


}
