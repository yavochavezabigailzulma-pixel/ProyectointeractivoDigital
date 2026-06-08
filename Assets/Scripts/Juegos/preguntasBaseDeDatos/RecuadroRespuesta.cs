using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ?? Script para el recuadro receptor ?????????????????????????
public class RecuadroRespuesta : MonoBehaviour, IDropHandler
{
    public PreguntasManager manager;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop detectado en recuadro");
        OpcionArrastrable opcion = eventData.pointerDrag.GetComponent<OpcionArrastrable>();
        Debug.Log("Opcion encontrada: " + (opcion != null));
        if (opcion == null) return;
        Debug.Log("Es correcta: " + opcion.esCorrecta);

        if (opcion == null) return;

        if (opcion.esCorrecta)
        {
            manager.RespuestaCorrecta(opcion);
        }
        else
        {
            manager.RespuestaIncorrecta(opcion);
        }
    }
}
