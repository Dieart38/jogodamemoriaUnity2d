using UnityEngine;
using System.Collections;

using System;
public class Card : MonoBehaviour
{
    [Header("Configuracao da Carta")]
    public int cardID;  // ID que define o par
    public Sprite fronSprite;  // Imagem da face da carta

    [Header("Referencias Internas")]
    public SpriteRenderer frontRenderer;
    public SpriteRenderer backRenderer;

    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isAnimating = false;


    private void Start()
    {
        frontRenderer.sprite = fronSprite;
        ShowBack();
    }

    private void OnMouseDown() // Nome corrigido (D maiúsculo)
    {
        // Verifica se pode virar a carta antes de executar
        if (GameManager.Instance.CanFlip() && !isFlipped && !isMatched && !isAnimating)
        {
            FlipToFront();
            GameManager.Instance.CardFlipped(this);
        }
    }

    public void FlipToFront()
    {
        isFlipped = true;
        StartCoroutine(FlipAnimation(true));
    }

    public void FlipToBack()
    {
        isFlipped = false;
        StartCoroutine(FlipAnimation(false));
    }

    private IEnumerator FlipAnimation(bool showFront)
    {
        isAnimating = true;
        float t = 0f;
        float duration = 0.15f;

        // Primeira metade: reduz escala X para 0
        while (t < duration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / duration); // Escala X vai de 1 a 0, Y e Z permanecem constantes
            transform.localScale = new Vector3(s, 1f, 1f);
            yield return null;
        }

        // Troca Visual no ponto de menor escala
        if (showFront) ShowFront();
        else ShowBack();

        // Segunda metade: aumenta a escala X de volta a 1
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, t / duration);
            transform.localScale = new Vector3(s, 1f, 1f); // Mantém a escala Y e Z constantes
            yield return null;

        }

        transform.localScale = Vector3.one; // Garante escala final correta
        isAnimating = false;

    }

    private void ShowFront()
    {
        frontRenderer.enabled = true;
        backRenderer.enabled = false;
    }

    private void ShowBack()
    {
        frontRenderer.enabled = false;
        backRenderer.enabled = true;
    }

    public void SetMatched()
    {
        isMatched = true;
        // Efeito visual de par encontrado
        StartCoroutine(MatchEffect());
    }

    private IEnumerator MatchEffect()
    {
        Color original = frontRenderer.color;
        frontRenderer.color = Color.green;
        yield return new WaitForSeconds(0.3f);
        frontRenderer.color = original;
    }
}
