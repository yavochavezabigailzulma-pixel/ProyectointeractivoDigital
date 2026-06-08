using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecuadroOracion : MonoBehaviour, IDropHandler
{
    public OracionManager manager;

    public void OnDrop(PointerEventData eventData)
    {
        TarjetaOracion tarjeta = eventData.pointerDrag?.GetComponent<TarjetaOracion>();
        if (tarjeta == null) return;
        manager.VerificarRespuesta(tarjeta);
    }
}
