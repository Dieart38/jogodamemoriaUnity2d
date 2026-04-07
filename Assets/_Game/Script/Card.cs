using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("Configuracao da Carta")]
    public int cardID;
    public Sprite fronSprite;

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

    // ─── INPUT UNIFICADO ──────────────────────────────────────────────────────

    private void Update()
    {
        // PC / Editor: mouse
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
                HandleCardPress();
        }

        // Android: toque
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                    HandleCardPress();
            }
        }
    }

    private void HandleCardPress()
    {
        Debug.Log($"Carta clicada: {cardID}"); // remove depois

        if (GameManager.Instance.CanFlip() && !isFlipped && !isMatched && !isAnimating)
        {
            FlipToFront();
            GameManager.Instance.CardFlipped(this);
        }
    }

    // ─── FLIP ─────────────────────────────────────────────────────────────────

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
        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / duration);
            transform.localScale = new Vector3(s, 1f, 1f);
            yield return null;
        }

        if (showFront) ShowFront();
        else ShowBack();

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, t / duration);
            transform.localScale = new Vector3(s, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
        isAnimating = false;
    }

    // ─── VISUAIS ──────────────────────────────────────────────────────────────

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