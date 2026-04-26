using UnityEngine;

public class SeleccionPlaneta : MonoBehaviour
{
    public string nombrePlaneta;

    void OnMouseDown()
    {
        UIManager.Instance.MostrarInfo(nombrePlaneta);
    }
}