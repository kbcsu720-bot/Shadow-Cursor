using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform player;
    public GameObject[] mapPrefabs;
    public GameObject[] obstaclePrefabs;

    [Header("맵 생성 설정")]
    public float mapLength = 20f;
    public int initialMapCount = 5;
    public float destroyDistance = 30f;

    [Header("★ 안전 구역 설정 ★")]
    [Tooltip("게임 시작 시 장애물 없이 비워둘 '맵의 개수'입니다. (1 = 맵 1개 길이만큼 안전)")]
    public int safeZoneMapCount = 1; // 맵 1개 길이만큼 비워둠!

    private float spawnX = 0f;
    private float startPlayerX = 0f;
    private Queue<GameObject> activeMaps = new Queue<GameObject>();

    void Start()
    {
        if (player != null)
        {
            startPlayerX = player.position.x;
            spawnX = startPlayerX - mapLength;
        }

        for (int i = 0; i < initialMapCount; i++)
        {
            SpawnMap();
        }
    }

    void Update()
    {
        if (player != null && player.position.x > spawnX - (initialMapCount * mapLength))
        {
            SpawnMap();
            DeleteOldMap();
        }
    }

    void SpawnMap()
    {
        int mapIndex = Random.Range(0, mapPrefabs.Length);
        GameObject newMap = Instantiate(mapPrefabs[mapIndex], new Vector3(spawnX, 0, 0), Quaternion.identity);

        // =========================================================
        // [수정된 로직] 맵 1개 길이 * 1(safeZoneMapCount) 만큼을 절대 안전구역으로 계산
        // =========================================================
        float safeDistance = startPlayerX + (mapLength * safeZoneMapCount);

        if (spawnX > safeDistance && obstaclePrefabs.Length > 0)
        {
            SpawnObstacles(newMap.transform);
        }

        activeMaps.Enqueue(newMap);
        spawnX += mapLength;
    }

    void SpawnObstacles(Transform parentMap)
    {
        int obsIndex = Random.Range(0, obstaclePrefabs.Length);

        // 맵의 정중앙(Y=0)에 1맵 1장애물 고정 소환
        Vector3 spawnPos = new Vector3(parentMap.position.x, 0f, 0f);

        GameObject obstacle = Instantiate(obstaclePrefabs[obsIndex], spawnPos, Quaternion.identity);
        obstacle.transform.SetParent(parentMap);
    }

    void DeleteOldMap()
    {
        if (activeMaps.Count > 0 && player != null && player.position.x - activeMaps.Peek().transform.position.x > destroyDistance)
        {
            Destroy(activeMaps.Dequeue());
        }
    }
}