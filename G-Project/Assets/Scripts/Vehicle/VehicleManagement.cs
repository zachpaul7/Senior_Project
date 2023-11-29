using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManagement : MonoBehaviour
{
    public float range = 100f;

    private GameObject[] hostVehicles; // ȣ��Ʈ ���� �迭
    private Rigidbody vehicleRigidbody; // ���� Ÿ���� Rigidbody

    void Start()
    {
        vehicleRigidbody = GetComponent<Rigidbody>(); // ������ �� Rigidbody ����
    }

    void FixedUpdate()
    {
        // ������ ��� ȣ��Ʈ ���� ����
        hostVehicles = GameObject.FindGameObjectsWithTag("Player");

        // ȣ��Ʈ ������ Ÿ�� ���� ������ �Ÿ� ���
        float distance = Vector3.Distance(hostVehicles[0].transform.position, vehicleRigidbody.position);

        // ������ ������ ����� �ı�
        if (distance > range)
        {
            ReturnToPool();
        }
    }

    // �ٸ� �������� �浹�ϸ� �ڽ��� �ı��ϴ� �Լ�
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