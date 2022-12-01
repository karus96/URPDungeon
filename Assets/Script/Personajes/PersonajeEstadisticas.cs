using UnityEngine;

public class PersonajeEstadisticas : MonoBehaviour
{
    //====================================================================
    /* Clase base para tener las estadisticas de los personajes 
     * ademas de los metodos relacionados con la vida y la armadura
     */
    //====================================================================
    public string _nombre;
    public float _puntosDeGolpeMaximo;
    public float _puntosDeGolpeActuales;
    public float _velocidad;
    //====================================================================
    //Para el combate
    //====================================================================
    public float _fuerza; //para determinar el daño    
    public float _defensa; //La sumatoria de los objetos de armadura
    public bool _estaConVida;
    //====================================================================
    private static float TOPE_ARMADURA = 330;
    //====================================================================
    //Metodos
    //====================================================================
    private void ReducirVida(float cantidad)
    {
        _puntosDeGolpeActuales -= cantidad;
        if (_puntosDeGolpeActuales < 0)
        {
            _estaConVida = false;
            _puntosDeGolpeActuales = 0;
        }
    }
    public void AumentarVida(float cantidad)
    {
        _puntosDeGolpeActuales += cantidad;
        if (_puntosDeGolpeActuales > _puntosDeGolpeMaximo)
        {
            Debug.Log("La vida llego al maximo");
            _puntosDeGolpeActuales = _puntosDeGolpeMaximo;
        }
    }
    public void RecibirDaño(float cantidad)
    {
        float cantidadReducida = cantidad * PorcentajeDeArmadura();
        float dañoTotalRecivido = cantidad - cantidadReducida;
        //Debug.Log("Cantidad reducida:" + cantidadReducida);
        //Debug.Log("Daño recivido: " + dañoTotalRecivido);
        ReducirVida(dañoTotalRecivido);
    }
    private float PorcentajeDeArmadura()
    {
        float porcentaje = _defensa / TOPE_ARMADURA;
        //Debug.Log("Porcentaje de reduccion de armadura: " +  porcentaje);
        //Debug.Log("Defensa: " + _defensa);
        return porcentaje;
    }
}