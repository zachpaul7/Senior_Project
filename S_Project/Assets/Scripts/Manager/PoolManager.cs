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

    // 특정 차량 유형에 대한 오브젝트 풀 생성
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

    // 풀에서 오브젝트 가져오기
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
        return null; // 풀이 비어 있거나 생성되지 않았을 경우
    }

    // 오브젝트를 풀로 반환
    public void ReturnObjectToPool(string objType, GameObject obj)
    {
        if (objectPools.ContainsKey(objType))
        {
            obj.SetActive(false);
        }
    }
}