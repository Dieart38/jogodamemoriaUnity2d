using UnityEngine;

public class MovimentoPatrulha : MonoBehaviour
{
    [Header("Pontos de Movimento")]
    public Transform pontoA;
    public Transform pontoB;

    [Header("Configurações")]
    public float velocidade = 3f;

    private Transform destinoAtual;

    void Start()
    {
        // Define o ponto B como o primeiro destino
        destinoAtual = pontoB;
        AjustarRotacao();
    }

    void Update()
    {
        // Move o objeto em direção ao destino atual
        transform.position = Vector3.MoveTowards(transform.position, destinoAtual.position, velocidade * Time.deltaTime);

        // Verifica se o objeto chegou bem perto do destino
        if (Vector3.Distance(transform.position, destinoAtual.position) < 0.1f)
        {
            // Alterna o destino e flipa a imagem
            if (destinoAtual == pontoB)
            {
                destinoAtual = pontoA;
            }
            else
            {
                destinoAtual = pontoB;
            }

            AjustarRotacao();
        }
    }

    void AjustarRotacao()
    {
        // Descobre a direção (se o destino está para a direita ou esquerda do objeto)
        float direcao = destinoAtual.position.x - transform.position.x;

        // Pega a escala atual do objeto
        Vector3 escalaLocal = transform.localScale;

        if (direcao > 0)
        {
            // Se o destino for para a direita, garante que o X da escala seja positivo
            escalaLocal.x = Mathf.Abs(escalaLocal.x);
        }
        else if (direcao < 0)
        {
            // Se o destino for para a esquerda, força o X da escala a ser negativo (flip)
            escalaLocal.x = -Mathf.Abs(escalaLocal.x);
        }

        // Aplica a nova escala ao objeto
        transform.localScale = escalaLocal;
    }
}