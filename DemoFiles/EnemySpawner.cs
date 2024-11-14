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
            Vector2 randomPos = new Vector2(Random.Range(-6, 6), Random.Range(-6, 6));
            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            spawnInterval = 0;
        }
    }
}
