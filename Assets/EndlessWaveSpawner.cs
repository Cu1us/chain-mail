using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class EndlessWaveSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject sword;
    [SerializeField] GameObject archer;
    [SerializeField] GameObject hammer;
    Transform player1;
    [SerializeField] Transform[] spawnPositions;

    int swordCost = 10;
    int hammerCost = 20;
    int archerCost = 20;
    Vector2 pos;

    List<GameObject> spawnList = new List<GameObject>();

    public enum PrefabToSpawn
    {
        SWORD, HAMMER, ARCHER,
    }
    public PrefabToSpawn prefab;
    int WaveCost;

    void Start()
    {
        player1 = GameObject.Find("Player1").GetComponent<Transform>();
        WaveCost = 10;
        SpawnList();
    }

    void Update()
    {
        if(EnemyMovement.EnemyList.Count == 0)
        {
            SpawnList();
        }
    }

    void SpawnList()
    {
        int cost = WaveCost;
        while (cost > 0)
        {
            int rnd = UnityEngine.Random.Range(0, 3);
            switch (rnd)
            {
                case 0:
                    if (cost >= swordCost)
                    {
                        spawnList.Add(sword);
                        cost -= swordCost;
                    }
                    break;
                case 1:
                    if (cost >= hammerCost)
                    {
                        spawnList.Add(hammer);
                        cost -= hammerCost;
                    }
                    break;
                case 2:
                    if (cost >= archerCost)
                    {
                        spawnList.Add(archer);
                        cost -= archerCost;
                    }
                    break;
            }

        }

        AddWaveCost();
        SpawnPosition();
        SpawnWave();
    }

    void AddWaveCost()
    {
        WaveCost += 50;
    }
    void SpawnPosition()
    {
        pos = Vector2.zero;
        float highestDistToTarget = 0;
        foreach(Transform i in spawnPositions)
        {
            float distToTarget = Vector2.Distance(i.position ,player1.position);
            if(distToTarget > highestDistToTarget)
            {
                pos = i.position;
                highestDistToTarget = distToTarget;
            }
        }
    }
    void SpawnWave()
    {
        while(spawnList.Count != 0)
        {
            Instantiate(spawnList[0], pos, quaternion.identity);
            spawnList.RemoveAt(0);
        }
    }
}
