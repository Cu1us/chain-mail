using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval;

    float spawnTimer;

    void Update()
    {
        spawnInterval += Time.deltaTime;

        if (spawnInterval > 3)
        {
            Instantiate(enemyPrefab);
            spawnInterval = 0;
        }
    }
}
