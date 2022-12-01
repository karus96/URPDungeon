using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;
/// 
/// Creado por Ricardo Apuy
/// 
public class MapGeneratorController : MonoBehaviour
{
    //================================================================================
    //Clase que controla la generacion de la estructura logica de mapas
    //================================================================================
    //Variables
    //================================================================================
    [NonSerialized] public List<CellController> UseCells;
    [SerializeField] private CellController _cell;
    private CellController[,] _map;
    private WanderingWalker _walker;
    private int _X, _Y;
    private bool _mapComplete;
    private int lastX, lastZ;
    private const int CELL_DISTANCE = 92;
    //--------------------------------------------------------------------------------
    public bool MapComplete { get => _mapComplete; }
    //================================================================================
    //Metodos
    //================================================================================
    //Metodos publicos
    //--------------------------------------------------------------------------------
    public void CreateMapStruct(int size, int x, int y)
    {
        /*
         * Crea el mapa
         * Establece los vecinos
         * Determina la estructrua logica del mapa
         * Destruye las celdas que no se usan
         */
        Clear();
        _X = x;
        _Y = y;
        CreateMap();
        CreateNeighbors();
        _walker = new WanderingWalker(_map[_X / 2, _Y / 2], _X * _Y);
        _mapComplete = false;
        //size no puede exeder _X*_Y
        if (size > _X * _Y) size = _X * _Y;
        CreateStructure(size);
        //Destruye las celdas que no usa y guarda las que si
        DestroyOrAddCells();
    }
    //--------------------------------------------------------------------------------
    //Metodos privados
    //--------------------------------------------------------------------------------
    private void CreateNeighbors()
    {
        //Establece los vecinos        
        for (int x = 0; x < _X; x++)
        {
            for (int y = 0; y < _Y; y++)
            {
                if (y + 1 < _Y)
                {
                    _map[x, y]._neighbours[(int)Direction.Up] = _map[x, y + 1];
                }
                if (y - 1 >= 0)
                {
                    _map[x, y]._neighbours[(int)Direction.Down] = _map[x, y - 1];
                }
                if (x + 1 < _X)
                {
                    _map[x, y]._neighbours[(int)Direction.Right] = _map[x + 1, y];
                }
                if (x - 1 >= 0)
                {
                    _map[x, y]._neighbours[(int)Direction.Left] = _map[x - 1, y];
                }
            }

        }
    }
    private void CreateMap()
    {
        //Crea las celdas
        _map = new CellController[_X, _Y];
        lastX = 0;
        lastZ = 0;
        int x = 0;
        for (int i = 0; i < _X; i++)
        {
            for (int j = 0; j < _Y; j++)
            {
                _map[i, j] = Instantiate(_cell, transform);
                _map[i, j].transform.position = new Vector3(lastX, 0, lastZ);
                _map[i, j].Initialization(i, j);
                _map[i, j].name = "_X:" + i + " _Y:" + j;
                lastZ += CELL_DISTANCE;
                x++;
            }
            lastZ = 0;
            lastX += CELL_DISTANCE;
        }
    }
    private void DestroyOrAddCells()
    {
        //Destruye las celdas inecesarias y almacena las usadas
        for (int x = 0; x < _X; x++)
        {
            for (int y = 0; y < _Y; y++)
            {
                if (_map[x, y]._using)
                {
                    UseCells.Add(_map[x, y]);
                }
                else
                {
                    Destroy(_map[x, y].gameObject);
                }
            }
        }
    }
    private void CreateStructure(int size)
    {
        int protection = 0;
        while (!_mapComplete)
        {
            //evita que se pegue
            if (protection >= 1000) _mapComplete = true;
            protection++;
            int counter = 0;
            for (int x = 0; x < _X; x++)
            {
                for (int y = 0; y < _Y; y++)
                {
                    if (_map[x, y]._using)
                    {
                        counter++;
                    }
                }
            }
            if (counter == size)
            {
                _walker._explore = false;
                _mapComplete = true;
            }
            else
            {
                _walker.BasicSearch();
            }
        }
    }
    //--------------------------------------------------------------------------------
    //Metodos Auxiliares
    //--------------------------------------------------------------------------------
    public static byte OppositeDirection(byte direcction)
    {
        direcction += 2;
        direcction &= 3;
        return direcction;
    }
    public static byte Rotation(CellController current)
    {
        List<int> directionsOpen = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            //Si la direccion esta libre
            if (current._byteNeighbours[i] == 0)
            {
                directionsOpen.Add(i + 1);
            }
        }
        byte randomNum = System.Convert.ToByte(directionsOpen[Random.Range(0, directionsOpen.Count)]);
        return randomNum;
    }
    public void Clear()
    {
        foreach (CellController cell in UseCells)
        {
            Destroy(cell.gameObject);
        }
        UseCells.Clear();
    }
    //--------------------------------------------------------------------------------
    //Metodos Unity
    //--------------------------------------------------------------------------------    
    private void Awake()
    {
         UseCells =  new List<CellController>();
    }
    //================================================================================
}
//================================================================================