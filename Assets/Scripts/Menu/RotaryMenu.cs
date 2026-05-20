using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;
[ExecuteInEditMode]
public class RotaryMenu : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Sonidos")]
    public EventReference clicMatraca;

    [Header("Geometría de la rueda")]
    public float radius = 300f;
    [Range(60f, 360f)]
    public float visibleArcDegrees = 220f;
    public float arcRotationOffset = 270f;

    [Header("Comportamiento")]
    public float snapSpeed = 8f;
    public float dragSensitivity = 0.3f;
    [Range(0f, 0.99f)]
    public float inertia = 0.92f;

    [Header("Escalado de ítems")]
    public float centerScale = 1.3f;
    public float edgeScale = 0.7f;
    public float scaleSpeed = 10f;

    [Header("Opacidad")]
    public float centerAlpha = 1f;
    public float edgeAlpha = 0.4f;

    [Header("Centro de la ruleta")]
    public Vector2 centerOffset = Vector2.zero;

    [Header("Tamaño de ítems")]
    public Vector2 itemSize = new Vector2(100f, 100f);

    [Header("Avatar")]
    public Image avatarImage;
    public Sprite[] avatarSprites;
    public Sprite[] avatarBackgroundSprites;
    public Image avatarBackgroundImage;

    [Header("Fondo")]
    [Tooltip("Un sprite por cada ítem, en el mismo orden que los botones hijos")]
    public Sprite[] backgroundSprites;
    [Tooltip("La Image del objeto 'Fondo' en el Canvas")]
    public Image backgroundImage;

    public System.Action<int> onCenterChanged;

    private RectTransform[] items;
    private CanvasGroup[] groups;
    private int itemCount;
    private float anglePerItem;
    private float currentAngle;
    private float targetAngle;
    private float dragVelocity;
    private bool isDragging;
    private int centeredIndex;
    private int lastCenteredIndex = 0;
    private Vector2 lastDragPos;

    void Start()
    {
        var rts = new List<RectTransform>();
        var cgs = new List<CanvasGroup>();

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;
            var rt = child.GetComponent<RectTransform>();
            if (rt == null) continue;
            var cg = child.GetComponent<CanvasGroup>();
            if (cg == null) cg = child.gameObject.AddComponent<CanvasGroup>();
            rts.Add(rt);
            cgs.Add(cg);

        }

        items = rts.ToArray();
        groups = cgs.ToArray();
        itemCount = items.Length;
        if (itemCount == 0) { Debug.LogWarning("RotaryMenu: no hay ítems hijos activos."); return; }

        anglePerItem = 360f / itemCount;
        lastCenteredIndex = Mathf.RoundToInt(currentAngle / anglePerItem) % itemCount;
        lastCenteredIndex = ((lastCenteredIndex % itemCount) + itemCount) % itemCount;
        currentAngle = 0f;
        targetAngle = 0f;

        PositionAllItems(currentAngle);
        UpdateScalesAndAlpha(currentAngle);

        foreach (var rt in items)
            rt.sizeDelta = itemSize;

        ApplyBackground(0);
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        if (itemCount == 0) return;

        if (!isDragging)
        {
            if (Mathf.Abs(dragVelocity) > 0.1f)
            {
                currentAngle += dragVelocity * Time.deltaTime;
                dragVelocity = Mathf.Lerp(dragVelocity, 0f, Time.deltaTime * (1f - inertia) * 10f);
            }
            else
            {
                dragVelocity = 0f;
            }

            float nearest = Mathf.Round(currentAngle / anglePerItem) * anglePerItem;
            currentAngle = Mathf.LerpAngle(currentAngle, nearest, Time.deltaTime * snapSpeed);


            //// Avatar
            //if (avatarImage != null)
            //    avatarImage.color = items[centeredIndex].GetComponent<Image>().color;

            //// Fondo — solo actualiza cuando realmente cambió el ítem central
            
        }

        int idx = Mathf.RoundToInt(currentAngle / anglePerItem) % itemCount;
        centeredIndex = ((idx % itemCount) + itemCount) % itemCount;
        if (centeredIndex != lastCenteredIndex)
        {
            lastCenteredIndex = centeredIndex;
            ApplyBackground(centeredIndex);
            onCenterChanged?.Invoke(centeredIndex);
            AudioManager.Instance.Play(clicMatraca);
        }

        PositionAllItems(currentAngle);
        UpdateScalesAndAlpha(currentAngle);        
    }
    void ApplyBackground(int index)
    {
        if (backgroundImage != null && backgroundSprites != null && backgroundSprites.Length > 0)
        {
            // Usa módulo por si backgroundSprites tiene menos entradas que items
            int spriteIdx = index % backgroundSprites.Length;
            if (backgroundSprites[spriteIdx] != null)
                backgroundImage.sprite = backgroundSprites[spriteIdx];
        }

        // Avatar sprite
        if (avatarImage != null && avatarSprites != null && avatarSprites.Length > 0)
        {
            int idx = index % avatarSprites.Length;
            if (avatarSprites[idx] != null)
                avatarImage.sprite = avatarSprites[idx];
        }

        // Avatar fondo
        if (avatarBackgroundImage != null && avatarBackgroundSprites != null && avatarBackgroundSprites.Length > 0)
        {
            int idx = index % avatarBackgroundSprites.Length;
            if (avatarBackgroundSprites[idx] != null)
                avatarBackgroundImage.sprite = avatarBackgroundSprites[idx];
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragVelocity = 0f;
        lastDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - lastDragPos.x;
        lastDragPos = eventData.position;
        dragVelocity = delta * dragSensitivity * 60f;
        currentAngle += delta * dragSensitivity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    void PositionAllItems(float wheelAngle)
    {
        for (int i = 0; i < itemCount; i++)
        {
            float itemAngle = i * anglePerItem - wheelAngle + arcRotationOffset;
            float rad = itemAngle * Mathf.Deg2Rad;
            //items[i].anchoredPosition = new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            items[i].anchoredPosition = new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius) + centerOffset;

            items[i].localRotation = Quaternion.Euler(0f, 0f, itemAngle - 90f);
        }
    }

    void UpdateScalesAndAlpha(float wheelAngle)
    {
        float halfArc = visibleArcDegrees * 0.5f;

        for (int i = 0; i < itemCount; i++)
        {
            float itemAngle = NormalizeAngle(i * anglePerItem - wheelAngle);
            float t = Mathf.Clamp01(Mathf.Abs(itemAngle) / halfArc);

            float cur = items[i].localScale.x;
            items[i].localScale = Vector3.one * Mathf.Lerp(cur, Mathf.Lerp(centerScale, edgeScale, t), Time.deltaTime * scaleSpeed);
            groups[i].alpha = Mathf.Lerp(centerAlpha, edgeAlpha, t);
        }
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    public void GoToItem(int index)
    {
        if (itemCount == 0) return;
        index = ((index % itemCount) + itemCount) % itemCount;
        currentAngle = index * anglePerItem;
        dragVelocity = 0f;
    }

    public int GetCenteredIndex() => centeredIndex;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var rt = GetComponent<RectTransform>();
        if (rt == null) return;
        Vector3 origin = transform.position;
        Gizmos.color = new Color(0.4f, 0.6f, 1f, 0.5f);
        for (int s = 0; s < 64; s++)
        {
            float a1 = arcRotationOffset + (s / 64f) * visibleArcDegrees;
            float a2 = arcRotationOffset + ((s + 1) / 64f) * visibleArcDegrees;
            Gizmos.DrawLine(
                origin + new Vector3(Mathf.Cos(a1 * Mathf.Deg2Rad), Mathf.Sin(a1 * Mathf.Deg2Rad)) * radius,
                origin + new Vector3(Mathf.Cos(a2 * Mathf.Deg2Rad), Mathf.Sin(a2 * Mathf.Deg2Rad)) * radius
            );
        }
    }
#endif
}