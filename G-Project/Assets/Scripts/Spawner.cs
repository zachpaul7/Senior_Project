using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private VehicleManager[] vehicleManager;
    public float customValue; // �ν����Ϳ��� ������ ��

    private void Update()
    {
        if (vehicleManager != null)
        {
            for(int i = 0; i < vehicleManager.Length; i++)
            {
                vehicleManager[i].SetCustomValue(customValue);
            }
        }
    }
}
