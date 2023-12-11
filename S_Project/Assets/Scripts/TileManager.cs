using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public GameObject[] specialTilePrefab; // ���� ���ϴ� Ư���� Ÿ��

    private Transform playerTransform;
    private float spawnZ = -6f;
    private float tileLength = 80.0f;
    private float safeZone = 200f;
    private int amnTileOnScreen = 13;
    private int tilesUntilSpecial = 20; // ���� ���ϴ� Ÿ���� �����Ǳ� �������� ���� Ÿ�� ��
    private int lastPrefabIndex = 0;
    private int tileCount = 0;

    private List<GameObject> activeTiles;

    private void Start()
    {
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // ������Ʈ Ǯ ������ �ʱ�ȭ
        InitializeObjectPool();

        // �ʱ⿡�� ���� Ÿ�Ϸ� ����
        for (int i = 0; i < amnTileOnScreen; i++)
        {
            SpawnTile();
        }
    }

    private void InitializeObjectPool()
    {
        // ���� Ÿ�� �����տ� ���� ������Ʈ Ǯ ����
        foreach (var tile in tilePrefabs)
        {
            PoolManager.Instance.CreateObjectPool(tile.name, tile, 10);
        }

        // Ư���� Ÿ�� �����տ� ���� ������Ʈ Ǯ ����
        foreach (var specialTile in specialTilePrefab)
        {
            PoolManager.Instance.CreateObjectPool(specialTile.name, specialTile, 2);
        }
    }

    private void Update()
    {
        // �÷��̾ ���� ������ ���� ���ο� Ÿ�� ����
        if (playerTransform.position.z - safeZone > (spawnZ - amnTileOnScreen * tileLength))
        {
            // ���� ���ϴ� Ÿ�� ���� ����
            if (tileCount >= tilesUntilSpecial && Random.Range(0, 100) < 30)
            {
                // Ư���� Ÿ���� 3�� �����ϴ� �κ� ����
                SpawnSpecialTile(0);
                SpawnSpecialTile(1);
                SpawnSpecialTile(2);

                tileCount = 0; // ī��Ʈ �ʱ�ȭ
            }
            else
            {
                SpawnTile(); // ���� Ÿ�� ����
                tileCount++; // ī��Ʈ ����
            }

            DeleteTile(); // ���� ������ Ÿ�� ����
        }
    }

    void SpawnTile()
    {
        // ���� Ÿ���� ������Ʈ Ǯ���� �����ͼ� Ȱ��ȭ
        GameObject go = PoolManager.Instance.GetObjectFromPool(tilePrefabs[RandomPrefabIndex()].name) as GameObject;
        SetTilePositionAndRotation(go);
    }

    void SpawnSpecialTile(int index)
    {
        // Ư���� Ÿ���� ������Ʈ Ǯ���� �����ͼ� Ȱ��ȭ
        GameObject go = PoolManager.Instance.GetObjectFromPool(specialTilePrefab[index].name) as GameObject;
        SetTilePositionAndRotation(go);
    }

    void SetTilePositionAndRotation(GameObject tile)
    {
        // Ÿ���� ��ġ�� ���� ���� ��ġ��, ȸ���� �⺻ ȸ������ ����
        tile.transform.position = transform.forward * spawnZ;
        tile.transform.rotation = transform.rotation;
        tile.SetActive(true);
        spawnZ += tileLength;
        activeTiles.Add(tile);
    }

    void DeleteTile()
    {
        // ���� ������ Ÿ�� ��Ȱ��ȭ
        activeTiles[0].SetActive(false);

        // Ȱ�� Ÿ�� ����Ʈ���� ���� ������ Ÿ�� ����
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
