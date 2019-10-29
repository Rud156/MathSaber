using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
   // public Scene gameScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "Sword")
        {

           
        }
    }
}
