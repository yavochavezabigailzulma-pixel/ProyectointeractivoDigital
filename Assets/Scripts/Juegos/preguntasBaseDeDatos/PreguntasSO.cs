using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPregunta", menuName = "Preguntas/DatosPreguntas")]
public class DatosPreguntas : ScriptableObject
{
    public string pregunta;
    public string opcion1; // ← SIEMPRE la correcta
    public string opcion2;
    public string opcion3;
}