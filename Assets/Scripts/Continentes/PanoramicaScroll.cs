using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanoramicaScroll : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Header("Configuración")]
    public RectTransform imagenPanoramica;
    //public Image imagenUI; // ← AGREGA ESTO (componente Image del objeto)
    public float sensibilidad = 1f;
    public bool invertir = false;

    private float limiteIzquierda;
    private float limiteDerecha;
    private RectTransform canvasRect;

    void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        CalcularLimites();
    }

    void CalcularLimites()
    {
        float mitadImagen = imagenPanoramica.rect.width * imagenPanoramica.localScale.x / 2f;
        float mitadCanvas = canvasRect.rect.width / 2f;

        limiteDerecha = mitadImagen - mitadCanvas;
        limiteIzquierda = -(mitadImagen - mitadCanvas);
    }
    public void CambiarImagen(RectTransform nuevaImagen)
    {
        if (nuevaImagen == null) return;

        imagenPanoramica = nuevaImagen;

        CalcularLimites();

        imagenPanoramica.anchoredPosition = Vector2.zero;
    }
    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        float direccion = invertir ? -1f : 1f;
        float nuevaX = imagenPanoramica.anchoredPosition.x + eventData.delta.x * sensibilidad * direccion;
        nuevaX = Mathf.Clamp(nuevaX, limiteIzquierda, limiteDerecha);
        imagenPanoramica.anchoredPosition = new Vector2(nuevaX, imagenPanoramica.anchoredPosition.y);
    }
}