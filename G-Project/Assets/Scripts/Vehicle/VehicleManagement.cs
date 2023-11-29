using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManagement : MonoBehaviour
{
    public float range = 100f;

    private GameObject[] hostVehicles; // 호스트 차량 배열
    private Rigidbody vehicleRigidbody; // 현재 타겟의 Rigidbody

    void Start()
    {
        vehicleRigidbody = GetComponent<Rigidbody>(); // 시작할 때 Rigidbody 정의
    }

    void FixedUpdate()
    {
        // 범위를 벗어난 호스트 차량 삭제
        hostVehicles = GameObject.FindGameObjectsWithTag("Player");

        // 호스트 차량과 타겟 차량 사이의 거리 계산
        float distance = Vector3.Distance(hostVehicles[0].transform.position, vehicleRigidbody.position);

        // 차량이 범위를 벗어나면 파괴
        if (distance > range)
        {
            ReturnToPool();
        }
    }

    // 다른 차량끼리 충돌하면 자신을 파괴하는 함수
    void OnTriggerEnter(Collider other)
    {
        string[] otherTags = { "other1", "other2", "other3", "other4", "other5", "other6", "other7", "other8" };

        if (System.Array.IndexOf(otherTags, other.gameObject.tag) != -1)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        // Use ObjectPool Manager to return the object to the pool
        PoolManager.Instance.ReturnObjectToPool(gameObject.tag, gameObject);
    }
}