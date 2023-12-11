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
    public void CreateObjectPool(string objType, GameObject prefab, int poolSize)
    {
        if (!objectPools.ContainsKey(objType))
        {
            objectPools[objType] = new List<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                objectPools[objType].Add(obj);
            }
        }
    }

    // Ǯ���� ������Ʈ ��������
    public GameObject GetObjectFromPool(string objType)
    {
        if (objectPools.ContainsKey(objType))
        {
            foreach (GameObject obj in objectPools[objType])
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
    public void ReturnObjectToPool(string objType, GameObject obj)
    {
        if (objectPools.ContainsKey(objType))
        {
            obj.SetActive(false);
        }
    }
}