using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ── Tarjeta arrastrable ───────────────────────────────────────
public class TarjetaOracion : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string valorRespuesta;
    [HideInInspector] public OracionManager manager;

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
