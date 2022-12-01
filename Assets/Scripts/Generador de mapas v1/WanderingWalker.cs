using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
/// 
/// Creado por Ricardo Apuy
/// 
public class WanderingWalker
{
    //================================================================================
    //Clase para construir la estructura del mapa
    //================================================================================
    //Variables
    //================================================================================
    private CellController _currentCell;
    private Stack<CellController> cells;
    private int _steps;
    public byte _direction;
    public byte _directionBeforeTheLast;
    public bool _explore;
    //================================================================================
    //Metodos
    //================================================================================
    //public void Rotation()
    //{   
    //    //forma vieja
    //   byte randomNum = Convert.ToByte(Random.Range(0, 4));
    //    if(randomNum == 0) { randomNum = 1; } else { randomNum = 3; }
    //    _direction += randomNum;
    //    _direction &= 3;    
    //}
    public void BasicSearch()
    {
        //Se mueve de forma aleatoria
        _direction = MapGeneratorController.Rotation(_currentCell);
        CellController cellIGoTo = _currentCell._neighbours[_direction];
        if (cellIGoTo != null)
        {
            if (!cells.Contains(cellIGoTo) || (cellIGoTo._openNeighbours < 15 && cellIGoTo._byteNeighbours[_direction] == 0))
            {
                //CellController de salida
                byte outputDirection = MapGeneratorController.OppositeDirection(_direction);
                _currentCell._orientation |= Convert.ToByte(Math.Pow(2, _direction));
                _currentCell._byteNeighbours[_direction] = Convert.ToByte(Math.Pow(2, outputDirection)); ;

                //CellController de entrada
                cellIGoTo._orientation |= Convert.ToByte(Math.Pow(2, outputDirection));
                cellIGoTo._byteNeighbours[outputDirection] = Convert.ToByte(Math.Pow(2, _direction));

                _currentCell._using = true;
                _currentCell = cellIGoTo;
                cells.Push(_currentCell);
            }
            else
            {
                _currentCell = cells.Pop();
            }
        }        
    }
    public void DeepSearch()
    {
        //Abre celdas una unica vez
        //Recorre toda la matriz sin fomar triple entradas en las celdas          
        _direction = MapGeneratorController.Rotation(_currentCell);
        CellController cellIGoTo = _currentCell._neighbours[_direction];
        if (cellIGoTo != null)
        {           
            if (cellIGoTo._openNeighbours < 15 && cellIGoTo._byteNeighbours[_direction]==0)
            {
                //CellController de salida
                byte outputDirection = MapGeneratorController.OppositeDirection(_direction);
                _currentCell._orientation |= Convert.ToByte(Math.Pow(2, _direction));
                _currentCell._byteNeighbours[_direction] = Convert.ToByte(Math.Pow(2, outputDirection)); ;
                
                //CellController de entrada
                cellIGoTo._orientation |= Convert.ToByte(Math.Pow(2, outputDirection));
                cellIGoTo._byteNeighbours[outputDirection] = Convert.ToByte(Math.Pow(2, _direction));

                _currentCell._using = true;
                _currentCell = cellIGoTo;
                cells.Push(_currentCell);                                              
            }
            else
            {
                _currentCell =cells.Pop();
            }
        }        
    }
    //================================================================================
    //Constructor
    //================================================================================
    public WanderingWalker(CellController currentCell, int steps)
    {
        cells = new Stack<CellController>();
        _currentCell = currentCell;
        _direction = Convert.ToByte(Direction.Up);
        _directionBeforeTheLast = _direction;
        cells.Push(_currentCell);
        _steps = steps;      
    }
    //================================================================================
}
//================================================================================