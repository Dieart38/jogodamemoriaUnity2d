using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ControllerCursor : MonoBehaviour
{
    public float velocidade = 500f;
    public RectTransform cursorVisual; 
    
    private Vector2 posicaoVirtual;
    private Vector2 ultimaPosicaoMouse; // Guarda a posição do mouse no frame anterior

    void Awake()
    {
        if (cursorVisual == null)
        {
            Debug.LogError("ControllerCursor: O cursorVisual não foi atribuído no Inspector.");
        }
        
    }

    void Start()
    {
        posicaoVirtual = new Vector2(Screen.width / 2, Screen.height / 2);
        ultimaPosicaoMouse = Input.mousePosition;
        Cursor.visible = false; // Esconde o cursor real do sistema
    }

    void Update()
    {
        // 1. DETECTAR MOVIMENTO DO MOUSE
        Vector2 mouseAtual = Input.mousePosition;
        
        // Se a posição atual do mouse for diferente da última, o usuário mexeu o mouse
        if (mouseAtual != ultimaPosicaoMouse)
        {
            posicaoVirtual = mouseAtual;
            ultimaPosicaoMouse = mouseAtual;
        }

        // 2. CAPTURAR MOVIMENTO DO CONTROLE (Analógico/Setas)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // O controle move o cursor a partir da posição atual (seja ela vinda do mouse ou do stick)
        posicaoVirtual.x += horizontal * velocidade * Time.deltaTime;
        posicaoVirtual.y += vertical * velocidade * Time.deltaTime;

        // 3. LIMITAR E ATUALIZAR VISUAL
        posicaoVirtual.x = Mathf.Clamp(posicaoVirtual.x, 0, Screen.width);
        posicaoVirtual.y = Mathf.Clamp(posicaoVirtual.y, 0, Screen.height);

        cursorVisual.position = posicaoVirtual;

        // 4. CLIQUE (Aceita Botão A do Xbox, Enter ou Clique Esquerdo do Mouse)
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
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
                GameObject targetComEvento = ExecuteEvents.GetEventHandler<IPointerClickHandler>(result.gameObject);
                if (targetComEvento != null)
                {
                    ExecuteEvents.Execute(targetComEvento, pointerData, ExecuteEvents.pointerClickHandler);
                    return; 
                }
            }
        }

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(posicaoVirtual);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            hit.collider.gameObject.SendMessage("HandleCardPress", SendMessageOptions.DontRequireReceiver);
        }
    }
}