using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    //================================================================================
    //Clase para intanciar el mapa
    //================================================================================
    //Variables
    //================================================================================
    private MapGeneratorController _mapGeneration;
    public int X, Y;
    public int cantidadDeBloques;    
    public Stack<GameObject> _bloquesInstanciados;
    private bool _mapaCargado;
    private int _ultimaEntradaEstablecida;
    public bool MapaCargado { get => _mapaCargado; }
   [SerializeField] List<GameObject> _bloquesDeNivel;
    //--------------------------------------------------------------------------------
    //Singleton
    //--------------------------------------------------------------------------------
    public static MapController Instancia;
    //================================================================================
    //Metodos
    //================================================================================
    private void InstanciarBloque(CellController celda)
    {
        /*
         * Tipos de mapa con su orientacion
         * | = 1-2-4-5-8-10
         * L = 3-6-9-12 
         * T = 7-11-13-14
         * X = 15
         */
        GameObject bloqueAInstanciar = null;
        if (celda.Direccion == DireccionDelBloque.Arriba || celda.Direccion == DireccionDelBloque.Derecha ||
            celda.Direccion == DireccionDelBloque.Abajo || celda.Direccion == DireccionDelBloque.ArribaAbajo ||
            celda.Direccion == DireccionDelBloque.Izquierda || celda.Direccion == DireccionDelBloque.IzquierdaDerecha)
        {
            bloqueAInstanciar = BloqueAInstanciar(TipoBloque.I);
        }
        if (celda.Direccion == DireccionDelBloque.ArribaDerecha || celda.Direccion == DireccionDelBloque.DerechaAbajo ||
           celda.Direccion == DireccionDelBloque.IzquierdaArriba || celda.Direccion == DireccionDelBloque.IzquierdaAbajo)
        {
            bloqueAInstanciar = BloqueAInstanciar(TipoBloque.L);
        }
        if (celda.Direccion == DireccionDelBloque.ArribaDerechaAbajo || celda.Direccion == DireccionDelBloque.IzquierdaDerechaArriba ||
           celda.Direccion == DireccionDelBloque.IzquierdaArribaAbajo || celda.Direccion == DireccionDelBloque.IzquierdaDerechaAbajo)
        {
            bloqueAInstanciar = BloqueAInstanciar(TipoBloque.T);
        }
        if (celda.Direccion == DireccionDelBloque.Todas)
        {
            bloqueAInstanciar = BloqueAInstanciar(TipoBloque.X);
        }
        GameObject temp = Instantiate(bloqueAInstanciar, celda.transform.position, RotarBloque(celda));
        _bloquesInstanciados.Push(temp);
    }
    private Quaternion RotarBloque(CellController celda)
    {
        //Rota la piesa para la orientacion correcta       
        Quaternion rotacion = new Quaternion();
        switch (celda.Direccion)
        {
            case DireccionDelBloque.Abajo:
            case DireccionDelBloque.Arriba:
            case DireccionDelBloque.ArribaAbajo:
                rotacion = Quaternion.Euler(0, 90, 0);
                break;
            case DireccionDelBloque.IzquierdaAbajo:
                rotacion = Quaternion.Euler(0, -90, 0);
                break;
        }
        return rotacion;
    }
    //--------------------------------------------------------------------------------
    //Selecionar bloques de nivel
    //--------------------------------------------------------------------------------
    private GameObject BloqueAInstanciar(TipoBloque tipoBloque)
    {
        /*
         * Busca a instanciar un bloque de nivel de los almacenados deacuerdo al tipo
         */

        return _bloquesDeNivel[(int)tipoBloque];
    }
    //--------------------------------------------------------------------------------
    //Generar mapas
    //--------------------------------------------------------------------------------
    public void CrearMapa()
    {
        DestruirBloquesDeNivel();//Limpia el mapa anterior si lo hay
        CrearMapaSalas();
    }
    private void CrearMapaSalas()
    {
        if (_mapGeneration != null)
            if (_mapGeneration.MapComplete)
            {
                int count = 0;
                foreach (CellController celda in _mapGeneration.UseCells)
                {
                    InstanciarBloque(celda);
                    count++;
                }
            }
    }  
    
    //--------------------------------------------------------------------------------
    //Destrir mapas
    //--------------------------------------------------------------------------------
    public void DestruirBloquesDeNivel()
    {
        _mapaCargado = false;
        foreach (GameObject bloque in _bloquesInstanciados)
            Destroy(bloque.gameObject);
        _bloquesInstanciados.Clear();
    }
    //--------------------------------------------------------------------------------
    //Metodos unity
    //--------------------------------------------------------------------------------
    void Awake()
    {
        //--------------------------------------------------------------------------------
        //Singleton
        //--------------------------------------------------------------------------------
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
        //--------------------------------------------------------------------------------
        //Declaracion de variables
        //--------------------------------------------------------------------------------
        _mapGeneration = GetComponent<MapGeneratorController>();
        _mapaCargado = false;
        _bloquesInstanciados = new Stack<GameObject>(); 
    }
    private void Start()
    {
        _mapGeneration.CreateMapStruct(cantidadDeBloques, X, Y);
        CrearMapaSalas();
    }
    private void Update()
    {
        //Determina si ya cargo todo el mapa
        if (!_mapaCargado)
        {
            if (_bloquesInstanciados.Count == cantidadDeBloques)
            {
                _mapGeneration.Clear();
                _mapaCargado = true;
            }
        }
    }
}

