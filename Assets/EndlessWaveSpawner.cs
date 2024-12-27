using System.Collections.Generic;
using Unity.Mathematics;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class EndlessWaveSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject sword;
    [SerializeField] GameObject archer;
    [SerializeField] GameObject hammer;
    [SerializeField] Transform[] spawnPositions;
    Transform player1;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreCounter;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] GameObject waveText;

    float totalScore;
    int currentWave;

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
        WaveText();
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

    public void AddScore(float newScore)
    {
        totalScore += newScore;
        scoreCounter.text = totalScore.ToString();
    }

    void WaveText()
    {
        currentWave++;
        waveCounter.text = currentWave.ToString();

        waveText.GetComponent<RectTransform>().DOAnchorPosX(0, 2).SetEase(Ease.InOutQuad).OnComplete(() =>
        waveText.GetComponent<RectTransform>().DOAnchorPosX(1100, 2).SetEase(Ease.InOutQuad).OnComplete(() =>
        waveText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1100, 0)));
    }
}
