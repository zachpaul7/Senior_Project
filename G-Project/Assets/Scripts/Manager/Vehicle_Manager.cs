using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public int managerNum;               // 매니저 식별자
    public GameObject vehiclePrefab;     // 차량 프리팹
    public GameObject Agent;             // 에이전트에 대한 참조
    public int X_margin = 10;            // 차량 존재 여부를 확인하기 위한 X 축 여유 범위
    public int Y_margin = 10;            // 차량 존재 여부를 확인하기 위한 Y 축 여유 범위
    public int WaitTime_min = 2;         // 새로운 차량 생성 전 최소 대기 시간
    public int WaitTime_max = 8;         // 새로운 차량 생성 전 최대 대기 시간
    public Transform spawnPoints;        // 차량 생성 위치

    private float sum_time = 0f;          // 경과한 시간의 합
    private float wait_time;              // 랜덤으로 생성된 대기 시간
    private float Num_vehicles;           // 생성할 차량의 수
    private GameObject[] vehicleObjects;  // 생성된 차량의 배열
    private bool isOthers = false;        // 다른 차량이 존재하는지 여부를 나타내는 플래그

    // 스크립트가 시작될 때 호출되는 함수
    void Start()
    {
        // 차량 프리팹을 이용하여 오브젝트 풀을 생성
        PoolManager.Instance.CreateObjectPool(vehiclePrefab.tag, vehiclePrefab, 6);
    }

    // 매 프레임마다 호출되는 함수
    private void FixedUpdate()
    {
        // 다른 차량이 없다고 가정
        isOthers = false;

        // 경과 시간 누적
        sum_time += Time.deltaTime;

        // 대기 시간 랜덤으로 생성
        wait_time = Random.Range(WaitTime_min, WaitTime_max);

        // 대기 시간이 경과하면 차량을 생성
        if (sum_time > wait_time)
        {
            sum_time = 0; // 경과 시간 초기화

            // 다른 차선 및 위치 계산
            int other_lane = Random.Range(1, 4);
            int other_x = CalculateOtherXPosition(other_lane);
            int other_z = CalculateOtherZPosition();

            // 생성 위치 벡터 계산
            vehicleObjects = GameObject.FindGameObjectsWithTag(vehiclePrefab.tag);
            Vector3 spawnVector = new Vector3(other_x, spawnPoints.position.y, spawnPoints.position.z + other_z);



            // 다른 차량이 존재하는지 확인
            isOthers = CheckIfOtherVehiclePresent(spawnVector);

            // 차량 생성 가능한 경우
            if (vehicleObjects.Length < Num_vehicles && !isOthers)
            {
                Vector3 spawnPosition = spawnVector;
                GameObject spawnedVehicle = PoolManager.Instance.GetObjectFromPool(vehiclePrefab.tag);

                // 생성된 차량의 위치 및 활성화 설정
                spawnedVehicle.transform.position = spawnPosition;
                spawnedVehicle.SetActive(true);
                spawnedVehicle.transform.parent = Agent.transform;
            }
        }
        
        // 오브젝트 풀에 더 많은 차량이 있다면 반환
        if (vehicleObjects != null)
        {
            if (vehicleObjects.Length > Num_vehicles)
            {
                // 오브젝트를 풀에 반환
                PoolManager.Instance.ReturnObjectToPool(vehiclePrefab.tag, vehicleObjects[0]);
            }
        }
    }

    // 다른 차선에서의 X 위치 계산
    private int CalculateOtherXPosition(int lane)
    {
        switch (lane)
        {
            case 1:
                return 0;
            case 2:
                return 10;
            case 3:
                return 20;
            case 4:
                return 30;
            default:
                return 10;
        }
    }

    // 다른 Z 위치 계산
    private int CalculateOtherZPosition()
    {
        return Random.Range(Random.Range(0, 1f) < 0.7f ? 30 : -60, Random.Range(0, 1f) < 0.7f ? 90 : -30);
    }

    // 다른 차량이 존재하는지 확인
    private bool CheckIfOtherVehiclePresent(Vector3 spawnVector)
    {
        foreach (GameObject vehicleObj in vehicleObjects)
        {
            

            float objX = vehicleObj.transform.position.x;
            float objZ = vehicleObj.transform.position.z;
            
            if (spawnVector.x > objX - X_margin && spawnVector.x < objX + X_margin &&
                spawnVector.z - Y_margin < objZ && spawnVector.z + Y_margin > objZ)
            {
                return true;
            }
        }

        return false;
    }

    // 외부에서 호출하여 사용자 지정 값 설정
    public void SetCustomValue(float value)
    {
        // 여기에서 받은 값(value)을 사용
        // 예를 들어, Num_vehicles = Mathf.Floor(value / 8f); 와 같은 로직을 수행
        Num_vehicles = Mathf.Floor(value / 8f);

        if (value % 8 > (managerNum - 1))
        {
            Num_vehicles = Num_vehicles + 1;
        }
    }
}
