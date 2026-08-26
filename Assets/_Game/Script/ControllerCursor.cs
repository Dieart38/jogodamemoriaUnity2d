using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

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

        // 1. SEPARAMOS QUEM ESTÁ CLICANDO (Mouse ou Controle)
        bool cliqueDeMouse = Input.GetMouseButtonDown(0);
        bool cliqueDeControle = Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return);
        bool botaoPressionado = cliqueDeMouse || cliqueDeControle;

        bool botaoSegurado = Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.Return) || Input.GetMouseButton(0);
        bool botaoSolto = Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonUp(0);

        // 2. MATA O FOCO NATIVO (Mas poupa o InputField para permitir digitação!)
        GameObject objetoSelecionado = EventSystem.current.currentSelectedGameObject;
        if (objetoSelecionado != null)
        {
            // Checa se o objeto selecionado é um campo de texto
            bool ehUmCampoDeTexto = objetoSelecionado.GetComponent<InputField>() != null || objetoSelecionado.GetComponent<TMPro.TMP_InputField>() != null;

            // Se NÃO for um campo de texto, limpa o foco para evitar o bug de duplo-clique.
            // Se for um campo de texto, não faz nada (deixa o cara digitar).
            if (!ehUmCampoDeTexto)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

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

                // 3. BLINDAGEM DO MOUSE 
                // Se foi o Mouse, o EventSystem nativo da Unity JÁ clicou sozinho. 
                // Só injetamos o nosso clique virtual se o jogador usar Teclado/Controle!
                if (cliqueDeControle)
                {
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
                }

                return; // Bloqueia o clique de vazar para trás (não atinge as cartas)
            }

            // Se não bateu na UI e o jogo não tá pausado, interage com as cartas do jogo
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

        // --- ESTADO 2: O JOGADOR ESTÁ SEGURANDO E MOVENDO (Apenas Controle) ---
        if (botaoSegurado && objetoSendoArrastado != null)
        {
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