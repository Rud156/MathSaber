using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedCubesScript : MonoBehaviour
{
    public float speed;
    public float destroyAreaZ;
    public float stopAreaZ;
    public float targetTime;

    private float _currentTimer;
    private float _currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _currentTimer = targetTime;
        _currentSpeed = speed;
    }


    // Update is called once per frame
    void Update()
    {
        
        Vector3 pos = transform.position;
        pos.z += _currentSpeed * Time.deltaTime;
        transform.position = pos;

        //stop for few seconds
        if(pos.z>=stopAreaZ && _currentTimer > 0)
        {
            _currentSpeed = 0;
            _currentTimer -= Time.deltaTime;
            if(_currentTimer <= 0)
            {
                _currentSpeed = speed;
            }

        }

        if (pos.z>destroyAreaZ)
        {
            Destroy(this.gameObject);

        }


    }
    void Stop()
    { 
    }
}
