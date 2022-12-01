using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuntoDeMovimiento : MonoBehaviour
{
    //====================================================================
    /* Clase para los movimientos de los actores del jugador
     */
    //====================================================================  
    //Variables
    //====================================================================
    //Muestra cual es la celda elegida
    //====================================================================
    private GameObject _puntero;
    public bool _campoOcupado;
    //====================================================================
    //Metodos
    //====================================================================
    private void Start() {
        _puntero= GameManager.InstanciaActual._puntero;
    }
    private void OnMouseDown()
    {        
        if (!DetectarSiEstaDetrasDelCanvas())
        {
            ControladorDePersonajesDelJugador.InstanciaActual.CambiarPosicionDelPersonaje(transform);
            _puntero.transform.position = new Vector3(transform.position.x, transform.position.y+1.5f, transform.position.z);
        }        
    }     
    private void OnTriggerEnter(Collider collision)
    {      
        if (collision.gameObject.tag == "Personaje")
        {
            _campoOcupado = true;
            
        }
    }
    //====================================================================
    private bool DetectarSiEstaDetrasDelCanvas(){
        PointerEventData currentPointer = new PointerEventData(EventSystem.current);
        currentPointer.position=new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(currentPointer,results);
        return results.Count>0;
        }
}
//====================================================================