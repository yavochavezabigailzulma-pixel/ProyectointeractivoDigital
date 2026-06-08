// ── SopaLetrasCell.cs ─────────────────────────────────────────
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SopaLetrasCell : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public int fila;
    public int columna;
    public TextMeshProUGUI letra;

    private SopaLetrasManager manager;

    public void Init(int f, int c, char l, SopaLetrasManager m)
    {
        fila = f;
        columna = c;
        letra.text = l.ToString();
        manager = m;
    }

    public void OnPointerDown(PointerEventData e)
    {
        manager.IniciarSeleccion(this);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        manager.ActualizarSeleccion(this);
    }
}