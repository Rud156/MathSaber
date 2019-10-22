using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedCubesScript : MonoBehaviour
{
    public float speed;
    public float destroyAreaZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.z += speed * Time.deltaTime;
        transform.position = pos;

        if (pos.z>destroyAreaZ)
        {
            Destroy(this.gameObject);

        }


    }
}
