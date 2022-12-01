using UnityEngine;

public class GameManager : MonoBehaviour {
    //====================================================================
    /* Clase para controlar estados y objetos estaticos del juego
     */
    //====================================================================  
    //Variables
    //====================================================================
    public GameObject _puntero;
    public static GameManager InstanciaActual;
     //====================================================================  
    //Metodos
    //====================================================================
    private void Awake() {
        InstanciaActual=this;
    }

}