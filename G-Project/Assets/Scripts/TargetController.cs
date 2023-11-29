using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] wheels = new WheelCollider[4]; // ������ 4�� ���� �ݶ��̴� �迭
    [SerializeField] private Transform[] tires = new Transform[4]; // ������ 4�� ���� ����� ��Ÿ���� Transform �迭
    [SerializeField] private float targetSpeed = 5.0f; // ��ǥ �ӵ�
    [SerializeField] private float acceleration = 2.0f; // ���ӵ�
    [SerializeField] private float steeringAngle = 30f; // �ڵ� ���ۿ� ���� ���� ȸ�� ����
    [SerializeField] private float currentSpeed; // ���� �ӵ�
    [SerializeField] private float horizontalInput; // ���� �Է� ��
    

    private Rigidbody rb; // Rigidbody ������Ʈ

    // ���� ���� ���� ����
    [SerializeField] private bool isTurn = false; // ���� ���� ���� �÷���
    [SerializeField] private float[] lanePositions = { 0f, 10f, 20f }; // ���� ��ġ �迭
    private bool Is_right_lane_change = false;  // ���� ���� ���� ���� �ʱ�ȭ
    private bool Is_left_lane_change = false;   // ���� ���� ���� ���� �ʱ�ȭ
    private float start_y = 0;  // ���� ���� ���� ��ġ �ʱ�ȭ
    private int currentLane = 1; // ���� ����

    private bool isChangingLane = false; // ���� ���� ������ ���θ� ��Ÿ���� �÷���
    private float laneChangeDistance = 10f; // ���� ������ ���� �̵� �Ÿ�
    private float laneChangeSpeed = 5f; // ���� ���� �ӵ�

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // �ڽ��� Rigidbody ������Ʈ ���
    }

    private void FixedUpdate()
    {
        HandleMove(); // ������ ó��
        UpdateWheelVisuals(); // ������ �ð����� �κ� ������Ʈ
    }

    private void HandleMove()
    {
        if (isChangingLane)
        {
            // Lane change in progress
            float steeringChange = laneChangeSpeed * Time.fixedDeltaTime;
            AdjustSteering(steeringChange);

            int randWay = Random.Range(0, lanePositions.Length);

            // Check if lane change distance is reached
            if (Mathf.Abs(rb.position.x - lanePositions[randWay]) >= laneChangeDistance)
            {
                isChangingLane = false;
                ResetSteering(); // Reset steering angles when lane change is complete
            }
        }
        else
        {
            // Lane change requested
            if (isTurn)
            {
                ChangeLane();
            }

            HandleTorque();
            HandleSteer();
        }
    }

    private void HandleTorque()
    {
        currentSpeed = 0f;

        foreach (WheelCollider wheel in wheels)
        {
            currentSpeed += wheel.radius * 2 * Mathf.PI * wheel.rpm / 60f; // ������ ȸ�� �ӵ��� �̿��� ���� �ӵ� ���
        }

        // ���� �ӵ��� ��ǥ �ӵ����� �۴ٸ�
        if (currentSpeed < targetSpeed)
        {
            float motor = acceleration * rb.mass; // ���ӿ� ���� ���� ��ũ ���

            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = motor; // ��� ������ ���� ��ũ ����
            }
        }
        // ���� �ӵ��� ��ǥ �ӵ��� �����ߴٸ�
        else
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0f; // ��� ������ ���� ��ũ�� 0���� �����Ͽ� ����
            }
        }
    }

    private void HandleSteer()
    {
        float radius = 6f;
        if (horizontalInput > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
        }
        else if(horizontalInput < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        }
        else
        {
            wheels[0].steerAngle = 0; // ù ��° ������ �ڵ� ���� ����
            wheels[1].steerAngle = 0; // �� ��° ������ �ڵ� ���� ����
        }
    }

    private void AdjustSteering(float change)
    {
        foreach (WheelCollider wheel in wheels)
        {
            wheel.steerAngle += change; // Adjust the steering angle for each wheel
        }
    }

    private void ResetSteering()
    {
        foreach (WheelCollider wheel in wheels)
        {
            wheel.steerAngle = 0f; // Reset the steering angle for each wheel
        }
    }

    private void ChangeLane()
    {
        isTurn = false; // Reset lane change flag

        // Determine the current lane based on the current position
        if (rb.position.x < (lanePositions[0] + lanePositions[1]) / 2)
        {
            currentLane = 1; // Current lane is lane 1
        }
        else if (rb.position.x < (lanePositions[1] + lanePositions[2]) / 2)
        {
            currentLane = 2; // Current lane is lane 2
        }
        else
        {
            currentLane = 3; // Current lane is lane 3
        }

        // Request right lane change if not already changing lanes and not in the rightmost lane
        if (!isChangingLane && currentLane != 3)
        {
            isChangingLane = true; // Start right lane change
            
        }
    }

    private void UpdateWheelVisuals()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            WheelCollider wheel = wheels[i];
            Transform tireTransform = tires[i];

            Vector3 pos;
            Quaternion rot;
            wheel.GetWorldPose(out pos, out rot);

            tireTransform.position = pos;
            tireTransform.rotation = rot;
        }
    }

    // Add a method to trigger lane change externally
    public void RequestLaneChange()
    {
        isTurn = true;
    }
}