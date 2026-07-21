using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPreguntaReloj", menuName = "Preguntas/DatosPreguntasReloj")]
public class DatosPreguntasReloj : ScriptableObject
{
    public string pregunta;
    public string horaEnNumeros; // ej: "12:15"
    [Header("Configuracion del reloj")]
    public float rotacionAgujaHoras;   // grados en Z para la aguja de horas
    public float rotacionAgujaMinutos; // grados en Z para la aguja de minutos

    [Header("Opciones")]
    public string opcion1; // siempre la correcta
    public string opcion2;
    public string opcion3;
}