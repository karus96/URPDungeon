using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControladorDePersonajesDelJugador : MonoBehaviour
{
     //====================================================================
    /* Clase para controlar el grupo de personajes del jugador
     */
    //====================================================================
    public static ControladorDePersonajesDelJugador InstanciaActual;

    public Vector3 _objetivoMovimiento;

    public GameObject _personaje;

    public GameObject _npc;

    public bool _cambiarPersonaje;

    public List<Transform> posiciones;

    //====================================================================
    //Metodos Unity
    //====================================================================
    private void Awake()
    {
        if (InstanciaActual == null)
        {
            InstanciaActual = this;
        }
        else if (InstanciaActual != this)
        {
            Destroy(this.gameObject);
        }
    }
/*
    private void Update()
    {
    }

    //====================================================================
   public void CambiarEstadoDeLaCelda(Transform posicion, bool estado)
    {
        foreach (Transform hijos in transform)
        {
            if (hijos == posicion)
            {
                hijos.GetComponent<PuntoDeMovimiento>()._campoOcupado = estado;
            }
        }
    }

    public bool ObtenerEstadoDeLaCelda(Transform posicion)
    {
        bool estado = false;
        foreach (Transform hijos in transform)
        {
            if (hijos == posicion)
            {
                estado = hijos.GetComponent<PuntoDeMovimiento>()._campoOcupado;
            }
        }
        return estado;
    }
*/
    public void CambiarPosicionDelPersonaje(Transform objetivo)
    {
       
        if (_cambiarPersonaje)
        {
            _npc
                .GetComponent<PersonajeController>()
                .EstablecerObjetivoDeMovimiento(objetivo.position);
        }
        else
        {
            _personaje
                .GetComponent<PersonajeController>()
                .EstablecerObjetivoDeMovimiento(objetivo.position);
        }
    }

    public void CambiarPersonaje()
    {
        _cambiarPersonaje = !_cambiarPersonaje;
    }

    //====================================================================
}
//====================================================================