using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Particle_Follow : MonoBehaviour
{
    public ParticleSystem tailer;
    public GameObject Sword;
    public GameObject AllSword;
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
        
        tailer.transform.position = Sword.transform.position;
        walk();
        CameraFollow();
        
    }
 
    void walk()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        transform.position += transform.forward * y * Time.deltaTime * WalkSpeed;
        transform.Rotate(transform.up * x * Time.deltaTime * RotateSpeed);
        
       
    }
    void CameraFollow()
    {
        Vector3 targetPoint = AllSword.transform.position;
        targetPoint.x += CameraOffsetX;
        targetPoint.y += CameraOffsetY;
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, targetPoint, Time.deltaTime * 10f);
    }

}
