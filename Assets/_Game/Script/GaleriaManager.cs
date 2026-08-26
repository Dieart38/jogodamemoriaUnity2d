using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//using UnityEngine.UI; // Mude para UnityEngine.UI se usar Text normal


public class GaleriaManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelGaleria;
    public GameObject painelMenuPrincipal;

    [Header("Elementos de UI")]
    public Image imgCartaGaleria;
    public Text txtDescricaoGaleria; // Se for Text normal, mude o tipo
    public Text txtContador;        // Ex: "1 / 12"

    [Header("Banco de Dados Local (Lore)")]
    // A lista agora mora aqui dentro! Você preenche direto no Inspector desta cena.
    public List<PersonagemLore> listaDePersonagensLocal;

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
        if (listaDePersonagensLocal == null || listaDePersonagensLocal.Count == 0) return;

        indiceAtual++;
        if (indiceAtual >= listaDePersonagensLocal.Count)
        {
            indiceAtual = 0; // Volta para o primeiro (Loop)
        }
        AtualizarGaleria();
    }

    // Chamado pelo botão "Voltar" (Anterior)
    public void PersonagemAnterior()
    {
        if (listaDePersonagensLocal == null || listaDePersonagensLocal.Count == 0) return;

        indiceAtual--;
        if (indiceAtual < 0)
        {
            indiceAtual = listaDePersonagensLocal.Count - 1; // Vai para o último
        }
        AtualizarGaleria();
    }

    private void AtualizarGaleria()
    {
        if (listaDePersonagensLocal == null || listaDePersonagensLocal.Count == 0) return;

        PersonagemLore lore = listaDePersonagensLocal[indiceAtual];

        // Atualiza a imagem e a descrição
        if (imgCartaGaleria != null)
        {
            imgCartaGaleria.sprite = lore.arteDaCarta;
        }

        if (txtDescricaoGaleria != null)
        {
            txtDescricaoGaleria.text = $"<b>{lore.nomePersonagem}</b>\n\n{lore.descricao}";
        }

        // Atualiza o contador (ex: 1/12)
        if (txtContador != null)
        {
            txtContador.text = $"{indiceAtual + 1} / {listaDePersonagensLocal.Count}";
        }
    }
}