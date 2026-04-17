using UnityEngine;
using UnityEngine.EventSystems; // Necessário para interagir com a UI
using System.Collections.Generic;

public class ControllerCursor : MonoBehaviour
{
    public float velocidade = 500f;
    public RectTransform cursorVisual; // Arraste a imagem do cursor aqui (UI)
    public string botaoSelecao = "JoystickButton0"; // Botão A do Xbox

    private Vector2 posicaoVirtual;

    void Start()
    {
        // Começa o cursor no centro da tela
        posicaoVirtual = new Vector2(Screen.width / 2, Screen.height / 2);
        // Esconde o cursor real do mouse se quiser
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Captura o movimento do Analógico Esquerdo ou Setas
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. Atualiza a posição baseada na velocidade
        posicaoVirtual.x += horizontal * velocidade * Time.deltaTime;
        posicaoVirtual.y += vertical * velocidade * Time.deltaTime;

        // Limita o cursor para não sair da tela
        posicaoVirtual.x = Mathf.Clamp(posicaoVirtual.x, 0, Screen.width);
        posicaoVirtual.y = Mathf.Clamp(posicaoVirtual.y, 0, Screen.height);

        // 3. Move o visual do cursor na UI
        cursorVisual.position = posicaoVirtual;

        // 4. SIMULA O CLIQUE (O "Pulo do Gato")
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return))
        {
            SimularClique();
        }
    }

    void SimularClique()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = posicaoVirtual;

        List<RaycastResult> uiResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, uiResults);

        if (uiResults.Count > 0)
        {
            foreach (var result in uiResults)
            {
                // O PULO DO GATO: 
                // Procuramos na hierarquia (do objeto atingido para cima) quem tem um manipulador de clique.
                GameObject targetComEvento = ExecuteEvents.GetEventHandler<IPointerClickHandler>(result.gameObject);

                if (targetComEvento != null)
                {
                    // Disparamos o evento no objeto certo (o Botão, não o Texto)
                    ExecuteEvents.Execute(targetComEvento, pointerData, ExecuteEvents.pointerClickHandler);
                    return;
                }
            }
        }

        // Se não bateu na UI, checa as cartas
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(posicaoVirtual);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            hit.collider.gameObject.SendMessage("HandleCardPress", SendMessageOptions.DontRequireReceiver);
        }
    }
}