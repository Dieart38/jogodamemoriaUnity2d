using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ControllerCursor : MonoBehaviour
{
    public float velocidade = 500f;
    public RectTransform cursorVisual; 
    
    private Vector2 posicaoVirtual;
    private Vector2 ultimaPosicaoMouse;

    // NOVO: Referência para guardar o slider que estamos "agarrando"
    private GameObject objetoSendoArrastado = null;

    void Awake()
    {
        if (cursorVisual == null) Debug.LogError("ControllerCursor: O cursorVisual não foi atribuído.");
    }

    void Start()
    {
        velocidade = PlayerPrefs.GetFloat("CursorSpeed", 500f);
        posicaoVirtual = new Vector2(Screen.width / 2, Screen.height / 2);
        ultimaPosicaoMouse = Input.mousePosition;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. MOVIMENTO (Mouse e Controle)
        Vector2 mouseAtual = Input.mousePosition;
        if (mouseAtual != ultimaPosicaoMouse)
        {
            posicaoVirtual = mouseAtual;
            ultimaPosicaoMouse = mouseAtual;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        posicaoVirtual.x += horizontal * velocidade * Time.unscaledDeltaTime;
        posicaoVirtual.y += vertical * velocidade * Time.unscaledDeltaTime;

        posicaoVirtual.x = Mathf.Clamp(posicaoVirtual.x, 0, Screen.width);
        posicaoVirtual.y = Mathf.Clamp(posicaoVirtual.y, 0, Screen.height);

        cursorVisual.position = posicaoVirtual;

        // 2. GERENCIAR CLIQUES E ARRASTOS
        GerenciarInteracao();
    }

    void GerenciarInteracao()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = posicaoVirtual;

        // Lemos os 3 estados possíveis do botão/mouse
        bool botaoPressionado = Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0);
        bool botaoSegurado = Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.Return) || Input.GetMouseButton(0);
        bool botaoSolto = Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonUp(0);

        // --- ESTADO 1: O JOGADOR APERTOU O BOTÃO AGORA ---
        if (botaoPressionado)
        {
            cursorVisual.localScale = Vector3.one * 0.8f; // Efeito visual de clique

            List<RaycastResult> uiResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, uiResults);

            // Bateu em UI?
            if (uiResults.Count > 0)
            {
                GameObject uiElement = uiResults[0].gameObject;

                // Tenta Clicar (Botões normais)
                GameObject clickTarget = ExecuteEvents.GetEventHandler<IPointerClickHandler>(uiElement);
                if (clickTarget != null) ExecuteEvents.Execute(clickTarget, pointerData, ExecuteEvents.pointerClickHandler);

                // Tenta Agarrar (Sliders e Barras de Rolagem)
                GameObject dragTarget = ExecuteEvents.GetEventHandler<IDragHandler>(uiElement);
                if (dragTarget != null)
                {
                    objetoSendoArrastado = dragTarget; // Salva o slider que pegamos
                    ExecuteEvents.Execute(objetoSendoArrastado, pointerData, ExecuteEvents.pointerDownHandler);
                    ExecuteEvents.Execute(objetoSendoArrastado, pointerData, ExecuteEvents.beginDragHandler);
                }

                return; // Bloqueia para não atingir as cartas!
            }

            // Se não bateu na UI e o jogo não tá pausado, interage com as cartas
            if (Time.timeScale != 0f)
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(posicaoVirtual);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if (hit.collider != null)
                {
                    Card cartaClicada = hit.collider.GetComponent<Card>();
                    if (cartaClicada != null) cartaClicada.HandleCardPress();
                }
            }
        }

        // --- ESTADO 2: O JOGADOR ESTÁ SEGURANDO E MOVENDO ---
        if (botaoSegurado && objetoSendoArrastado != null)
        {
            // Fica mandando a nova posição para o slider enquanto o botão estiver apertado
            ExecuteEvents.Execute(objetoSendoArrastado, pointerData, ExecuteEvents.dragHandler);
        }

        // --- ESTADO 3: O JOGADOR SOLTOU O BOTÃO ---
        if (botaoSolto)
        {
            cursorVisual.localScale = Vector3.one; // Restaura tamanho do cursor

            if (objetoSendoArrastado != null)
            {
                ExecuteEvents.Execute(objetoSendoArrastado, pointerData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(objetoSendoArrastado, pointerData, ExecuteEvents.endDragHandler);
                objetoSendoArrastado = null; // Limpa a referência
            }
        }
    }
}