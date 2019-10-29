using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour
{
    public GameObject DestroyVersion;
    // Start is called before the first frame update
    void Update()
    {

        Instantiate(DestroyVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
