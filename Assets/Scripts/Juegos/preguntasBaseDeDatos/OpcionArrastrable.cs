using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpcionArrastrable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public bool esCorrecta = false;
    [HideInInspector] public PreguntasManager manager;

    private Vector2 posicionOriginal;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private bool posicionGuardada = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        StartCoroutine(GuardarPosicionAlFrame());
    }

    IEnumerator GuardarPosicionAlFrame()
    {
        // Esperar un frame para que el Canvas termine de calcular posiciones
        yield return null;
        posicionOriginal = rectTransform.anchoredPosition;
        posicionGuardada = true;
    }

    public void RestablecerPosicion()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (posicionGuardada)
            rectTransform.anchoredPosition = posicionOriginal;
    }

    public void ResetearEstado()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        RestablecerPosicion();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        RestablecerPosicion();
    }
}