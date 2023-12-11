using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public GameObject[] specialTilePrefab; // 내가 원하는 특별한 타일

    private Transform playerTransform;
    private float spawnZ = -6f;
    private float tileLength = 80.0f;
    private float safeZone = 200f;
    private int amnTileOnScreen = 13;
    private int tilesUntilSpecial = 20; // 내가 원하는 타일이 생성되기 전까지의 랜덤 타일 수
    private int lastPrefabIndex = 0;
    private int tileCount = 0;

    private List<GameObject> activeTiles;

    private void Start()
    {
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // 오브젝트 풀 관리자 초기화
        InitializeObjectPool();

        // 초기에는 랜덤 타일로 시작
        for (int i = 0; i < amnTileOnScreen; i++)
        {
            SpawnTile();
        }
    }

    private void InitializeObjectPool()
    {
        // 랜덤 타일 프리팹에 대한 오브젝트 풀 생성
        foreach (var tile in tilePrefabs)
        {
            PoolManager.Instance.CreateObjectPool(tile.name, tile, 10);
        }

        // 특별한 타일 프리팹에 대한 오브젝트 풀 생성
        foreach (var specialTile in specialTilePrefab)
        {
            PoolManager.Instance.CreateObjectPool(specialTile.name, specialTile, 2);
        }
    }

    private void Update()
    {
        // 플레이어가 안전 영역에 들어가면 새로운 타일 생성
        if (playerTransform.position.z - safeZone > (spawnZ - amnTileOnScreen * tileLength))
        {
            // 내가 원하는 타일 생성 조건
            if (tileCount >= tilesUntilSpecial && Random.Range(0, 100) < 30)
            {
                // 특별한 타일을 3개 생성하는 부분 수정
                SpawnSpecialTile(0);
                SpawnSpecialTile(1);
                SpawnSpecialTile(2);

                tileCount = 0; // 카운트 초기화
            }
            else
            {
                SpawnTile(); // 랜덤 타일 생성
                tileCount++; // 카운트 증가
            }

            DeleteTile(); // 가장 오래된 타일 삭제
        }
    }

    void SpawnTile()
    {
        // 랜덤 타일을 오브젝트 풀에서 가져와서 활성화
        GameObject go = PoolManager.Instance.GetObjectFromPool(tilePrefabs[RandomPrefabIndex()].name) as GameObject;
        SetTilePositionAndRotation(go);
    }

    void SpawnSpecialTile(int index)
    {
        // 특별한 타일을 오브젝트 풀에서 가져와서 활성화
        GameObject go = PoolManager.Instance.GetObjectFromPool(specialTilePrefab[index].name) as GameObject;
        SetTilePositionAndRotation(go);
    }

    void SetTilePositionAndRotation(GameObject tile)
    {
        // 타일의 위치를 다음 스폰 위치로, 회전을 기본 회전으로 설정
        tile.transform.position = transform.forward * spawnZ;
        tile.transform.rotation = transform.rotation;
        tile.SetActive(true);
        spawnZ += tileLength;
        activeTiles.Add(tile);
    }

    void DeleteTile()
    {
        // 가장 오래된 타일 비활성화
        activeTiles[0].SetActive(false);

        // 활성 타일 리스트에서 가장 오래된 타일 제거
        activeTiles.RemoveAt(0);
    }

    private int RandomPrefabIndex()
    {
        if (tilePrefabs.Length <= 1)
            return 0;

        int randomIndex = lastPrefabIndex;

        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefabs.Length);
        }

        lastPrefabIndex = randomIndex;

        return randomIndex;
    }
}
