using UnityEngine;
using UnityEngine.AI;

public class PersonajeController : PersonajeEstadisticas
{
    //====================================================================
    /* Clase para base para controlar los personajes sin importar si son
     * controlados por la IA o el jugador
     */
    //====================================================================
    //Animiaciones disponibles
    //====================================================================
    [HideInInspector]
    public const string ANIM_MUERTE = "muerte", ANIM_MOVERSE = "moverse";
    [HideInInspector] public Animator _animator;
    //====================================================================
    [HideInInspector]
    public NavMeshAgent _agente;

    private NavMeshObstacle _obstaculo;

    [SerializeField]
    private Vector3 _objetivo;

    //====================================================================
    //Metodos
    //====================================================================
    //Metodos Unity
    //====================================================================
    void Start()
    {
        IniciarVariables();
    }

    void FixedUpdate()
    {
        Moverse();
    }

    //====================================================================
    //Movimiento
    //====================================================================
    public void Moverse()
    {
        if (EstaEnElObjetivo())
        {
            _agente.enabled = false;
            _obstaculo.enabled = true;
        }
        else
        {
            _obstaculo.enabled = false;
            _agente.enabled = true;
            _agente.SetDestination (_objetivo);
        }
    }

    public void EstablecerObjetivoDeMovimiento(Vector3 objetivo)
    {
        _objetivo = objetivo;
    }

    private bool LaPosicionYObjetivosIguales()
    {
        //ayuda cuando el objetivo ha cambiado y el agente estaba apagado
        //(Vector3.Distance(transform.position,_objetivo))<1
        int x = (int)(transform.position.x);
        int y = (int)(transform.position.y);
        int z = (int)(transform.position.z);
        Vector3 posicion = new Vector3Int(x, 0, z);
        x = (int)(_objetivo.x);
        y = (int)(_objetivo.y);
        z = (int)(_objetivo.z);
        Vector3 objetivo = new Vector3Int(x, 0, z);

        /*Debug.Log("posicion" + posicion);
        Debug.Log("objetivo" + objetivo);

        Debug.Log(posicion == objetivo);
        Debug.Log(_agente.remainingDistance < 0.1);
*/
        if (posicion == objetivo)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool DistanciaDespreciable()
    {
        if (_agente.enabled)
        {
            if (_agente.remainingDistance < 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool EstaEnElObjetivo()
    {
        if (LaPosicionYObjetivosIguales() && DistanciaDespreciable())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //====================================================================
    public void IniciarVariables()
    {
        _agente = GetComponent<NavMeshAgent>();
        _agente.speed = _velocidad;
        _obstaculo = GetComponent<NavMeshObstacle>();
        _objetivo = transform.position;
        _animator = GetComponent<Animator>();
    }
}
//====================================================================
