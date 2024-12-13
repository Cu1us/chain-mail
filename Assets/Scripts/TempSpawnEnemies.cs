using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemy2;
    GameObject enemyToSpawn;
    [SerializeField] float spawnEnemiesTimeDelay = 5;
    public float enemiesToSpawn;
    
    void Start()
    {
        GameObject.Find("NextLevel").GetComponent<DoorNextLevel>().isLevelCleared = false;
        Invoke(nameof(SpawnEnemies), 5f);

        //InvokeRepeating(nameof(SpawnEnemies),5f,spawnEnemiesTimeDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SpawnEnemies();
        }
    }
    void SpawnEnemies()
    {
        if (Random.Range(0, 2) == 0)
        {
            enemyToSpawn = enemy;
        }
        else
        {
            enemyToSpawn = enemy2;
        }
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        enemiesToSpawn--;
        if (enemiesToSpawn > 0)
        {
            Invoke(nameof(SpawnEnemies), spawnEnemiesTimeDelay);
        }
        else
        {
            GameObject.Find("NextLevel").GetComponent<DoorNextLevel>().isLevelCleared = true;
        }
    }
}
