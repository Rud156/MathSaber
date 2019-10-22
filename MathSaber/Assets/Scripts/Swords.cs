using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swords : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collider");
        GameObject collidedWith = collision.gameObject;
        if(collidedWith.tag=="SpawnedCubes")
        {
            Destroy(collidedWith);
        }
    }
}
