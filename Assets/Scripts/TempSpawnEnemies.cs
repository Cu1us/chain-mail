using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TempSpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] float spawnEnemiesTimeDelay = 5;
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemies),5f,spawnEnemiesTimeDelay);
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
