using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private VehicleManager[] vehicleManager;
    public float customValue; // 인스펙터에서 설정할 값

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
