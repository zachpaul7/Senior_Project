using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    private Dictionary<string, List<GameObject>> objectPools = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    // Ư�� ���� ������ ���� ������Ʈ Ǯ ����
    public void CreateObjectPool(string vehicleType, GameObject prefab, int poolSize)
    {
        if (!objectPools.ContainsKey(vehicleType))
        {
            objectPools[vehicleType] = new List<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab,transform);
                obj.SetActive(false);
                objectPools[vehicleType].Add(obj);
            }
        }
    }

    // Ǯ���� ������Ʈ ��������
    public GameObject GetObjectFromPool(string vehicleType)
    {
        if (objectPools.ContainsKey(vehicleType))
        {
            foreach (GameObject obj in objectPools[vehicleType])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
        }

        return null; // Ǯ�� ��� �ְų� �������� �ʾ��� ���
    }

    // ������Ʈ�� Ǯ�� ��ȯ
    public void ReturnObjectToPool(string vehicleType, GameObject obj)
    {
        if (objectPools.ContainsKey(vehicleType))
        {
            obj.SetActive(false);
        }
    }
}