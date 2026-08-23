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
        // 1. LÓGICA DO SHUFFLE BAG (Aleatório sem repetição)
        if (sacolaDeIndices.Count == 0)
        {
            ResetarSacola(); // Se a sacola esvaziou, enche de novo!
        }

        // Puxa um "papelzinho" aleatório da sacola
        int indexSorteado = Random.Range(0, sacolaDeIndices.Count);
        int idDoPersonagem = sacolaDeIndices[indexSorteado];

        // Joga o "papelzinho" fora para não repetir
        sacolaDeIndices.RemoveAt(indexSorteado);

        // 2. ATUALIZA A INTERFACE GRÁFICA
        PersonagemLore loreSorteada = listaDePersonagens[idDoPersonagem];
        imgCartaExpandida.sprite = loreSorteada.arteDaCarta;
        txtDescricao.text = $"<b>{loreSorteada.nomePersonagem}</b>\n\n{loreSorteada.descricao}";
        barraCarregamento.value = 0f;

        painelTransicao.SetActive(true);

        // 3. ANIMAÇÃO DA BARRA DE PROGRESSO
        float tempoGasto = 0f;
        while (tempoGasto < tempoDeCarregamento)
        {
            tempoGasto += Time.unscaledDeltaTime;
            // Interpola o valor do slider de 0 a 1 suavemente
            barraCarregamento.value = tempoGasto / tempoDeCarregamento;
            yield return null; // Espera o próximo frame
        }

        // 4. FINALIZAÇÃO
        painelTransicao.SetActive(false);

        // Chama o comando que o GameManager enviou (ex: ConfigurarDificuldade)
        acaoPosLoading?.Invoke();
    }
}