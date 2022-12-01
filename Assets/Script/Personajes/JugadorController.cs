using UnityEngine;

public class JugadorController : PersonajeController
{
    //====================================================================
    /* Clase para controlar los personajes del jugador
     */
    //====================================================================
    //====================================================================
    //Animiaciones disponibles
    //====================================================================
    private const string
        ANIM_CORRER = "correr",
        ANIM_ATAQUE_MELE = "ataqueMele",
        ANIM_ATAQUE_ARCO = "ataqueArco",
        ANIM_ATAQUE_VARITA = "ataqueVarita",
        ANIM_INTERACTUAR = "interactuar";

    //====================================================================
    
    public bool _estadoAnimacionMuerte; //Animacion

    public bool _estaMuriendo; //GameOver

    public bool _correr;

    public bool ataqueMele;

    public bool ataqueArco;

    public bool ataqueVarita;

    //se asegura de que las animaciones de combate se vean bien
    private float retrasoAtaque;

    private float tiempoDeRetraso = 1f;

    //====================================================================
    //Atacar
    //====================================================================
    private void AtaqueArco()
    {
        //Cualquiera puede usar el arco o ballesta
        if (retrasoAtaque > tiempoDeRetraso)
        {
            if (Input.GetMouseButtonDown((0)))
            {
                ataqueArco = true;
                retrasoAtaque = 0;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ataqueArco = false;
        }
        _animator.SetBool (ANIM_ATAQUE_ARCO, ataqueArco);
        if (retrasoAtaque == 0)
        {
            ataqueArco = false;
        }
        retrasoAtaque += Time.deltaTime;
    }

    private void AtaqueVarita()
    {
        //Solo el mago puede usar la varita
        if (retrasoAtaque > tiempoDeRetraso)
        {
            if (Input.GetMouseButtonDown((0)))
            {
                ataqueVarita = true;
                retrasoAtaque = 0;
                Debug.Log("Mouse!");
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ataqueVarita = false;
        }
        _animator.SetBool (ANIM_ATAQUE_VARITA, ataqueVarita);
        if (retrasoAtaque == 0)
        {
            ataqueVarita = false;
        }
        retrasoAtaque += Time.deltaTime;
    }

    private void AtaqueMele()
    {
        if (retrasoAtaque > tiempoDeRetraso)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ataqueMele = true;
                retrasoAtaque = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            ataqueMele = false;
        }
        _animator.SetBool (ANIM_ATAQUE_MELE, ataqueMele);
        if (retrasoAtaque == 0)
        {
            ataqueMele = false;
        }
        retrasoAtaque += Time.deltaTime;
    }

    //====================================================================
    //Metodos Unity
    //====================================================================
    void Start()
    {
        retrasoAtaque = 0;        
        IniciarVariables();
    }

    void Update()
    {
        //====================================================================
        //Controla que se detenga cuando muere
        //====================================================================
        if (_estaMuriendo)
        {
            _animator.SetBool (ANIM_MUERTE, _estadoAnimacionMuerte);
            if (_estadoAnimacionMuerte)
            {
                _estadoAnimacionMuerte = false;
            }
            return;
            //====================================================================
        }

        //====================================================================
        //Atacar
        //====================================================================
        //Ataque mele
        AtaqueMele();

        //Ataque a distancia
        /*
        if (mTipoDePersonaje == TiposDePersonajes.Mago)
        {
            AtaqueVarita();
        }
        else
        {
            AtaqueArco();
        }*/
        //====================================================================
        //Correr
        //====================================================================

        if (EstaEnElObjetivo())
        {
            _animator.SetBool(ANIM_CORRER, false); //Esta quieto
        }
        else
        {
            _animator.SetBool (ANIM_CORRER, _correr); //Se mueve y puede que corra
        }

        //====================================================================
        //Se suicida, solo para probar
        if (Input.GetKeyDown(KeyCode.K))
        {
            _estadoAnimacionMuerte = true;
            _estaMuriendo = true;
        }

        //====================================================================
    }

    void FixedUpdate()
    {
        if (!_estaMuriendo)
        {
            RealizarMovimiento();
        }
    }

    //====================================================================
    //Metodos para el movimiento
    private void RealizarMovimiento()
    {
        _animator.SetBool(ANIM_MOVERSE, !EstaEnElObjetivo());
        Moverse();
    }
    //====================================================================
}
//====================================================================
