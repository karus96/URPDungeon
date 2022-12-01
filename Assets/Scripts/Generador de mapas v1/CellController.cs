using System;
using UnityEngine;
/// 
/// Creado por Ricardo Apuy
/// 
public class CellController : MonoBehaviour
{
    //================================================================================
    // Clase auxiliar para generar la informacion de los mapas
    //================================================================================
    [NonSerialized] public byte _orientation;    
    public byte _openNeighbours { get => OpenNeighbours(); }    
    public DireccionDelBloque Direccion { get =>(DireccionDelBloque)_orientation; }
    //--------------------------------------------------------------------------------
    //Ubicacion en la matriz
    //--------------------------------------------------------------------------------
    private int X, Y; //No tienen uso...
    //--------------------------------------------------------------------------------
    // Requerido para la busqueda en profundidad
    //--------------------------------------------------------------------------------
    [NonSerialized] public CellController[] _neighbours;
    [NonSerialized] public byte[] _byteNeighbours;    
    [NonSerialized] public bool _using;
    //================================================================================
    //Metodos
    //================================================================================
    private byte OpenNeighbours()
    {
        //Determina si los hay vecinos sin conectar
        byte answer = 0;
        for (int i = 0; i < _byteNeighbours.Length; i++)
        {
            answer |= _byteNeighbours[i];
        }
        return answer;
    }
    //--------------------------------------------------------------------------------
    //Funciona como constructor
    //--------------------------------------------------------------------------------
    public void Initialization(int x, int y)
    {
        X = x;
        Y = y;
        _neighbours = new CellController[4];
        _byteNeighbours = new byte[4] { 0, 0, 0, 0 };
    }
    //================================================================================
}
//================================================================================