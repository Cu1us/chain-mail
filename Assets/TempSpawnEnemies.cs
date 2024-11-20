using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TempSpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            SpawnEnemies();
        }
    }
    void SpawnEnemies()
    {
        Instantiate(enemy, transform.position, Quaternion.identity);
    }
}
