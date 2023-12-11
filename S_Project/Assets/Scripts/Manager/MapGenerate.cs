using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerate : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs;  // �� ������ ����� Ÿ�� ������ �迭
    [SerializeField] private float tileLength;         // �� Ÿ���� ����

    private Transform playerTransform;                 // �÷��̾��� Ʈ�������� ���� ����
    private float zSpawn = -6f;                         // Ÿ�� ���� ���� ��ġ�� �ʱ� z-��ǥ
    private float safeZone = 200f;                      // ���ο� Ÿ���� �����ϱ� ���� �÷��̾� ���� ���� ����
    private int amnTilesOnscreen = 15;                  // �� ���� ȭ�鿡 ������ Ÿ�� ��

    private List<GameObject> activeTiles;              // ������ Ȱ��ȭ�� Ÿ���� �����ϴ� ����Ʈ

    void Start()
    {
        // Ȱ�� Ÿ���� ������ ����Ʈ �ʱ�ȭ
        activeTiles = new List<GameObject>();

        // �÷��̾��� Ʈ������ ã�� ����
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // �� Ÿ�� �����տ� ���� ������Ʈ Ǯ ����
        foreach (var tile in tilePrefabs)
        {
            // PoolManager�� ����Ͽ� Ÿ�� �����ո��� ������Ʈ Ǯ�� �����ϸ� �ʱ� ũ��� 10���� ����
            PoolManager.Instance.CreateObjectPool(tile.name, tile, 10);
        }

        // �ʱ� Ÿ�� ���� ����
        for (int i = 0; i < amnTilesOnscreen; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        // �÷��̾ ���� ������ �Ѿ ���ο� Ÿ���� �����ؾ� �ϴ��� Ȯ��
        if (playerTransform.position.z - safeZone > (zSpawn - amnTilesOnscreen * tileLength))
        {
            // ���ο� Ÿ�� ���� �� ���� ������ Ÿ�� ����
            SpawnTile();
            DeleteTile();
        }
    }

    // ���� ���� ��ġ�� Ÿ�� ����
    public void SpawnTile()
    {
        // �������� Ÿ���� �����ϱ� ���� �ε����� �������� ����
        int prefabIndex = Random.Range(1, tilePrefabs.Length);

        // ������Ʈ Ǯ���� Ÿ�� ��������
        GameObject tile = PoolManager.Instance.GetObjectFromPool(tilePrefabs[prefabIndex].name);

        // Ÿ���� null�� �ƴ��� Ȯ�� (������Ʈ Ǯ�� ��� ������ ������Ʈ�� �ִ���)
        if (tile != null)
        {
             // ���� �ֱ⸶�� Ư�� Ÿ�� ��ġ
            if (activeTiles.Count % 70 == 0)
            {
                // 0�� Ÿ�� ��ġ
                SpawnSpecificTile(0);
            }
            else
            {
                // Ÿ���� ��ġ�� ���� ���� ��ġ��, ȸ���� �⺻ ȸ������ ����
                tile.transform.position = transform.forward * zSpawn;
                tile.transform.rotation = transform.rotation;

                // Ÿ�� Ȱ��ȭ
                tile.SetActive(true);

                // ���� ���� ��ġ ������Ʈ
                zSpawn += tileLength;

                // ������ Ÿ���� Ȱ�� Ÿ�� ����Ʈ�� �߰�
                activeTiles.Add(tile);
            }
        }
    }

    // Ư�� Ÿ���� �����ϴ� �Լ�
    private void SpawnSpecificTile(int index)
    {
        // ������Ʈ Ǯ���� 0�� Ÿ�� ��������
        GameObject specificTile = PoolManager.Instance.GetObjectFromPool(tilePrefabs[index].name);

        // Ÿ���� null�� �ƴ��� Ȯ�� (������Ʈ Ǯ�� ��� ������ ������Ʈ�� �ִ���)
        if (specificTile != null)
        {
            // 0�� Ÿ���� ��ġ�� ���� ���� ��ġ��, ȸ���� �⺻ ȸ������ ����
            specificTile.transform.position = transform.forward * zSpawn;
            specificTile.transform.rotation = transform.rotation;

            // 0�� Ÿ�� Ȱ��ȭ
            specificTile.SetActive(true);

            // ������ 0�� Ÿ���� Ȱ�� Ÿ�� ����Ʈ�� �߰�
            activeTiles.Add(specificTile);
        }
    }

    // ���� ������ Ÿ���� ��Ȱ��ȭ�ϰ� ����
    public void DeleteTile()
    {
        // ���� ������ Ÿ�� ��Ȱ��ȭ
        activeTiles[0].SetActive(false);

        // Ȱ�� Ÿ�� ����Ʈ���� ���� ������ Ÿ�� ����
        activeTiles.RemoveAt(0);
    }
}
