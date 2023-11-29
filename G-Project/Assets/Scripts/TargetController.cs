using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] wheels = new WheelCollider[4]; // 차량의 4개 바퀴 콜라이더 배열
    [SerializeField] private Transform[] tires = new Transform[4]; // 차량의 4개 바퀴 모양을 나타내는 Transform 배열
    [SerializeField] private float targetSpeed = 5.0f; // 목표 속도
    [SerializeField] private float acceleration = 2.0f; // 가속도
    [SerializeField] private float steeringAngle = 30f; // 핸들 조작에 따른 바퀴 회전 각도
    [SerializeField] private float currentSpeed; // 현재 속도
    [SerializeField] private float horizontalInput; // 수평 입력 값
    

    private Rigidbody rb; // Rigidbody 컴포넌트

    // 차선 변경 관련 변수
    [SerializeField] private bool isTurn = false; // 차선 변경 여부 플래그
    [SerializeField] private float[] lanePositions = { 0f, 10f, 20f }; // 차선 위치 배열
    private bool Is_right_lane_change = false;  // 우측 차선 변경 여부 초기화
    private bool Is_left_lane_change = false;   // 좌측 차선 변경 여부 초기화
    private float start_y = 0;  // 차선 변경 시작 위치 초기화
    private int currentLane = 1; // 현재 차선

    private bool isChangingLane = false; // 차선 변경 중인지 여부를 나타내는 플래그
    private float laneChangeDistance = 10f; // 차선 변경을 위한 이동 거리
    private float laneChangeSpeed = 5f; // 차선 변경 속도

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // 자신의 Rigidbody 컴포넌트 얻기
    }

    private void FixedUpdate()
    {
        HandleMove(); // 움직임 처리
        UpdateWheelVisuals(); // 바퀴의 시각적인 부분 업데이트
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
            currentSpeed += wheel.radius * 2 * Mathf.PI * wheel.rpm / 60f; // 바퀴의 회전 속도를 이용한 현재 속도 계산
        }

        // 현재 속도가 목표 속도보다 작다면
        if (currentSpeed < targetSpeed)
        {
            float motor = acceleration * rb.mass; // 가속에 따른 모터 토크 계산

            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = motor; // 모든 바퀴에 모터 토크 적용
            }
        }
        // 현재 속도가 목표 속도에 도달했다면
        else
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0f; // 모든 바퀴에 모터 토크를 0으로 설정하여 유지
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
            wheels[0].steerAngle = 0; // 첫 번째 바퀴에 핸들 각도 적용
            wheels[1].steerAngle = 0; // 두 번째 바퀴에 핸들 각도 적용
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