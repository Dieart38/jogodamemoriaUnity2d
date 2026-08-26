using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// [System.Serializable] permite que essa classe apareça no Inspector da Unity
[System.Serializable]
public class PersonagemLore
{
    public string nomePersonagem;
    public Sprite arteDaCarta;

    [TextArea(3, 5)] // Cria uma caixa de texto maior no Inspector
    public string descricao;
}

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("UI do Loading")]
    public GameObject painelTransicao;
    public Image imgCartaExpandida;
    public Text txtDescricao; // Se usar TextMeshPro, mude para TMP_Text
    public Slider barraCarregamento;

    [Header("Banco de Dados (Lore)")]
    public float tempoDeCarregamento = 3.5f;
    public List<PersonagemLore> listaDePersonagens;

    // Esta é a nossa "Sacola de Sorteio" (Shuffle Bag)
    private List<int> sacolaDeIndices = new List<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ADICIONE ESTA LINHA:
            DontDestroyOnLoad(gameObject); // Faz o TransitionManager sobreviver à troca de cenas!
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (painelTransicao != null) painelTransicao.SetActive(false);
        ResetarSacola();
    }

    // Enche a sacola com os números de 0 até o total de personagens
    public void ResetarSacola()
    {
        sacolaDeIndices.Clear();
        for (int i = 0; i < listaDePersonagens.Count; i++)
        {
            sacolaDeIndices.Add(i);
        }
    }

    // Recebe um System.Action (um comando de retorno) para rodar quando terminar
    public void IniciarTransicao(System.Action acaoPosLoading)
    {
        StartCoroutine(RotinaDeLoading(acaoPosLoading));
    }

    private IEnumerator RotinaDeLoading(System.Action acaoPosLoading)
    {
        // 1. BLINDAGEM DE TRANSIÇÃO: Liga o painel se ele existir
        if (painelTransicao != null) 
        {
            painelTransicao.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Painel de transição não está linkado no Inspector!");
        }

        // 2. LÓGICA DO SHUFFLE BAG E TEXTOS (Protegido contra lista vazia)
        if (listaDePersonagens != null && listaDePersonagens.Count > 0)
        {
            if (sacolaDeIndices.Count == 0)
            {
                ResetarSacola(); 
            }

            int indexSorteado = Random.Range(0, sacolaDeIndices.Count);
            int idDoPersonagem = sacolaDeIndices[indexSorteado];
            sacolaDeIndices.RemoveAt(indexSorteado); 

            PersonagemLore loreSorteada = listaDePersonagens[idDoPersonagem];

            if (imgCartaExpandida != null && loreSorteada != null) 
                imgCartaExpandida.sprite = loreSorteada.arteDaCarta;
                
            if (txtDescricao != null && loreSorteada != null) 
                txtDescricao.text = $"<b>{loreSorteada.nomePersonagem}</b>\n\n{loreSorteada.descricao}";
        }

        // 3. ANIMAÇÃO DA BARRA DE PROGRESSO
        if (barraCarregamento != null) 
            barraCarregamento.value = 0f;

        float tempoGasto = 0f;
        while (tempoGasto < tempoDeCarregamento)
        {
            // Usamos unscaledDeltaTime para a barra carregar mesmo se o jogo estiver pausado (Time.timeScale = 0)
            tempoGasto += Time.unscaledDeltaTime;
            
            if (barraCarregamento != null)
            {
                barraCarregamento.value = tempoGasto / tempoDeCarregamento; 
            }
            
            yield return null; 
        }

        // 4. FINALIZAÇÃO
        if (painelTransicao != null) 
            painelTransicao.SetActive(false);
        
        // Executa a ação para gerar a próxima fase!
        acaoPosLoading?.Invoke(); 
    }
}