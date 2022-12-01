using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoDeCamara : MonoBehaviour
{
    public float _velocidad;
    public float _velocidadDelZoom;
    public float _velocidadDeRotacion;
    private Camera _nCamera;

    // Start is called before the first frame update
    void Start()
    {
        _nCamera= GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            //====================================================================
            //Movimiento
            //====================================================================
            if (Input.GetAxis("Horizontal") != 0)
            {

                Vector3 objetivo = transform.position;
                objetivo.z -= Input.GetAxis("Horizontal");
                transform.position = Vector3.MoveTowards(transform.position, objetivo, Time.deltaTime * _velocidad);
            }
            if (Input.GetAxis("Vertical") != 0)
            {

                Vector3 objetivo = transform.position;
                objetivo.x += Input.GetAxis("Vertical");
                transform.position = Vector3.MoveTowards(transform.position, objetivo, Time.deltaTime * _velocidad);
            }
            //====================================================================
            //Zoom
            //====================================================================
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                float visionBase = _nCamera.fieldOfView;                
                visionBase -= Input.mouseScrollDelta.y;
                if (visionBase > 30) { visionBase = 30; }
                if (visionBase < 10) { visionBase = 10; }
                _nCamera.fieldOfView=visionBase; 
            }
         
        }
    }
}
