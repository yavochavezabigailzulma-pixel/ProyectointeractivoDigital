using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// ?? Script para cada opción arrastrable ??????????????????????
public class OpcionArrastrable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public bool esCorrecta = false;
    [HideInInspector] public PreguntasManager manager;

    private Vector2 posicionOriginal;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        posicionOriginal = rectTransform.anchoredPosition;
    }

    public void RestablecerPosicion()
    {
        rectTransform.anchoredPosition = posicionOriginal;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // permite detectar el recuadro debajo
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Debug.Log($"{gameObject.name} - esCorrecta: {esCorrecta}");
        RestablecerPosicion();
    }

}



