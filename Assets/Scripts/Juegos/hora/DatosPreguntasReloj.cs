using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[CreateAssetMenu(fileName = "NuevaPreguntaReloj", menuName = "Preguntas/DatosPreguntas")]
public class DatosPreguntasReloj : ScriptableObject
{
    public string pregunta;
    public Sprite imagenReloj;  // imagen del reloj para esta pregunta
    public string opcion1;      // siempre la correcta
    public string opcion2;
    public string opcion3;
}