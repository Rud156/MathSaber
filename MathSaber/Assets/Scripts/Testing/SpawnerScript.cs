using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{

    // public Transform[] spawnLocation;
    // public GameObject[] whatToSpawnPrefab;
    // public GameObject[] whatToSpawnClone;


    // void spawnSomething()
    // {
    //     whatToSpawnClone[0] = Instantiate(whatToSpawnPrefab[0],spawnLocation[0].transform.position,Quaternion.Euler(0,0,0)) as GameObject;
    // }
    // Start is called before the first frame update

    public GameObject spawningPrefab;
    //public float speed;
    public float secondsBetweenEachObject;

    private float _currentTime;


    void Start()
    {
        _currentTime = secondsBetweenEachObject;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime -= Time.deltaTime;
        if(_currentTime <= 0)
        {
            SpawnObject();
            _currentTime = secondsBetweenEachObject;
        }
    }

    void SpawnObject()
    {
        Debug.Log("Spawning");
        GameObject spawn = Instantiate(spawningPrefab) as GameObject;
        spawn.transform.position = transform.position;
    }
}
