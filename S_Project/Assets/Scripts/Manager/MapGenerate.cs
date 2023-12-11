using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerate : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs;  // 맵 생성에 사용할 타일 프리팹 배열
    [SerializeField] private float tileLength;         // 각 타일의 길이

    private Transform playerTransform;                 // 플레이어의 트랜스폼에 대한 참조
    private float zSpawn = -6f;                         // 타일 생성 시작 위치의 초기 z-좌표
    private float safeZone = 200f;                      // 새로운 타일을 생성하기 전의 플레이어 앞의 안전 영역
    private int amnTilesOnscreen = 15;                  // 한 번에 화면에 유지할 타일 수

    private List<GameObject> activeTiles;              // 씬에서 활성화된 타일을 저장하는 리스트

    void Start()
    {
        // 활성 타일을 저장할 리스트 초기화
        activeTiles = new List<GameObject>();

        // 플레이어의 트랜스폼 찾아 저장
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // 각 타일 프리팹에 대해 오브젝트 풀 생성
        foreach (var tile in tilePrefabs)
        {
            // PoolManager를 사용하여 타일 프리팹마다 오브젝트 풀을 생성하며 초기 크기는 10으로 지정
            PoolManager.Instance.CreateObjectPool(tile.name, tile, 10);
        }

        // 초기 타일 집합 생성
        for (int i = 0; i < amnTilesOnscreen; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        // 플레이어가 안전 영역을 넘어서 새로운 타일을 생성해야 하는지 확인
        if (playerTransform.position.z - safeZone > (zSpawn - amnTilesOnscreen * tileLength))
        {
            // 새로운 타일 생성 및 가장 오래된 타일 삭제
            SpawnTile();
            DeleteTile();
        }
    }

    // 현재 스폰 위치에 타일 생성
    public void SpawnTile()
    {
        // 랜덤으로 타일을 선택하기 위해 인덱스를 랜덤으로 설정
        int prefabIndex = Random.Range(1, tilePrefabs.Length);

        // 오브젝트 풀에서 타일 가져오기
        GameObject tile = PoolManager.Instance.GetObjectFromPool(tilePrefabs[prefabIndex].name);

        // 타일이 null이 아닌지 확인 (오브젝트 풀에 사용 가능한 오브젝트가 있는지)
        if (tile != null)
        {
             // 일정 주기마다 특정 타일 배치
            if (activeTiles.Count % 70 == 0)
            {
                // 0번 타일 배치
                SpawnSpecificTile(0);
            }
            else
            {
                // 타일의 위치를 다음 스폰 위치로, 회전을 기본 회전으로 설정
                tile.transform.position = transform.forward * zSpawn;
                tile.transform.rotation = transform.rotation;

                // 타일 활성화
                tile.SetActive(true);

                // 다음 스폰 위치 업데이트
                zSpawn += tileLength;

                // 생성된 타일을 활성 타일 리스트에 추가
                activeTiles.Add(tile);
            }
        }
    }

    // 특정 타일을 스폰하는 함수
    private void SpawnSpecificTile(int index)
    {
        // 오브젝트 풀에서 0번 타일 가져오기
        GameObject specificTile = PoolManager.Instance.GetObjectFromPool(tilePrefabs[index].name);

        // 타일이 null이 아닌지 확인 (오브젝트 풀에 사용 가능한 오브젝트가 있는지)
        if (specificTile != null)
        {
            // 0번 타일의 위치를 다음 스폰 위치로, 회전을 기본 회전으로 설정
            specificTile.transform.position = transform.forward * zSpawn;
            specificTile.transform.rotation = transform.rotation;

            // 0번 타일 활성화
            specificTile.SetActive(true);

            // 생성된 0번 타일을 활성 타일 리스트에 추가
            activeTiles.Add(specificTile);
        }
    }

    // 가장 오래된 타일을 비활성화하고 제거
    public void DeleteTile()
    {
        // 가장 오래된 타일 비활성화
        activeTiles[0].SetActive(false);

        // 활성 타일 리스트에서 가장 오래된 타일 제거
        activeTiles.RemoveAt(0);
    }
}
