using UnityEngine;
using UnityEngine.UI;
using TMPro; // Mude para 'using UnityEngine.UI;' se estiver usando o componente Text padrão

public class GaleriaManager : MonoBehaviour
{
    [Header("Referências de UI")]
    public GameObject painelGaleria;
    public GameObject painelMenuPrincipal; // Para poder voltar
    public Image imgCartaGaleria;
    public Text txtDescricaoGaleria; // Se usar texto normal, mude para Text
    public Text txtContador;        // Ex: "1 / 12"

    [Header("Banco de Dados")]
    // Podemos puxar a mesma lista do TransitionManager para não duplicar dados!
    private int indiceAtual = 0;

    void Start()
    {
        if (painelGaleria != null) painelGaleria.SetActive(false);
    }

    // Chamado pelo botão "Galeria" no Menu Principal
    public void AbrirGaleria()
    {
        if (painelMenuPrincipal != null) painelMenuPrincipal.SetActive(false);
        if (painelGaleria != null) painelGaleria.SetActive(true);

        indiceAtual = 0;
        AtualizarGaleria();
    }

    // Chamado pelo botão "Voltar" dentro da Galeria
    public void FecharGaleria()
    {
        if (painelGaleria != null) painelGaleria.SetActive(false);
        if (painelMenuPrincipal != null) painelMenuPrincipal.SetActive(true);
    }

    // Chamado pelo botão "Avançar" (Próximo)
    public void ProximoPersonagem()
    {
        if (TransitionManager.Instance == null || TransitionManager.Instance.listaDePersonagens.Count == 0) return;

        indiceAtual++;
        if (indiceAtual >= TransitionManager.Instance.listaDePersonagens.Count)
        {
            indiceAtual = 0; // Volta para o primeiro (Loop infinito)
        }
        AtualizarGaleria();
    }

    // Chamado pelo botão "Voltar" (Anterior)
    public void PersonagemAnterior()
    {
        if (TransitionManager.Instance == null || TransitionManager.Instance.listaDePersonagens.Count == 0) return;

        indiceAtual--;
        if (indiceAtual < 0)
        {
            indiceAtual = TransitionManager.Instance.listaDePersonagens.Count - 1; // Vai para o último
        }
        AtualizarGaleria();
    }

    private void AtualizarGaleria()
    {
        var lista = TransitionManager.Instance.listaDePersonagens;
        if (lista == null || lista.Count == 0) return;

        PersonagemLore lore = lista[indiceAtual];

        // Atualiza a imagem e a descrição usando o mesmo formato do loading
        imgCartaGaleria.sprite = lore.arteDaCarta;
        txtDescricaoGaleria.text = $"<b>{lore.nomePersonagem}</b>\n\n{lore.descricao}";

        // Atualiza o contador de páginas (ex: 1/12)
        if (txtContador != null)
        {
            txtContador.text = $"{indiceAtual + 1} / {lista.Count}";
        }
    }
}