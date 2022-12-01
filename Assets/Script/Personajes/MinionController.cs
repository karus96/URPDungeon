using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MinionController : PersonajeController
{
    //====================================================================
    /* Clase para controlar los NPCS
     */
    //====================================================================    
    //Animiaciones disponibles
    //====================================================================
    private const string 
                         ANIM_ATACAR = "atacar",
                         ANIM_RECIBE_GOLPE = "recibirGolpe",
                         ANIM_CORRER = "corre";


    //====================================================================
    //Variables para el movimiento
    //====================================================================
    [SerializeField] private bool _movimientoPorRuta;
    [SerializeField] private List<Transform> _posicionesDeMovimiento;
    [SerializeField] private int _rangoDeDeteccion;
    private int _posicionDelMovimientoEnLaRuta;
    private Transform _posicionEnemigo;
    //====================================================================
    //Auxiliares 
    //====================================================================
    private float contador;
    private bool _buscarNuevasPosiciones;
    private bool _correr;
    //====================================================================
    [SerializeField] private bool _estaMuriendo;
    private bool _estadoAnimacionMuerte;
    private bool _recibirAtaque;
    public TiposDePersonajes _tipoPersonaje;
    public bool debug = false;    
    //====================================================================
    //Metodos
    //====================================================================
    //Metodos de unity
    //====================================================================
    void Start()
    {
        _buscarNuevasPosiciones = true;
        _posicionDelMovimientoEnLaRuta = 0;
        _correr = false;
        _estadoAnimacionMuerte = false;        
        IniciarVariables();
    }
    void Update()
    {
        //====================================================================
        //Controla que se detenga cuando muere
        //====================================================================
        if (_estaMuriendo)
        {
            _animator.SetBool(ANIM_MUERTE, _estadoAnimacionMuerte);
            if (_estadoAnimacionMuerte) { _estadoAnimacionMuerte = false; }
            return;
            //====================================================================
        }
        ActualizarAnimacionDeDaño();
        BusquedadDeObjetosEnRango();
        Atacar();
        RealizarAtaque();


        //Para realizar pruebas
        if (debug)
        {
            debug = false;
            RecivirAtaque(10);
        }
    }
    void FixedUpdate()
    {
        if (!_estaMuriendo)
        {            
            RealizarMovimiento();
        }
    }
    //====================================================================
    //Daño
    //====================================================================
    private void ActualizarAnimacionDeDaño()
    {
        if (_tipoPersonaje == TiposDePersonajes.Minion)
        {
           _animator.SetBool(ANIM_RECIBE_GOLPE, _recibirAtaque);
        }
        if (_recibirAtaque) { _recibirAtaque = false; }
    }
    public void RecivirAtaque(float cantidad)
    {
        RecibirDaño(cantidad);
        if (_estaConVida) { _recibirAtaque = true; }
        else
        {
            StartCoroutine(Destruir());
            _estaMuriendo = true;
            _estadoAnimacionMuerte = true;
        }
    }
    IEnumerator Destruir()
    {
        yield return new WaitForSecondsRealtime(3);
        Destroy(this.gameObject);
    }
    //====================================================================
    //Movimiento
    //====================================================================
    private void RealizarMovimiento()
    {
        _animator.SetBool(ANIM_MOVERSE, !EstaEnElObjetivo());
        if (_posicionEnemigo != null)
        {
            MoverseHaciaElEnemigo();
        }
        else if (_movimientoPorRuta)
        {         
            MovimientoPorRuta();
        }
        else
        {

            MovimientoAleatorio();
        }
        Moverse();
    }
    //====================================================================
    private void MoverseHaciaElEnemigo()
    {
        //Correr
        if (!_correr)
        {
            _agente.speed = _agente.speed * 1.5f;
            _correr = true;
            _animator.SetBool(ANIM_CORRER, _correr);
        }

        EstablecerObjetivoDeMovimiento(_posicionEnemigo.position);
    }
    private void MovimientoPorRuta()
    {
        if (EstaEnElObjetivo())
        {               
            EstablecerObjetivoDeMovimiento(_posicionesDeMovimiento[_posicionDelMovimientoEnLaRuta].position);
            _posicionDelMovimientoEnLaRuta++;
            if (_posicionDelMovimientoEnLaRuta >= _posicionesDeMovimiento.Count)
            {
                _posicionDelMovimientoEnLaRuta = 0;
            }
        }
    }
    private void MovimientoAleatorio()
    {
        if (EstaEnElObjetivo() && _buscarNuevasPosiciones)
        {
            _buscarNuevasPosiciones = false;
            StartCoroutine(IniciarMovimiento());
        }
        //====================================================================
        //Evita que se queden pegados
        //====================================================================
        if (!EstaEnElObjetivo())
        {
            contador += Time.deltaTime;
        }
        else
        {
            contador = 0;
        }
        if (contador > 5)
        {
            ReiniciarMovimiento();
            contador = 0;
        }
    }
    //====================================================================
    //Metodos de ataque
    //====================================================================
    private void Atacar()
    {
        if (_posicionEnemigo != null)
        {               
            if (_agente.remainingDistance < 1.1f)
            {
               
                _animator.SetBool(ANIM_ATACAR, true);

            }
            else
            {
                _animator.SetBool(ANIM_ATACAR, false);
            }
        }
    }
    public void RealizarAtaque(){
       float parametroAnimacionAtaque =_animator.GetFloat("ataque");
        if (parametroAnimacionAtaque > 1)
        {          
            Debug.Log("Realizar el ataque: " + _fuerza);
            _posicionEnemigo.GetComponent<PersonajeController>().RecibirDaño(_fuerza);
        }
    }
    
    //====================================================================
    //Metodos auxiliares
    //====================================================================
    private void ReiniciarMovimiento()
    {
        Debug.LogWarning("Se ha forzado un reinicio de movimiento aleatorio" + gameObject.name);
        BusquedadDeObjetosEnRango();
        _buscarNuevasPosiciones = false;
        StartCoroutine(IniciarMovimiento());
    }
    IEnumerator IniciarMovimiento()
    {
        yield return new WaitForSecondsRealtime(3);

        int x = Random.Range(0, _posicionesDeMovimiento.Count);
        EstablecerObjetivoDeMovimiento(_posicionesDeMovimiento[x].position);
        _buscarNuevasPosiciones = true;
    }
    private void BusquedadDeObjetosEnRango()
    {
        //Hacemos una esfera para buscar los objetos que nos interesan
        Collider[] objetosEnRango = Physics.OverlapSphere(transform.position, _rangoDeDeteccion);
        if (objetosEnRango.Length > 0)
        {
            if (!_movimientoPorRuta && EstaEnElObjetivo() && _buscarNuevasPosiciones)
            {
                _posicionesDeMovimiento.Clear();
                _posicionEnemigo = null;
            }
            foreach (Collider objeto in objetosEnRango)
            {                
                BucarEnemigos(objeto.gameObject);
                if (!_movimientoPorRuta && EstaEnElObjetivo() && _buscarNuevasPosiciones) { BuscarPosicionesDeMovimiento(objeto.gameObject); }               
            }
        }
    }
    private void BucarEnemigos(GameObject objeto)
    {
        if (objeto.tag == "Player")
        {
            _posicionEnemigo = objeto.transform;
        }
    }
    private void BuscarPosicionesDeMovimiento(GameObject objeto)
    {
        //====================================================================
        //Busqueda de pisos para movimiento aleatorio
        //====================================================================
        if (objeto.tag == "Piso")
        {
            bool estaEnLaLista = false;
            foreach (Transform posicion in _posicionesDeMovimiento)
            {
                if ((int)posicion.position.x == (int)objeto.transform.position.x &&
                    (int)posicion.position.y == (int)objeto.transform.position.y &&
                    (int)posicion.position.z == (int)objeto.transform.position.z)
                {                    
                    estaEnLaLista = true;
                    return;
                }
            }
            if (!estaEnLaLista)
            {
                _posicionesDeMovimiento.Add(objeto.transform);
            }
        }
        //====================================================================
    }
    //====================================================================
    //Debug
    //====================================================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _rangoDeDeteccion);
    }

}

//====================================================================