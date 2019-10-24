using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen_Particle : MonoBehaviour
{
    
    //public ParticleSystem tailer;
    public GameObject Pen;
    public float WalkSpeed;
    public float RotateSpeed;

    public Camera MainCamera;
    public float CameraOffsetX;
    public float CameraOffsetY;

    public float x;
    public float y;

    public bool SwordStatus;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //tailer.transform.position = Pen.transform.position;
        walk1();
        CameraFollow1();

    }

    void walk1()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        transform.position += transform.forward * y * Time.deltaTime * WalkSpeed;
        transform.Rotate(transform.up * x * Time.deltaTime * RotateSpeed);


    }
    void CameraFollow1()
    {
        Vector3 targetPoint = Pen.transform.position;
        targetPoint.x += CameraOffsetX;
        targetPoint.y += CameraOffsetY;
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, targetPoint, Time.deltaTime * 10f);
    }
}
