using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public int managerNum;               // �Ŵ��� �ĺ���
    public GameObject vehiclePrefab;     // ���� ������
    public GameObject Agent;             // ������Ʈ�� ���� ����
    public int X_margin = 10;            // ���� ���� ���θ� Ȯ���ϱ� ���� X �� ���� ����
    public int Y_margin = 10;            // ���� ���� ���θ� Ȯ���ϱ� ���� Y �� ���� ����
    public int WaitTime_min = 2;         // ���ο� ���� ���� �� �ּ� ��� �ð�
    public int WaitTime_max = 8;         // ���ο� ���� ���� �� �ִ� ��� �ð�
    public Transform spawnPoints;        // ���� ���� ��ġ

    private float sum_time = 0f;          // ����� �ð��� ��
    private float wait_time;              // �������� ������ ��� �ð�
    private float Num_vehicles;           // ������ ������ ��
    private GameObject[] vehicleObjects;  // ������ ������ �迭
    private bool isOthers = false;        // �ٸ� ������ �����ϴ��� ���θ� ��Ÿ���� �÷���

    // ��ũ��Ʈ�� ���۵� �� ȣ��Ǵ� �Լ�
    void Start()
    {
        // ���� �������� �̿��Ͽ� ������Ʈ Ǯ�� ����
        PoolManager.Instance.CreateObjectPool(vehiclePrefab.tag, vehiclePrefab, 6);
    }

    // �� �����Ӹ��� ȣ��Ǵ� �Լ�
    private void FixedUpdate()
    {
        // �ٸ� ������ ���ٰ� ����
        isOthers = false;

        // ��� �ð� ����
        sum_time += Time.deltaTime;

        // ��� �ð� �������� ����
        wait_time = Random.Range(WaitTime_min, WaitTime_max);

        // ��� �ð��� ����ϸ� ������ ����
        if (sum_time > wait_time)
        {
            sum_time = 0; // ��� �ð� �ʱ�ȭ

            // �ٸ� ���� �� ��ġ ���
            int other_lane = Random.Range(1, 4);
            int other_x = CalculateOtherXPosition(other_lane);
            int other_z = CalculateOtherZPosition();

            // ���� ��ġ ���� ���
            vehicleObjects = GameObject.FindGameObjectsWithTag(vehiclePrefab.tag);
            Vector3 spawnVector = new Vector3(other_x, spawnPoints.position.y, spawnPoints.position.z + other_z);



            // �ٸ� ������ �����ϴ��� Ȯ��
            isOthers = CheckIfOtherVehiclePresent(spawnVector);

            // ���� ���� ������ ���
            if (vehicleObjects.Length < Num_vehicles && !isOthers)
            {
                Vector3 spawnPosition = spawnVector;
                GameObject spawnedVehicle = PoolManager.Instance.GetObjectFromPool(vehiclePrefab.tag);

                // ������ ������ ��ġ �� Ȱ��ȭ ����
                spawnedVehicle.transform.position = spawnPosition;
                spawnedVehicle.SetActive(true);
                spawnedVehicle.transform.parent = Agent.transform;
            }
        }
        
        // ������Ʈ Ǯ�� �� ���� ������ �ִٸ� ��ȯ
        if (vehicleObjects != null)
        {
            if (vehicleObjects.Length > Num_vehicles)
            {
                // ������Ʈ�� Ǯ�� ��ȯ
                PoolManager.Instance.ReturnObjectToPool(vehiclePrefab.tag, vehicleObjects[0]);
            }
        }
    }

    // �ٸ� ���������� X ��ġ ���
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

    // �ٸ� Z ��ġ ���
    private int CalculateOtherZPosition()
    {
        return Random.Range(Random.Range(0, 1f) < 0.7f ? 30 : -60, Random.Range(0, 1f) < 0.7f ? 90 : -30);
    }

    // �ٸ� ������ �����ϴ��� Ȯ��
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

    // �ܺο��� ȣ���Ͽ� ����� ���� �� ����
    public void SetCustomValue(float value)
    {
        // ���⿡�� ���� ��(value)�� ���
        // ���� ���, Num_vehicles = Mathf.Floor(value / 8f); �� ���� ������ ����
        Num_vehicles = Mathf.Floor(value / 8f);

        if (value % 8 > (managerNum - 1))
        {
            Num_vehicles = Num_vehicles + 1;
        }
    }
}
