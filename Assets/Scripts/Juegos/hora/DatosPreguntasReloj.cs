using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPreguntaReloj", menuName = "Preguntas/DatosPreguntasReloj")]
public class DatosPreguntasReloj : ScriptableObject
{
    public string pregunta;
    public string horaEnNumeros; // ej: "12:15" (display debajo del reloj, solo nivel 1)

    [Header("Configuracion del reloj")]
    public float rotacionAgujaHoras;
    public float rotacionAgujaMinutos;

    [Header("Opciones - Texto (Nivel 1)")]
    public string opcion1; // siempre la correcta
    public string opcion2;
    public string opcion3;

    [Header("Opciones - Formato numérico (Nivel 2)")]
    [Tooltip("Debe corresponder exactamente a opcion1 (ej: opcion1 = 'Dos en punto' → opcion1Numerica = '2:00')")]
    public string opcion1Numerica;
    public string opcion2Numerica;
    public string opcion3Numerica;
}