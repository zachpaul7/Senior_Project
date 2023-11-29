using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    int random_val = 3;  // 랜덤한 값을 저장하는 변수 초기화

    // 이동 속도 및 기타 변수들
    public float m_Speed = 0.5f;  // 이동 속도를 나타내는 변수 초기화
    private float lateral_speed;  // 차량의 측면 속도를 나타내는 변수 초기화

    // 경과 시간과 대기 시간 변수
    public float wait_time_lon = 3f;  // 종횡 방향의 대기 시간 초기화
    public float wait_time_lat = 7f;  // 좌우 방향의 대기 시간 초기화

    private Rigidbody m_Rigidbody;  // Rigidbody를 담는 변수

    // 종횡 운동 변수
    private float max_speed = 0.8f;  // 최대 종횡 속도 초기화
    private float min_speed = 0.4f;  // 최소 종횡 속도 초기화

    // 차선 변경 관련 변수
    private bool Is_right_lane_change = false;  // 우측 차선 변경 여부 초기화
    private bool Is_left_lane_change = false;   // 좌측 차선 변경 여부 초기화
    private float start_y = 0;  // 차선 변경 시작 위치 초기화

    private float sum_time_lon = 0f;  // 경과된 종횡 대기 시간의 합 초기화
    private float sum_time_lat = 0f;  // 경과된 좌우 대기 시간의 합 초기화

    private float mode_lon = 0;  // 종횡 운동 모드 초기화
    private int mode_lat = 0;   // 좌우 운동 모드 초기화

    // 차선 좌표
    private float lane1_x = -20;  // 1번 차선의 x 좌표 초기화
    private float lane2_x = -10;  // 2번 차선의 x 좌표 초기화
    private float lane3_x = 0;    // 3번 차선의 x 좌표 초기화
    private float lane4_x = 10;   // 4번 차선의 x 좌표 초기화
    private float lane5_x = 20;   // 5번 차선의 x 좌표 초기화

    private int current_lane = 3;  // 현재 차선 초기화

    // 0: 아무것도 안 함
    // Mode_lon => 1: 가속, 2: 감속
    // Mode_lat => 1: 우측 차선 변경, 2: 좌측 차선 변경

    // 초기화 함수
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();  // 시작할 때 Rigidbody 정의
        m_Speed = Random.Range(0.4f, 0.6f);  // 초기 속도 설정
    }

    // 매 프레임마다 호출되는 함수
    private void FixedUpdate()
    {
        lateral_speed = m_Speed * 0.25f;  // 측면 속도 계산

        // 랜덤한 슬라이더 값을 가져와 대기 시간을 조절
        float random_slider_val = random_val;
        wait_time_lon = 8f - random_slider_val;
        wait_time_lat = 8f - random_slider_val;

        sum_time_lon += Time.deltaTime;  // 경과 시간 누적
        sum_time_lat += Time.deltaTime;  // 경과 시간 누적

        // 주어진 대기 시간을 초과하면 종횡 운동 및 좌우 이동 모드를 랜덤으로 설정하는 부분
        if (sum_time_lon > wait_time_lon)
        {
            mode_lon = Random.Range(0, 1f);  // 0부터 1 사이의 랜덤 값을 mode_lon에 할당
            sum_time_lon = 0;  // 경과 시간 초기화
        }

        if (sum_time_lat > wait_time_lat)
        {
            mode_lat = Random.Range(0, 3);  // 0부터 2 사이의 랜덤 값을 mode_lat에 할당
            sum_time_lat = 0;  // 경과 시간 초기화
        }

        bool Left_warning = false;
        bool Right_warning = false;
        float forward_dist = 0f;
        float forward_vel = 0f;
        bool Forward_warning = false;

        // 다른 스크립트에서 전방, 좌우 경고 및 거리, 속도 정보 가져오기
        Left_warning = this.gameObject.GetComponent<VehicleController>().Left_warning;
        Right_warning = this.gameObject.GetComponent<VehicleController>().Right_warning;
        forward_dist = this.gameObject.GetComponent<VehicleController>().forward_distance;
        forward_vel = this.gameObject.GetComponent<VehicleController>().forward_velocity;

        // 전방 거리 제어에 사용되는 변수 초기화
        float forward_threshold = 5f + (m_Speed / 3f);  // 전방 거리 제어에 사용되는 임계값 초기화
        float k_p = 0.001f;  // 비례 제어 상수 초기화
        float k_i = 0.001f;  // 적분 제어 상수 초기화

        // AEB와 유사한 동작을 위해 전방 거리가 일정 값 이하일 때 제어 상수를 조정
        if (forward_dist < 6f)
        {
            k_p = 0.5f;
        }

        float accel_prob = 0.75f;  // 가속 확률 초기화

        // 전방에 차량이 있으면 전방 경고를 활성화
        if ((forward_dist < forward_threshold) && (forward_dist != 0))
        {
            Forward_warning = true;
        }

        // 전방 경고가 없을 때
        if (Forward_warning == false)
        {
            // 가속 모드이고 현재 속도가 최대 속도 미만일 때
            if (mode_lon < accel_prob && mode_lon > 0 && m_Speed < max_speed)
            {
                m_Speed += 0.05f;  // 속도 증가
                mode_lon = 0;  // 모드 초기화
            }

            // 감속 모드이고 현재 속도가 최소 속도 초과일 때
            if (mode_lon > accel_prob && m_Speed > min_speed)
            {
                m_Speed -= 0.05f;  // 속도 감소
                mode_lon = 0;  // 모드 초기화
            }
        }
        else
        {
            // 전방 경고가 있을 때, 안전거리를 유지하기 위해 속도 조절 (AEB와 유사한 동작)
            m_Speed -= (k_p * (m_Speed - forward_vel) + k_i * (m_Speed - forward_vel) / Time.deltaTime);
            mode_lon = 0;  // 모드 초기화
        }

        VerticalMove();  // 종횡 운동 함수 호출

        // 속도가 최대 속도를 초과하지 않도록 최대 속도 설정
        if (m_Speed > max_speed)
        {
            m_Speed = max_speed;
        }

        // 속도가 최소 속도 미만이 되지 않도록 최소 속도 설정
        if (m_Speed < min_speed)
        {
            m_Speed = min_speed;
        }

        // 현재 차량의 x 좌표를 기준으로 현재 차선 결정
        if (m_Rigidbody.position.x < (lane1_x + lane2_x) / 2)
        {
            current_lane = 1;  // 현재 차량이 차선 1에 위치
        }
        else if (m_Rigidbody.position.x < (lane2_x + lane3_x) / 2)
        {
            current_lane = 2;  // 현재 차량이 차선 2에 위치
        }
        else if (m_Rigidbody.position.x < (lane3_x + lane4_x) / 2)
        {
            current_lane = 3;  // 현재 차량이 차선 3에 위치
        }
        else if (m_Rigidbody.position.x < (lane4_x + lane5_x) / 2)
        {
            current_lane = 4;  // 현재 차량이 차선 4에 위치
        }
        else
        {
            current_lane = 5;  // 현재 차량이 차선 5에 위치
        }

        // 우측 차선 변경
        if (mode_lat == 1 && Is_right_lane_change == false && current_lane != 5 && Right_warning == false)
        {
            Is_right_lane_change = true;  // 우측 차선 변경 시작
            start_y = m_Rigidbody.position.x;  // 우측 차선 변경 시작 위치 기록
        }

        if (Is_right_lane_change == true)
        {
            // 우측 차선 변경 중인 경우 측면 속도에 따라 우측으로 이동
            Vector3 movement = transform.right * lateral_speed;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

            // 우측으로 이동한 거리가 7 이상이면 우측 차선 변경 종료
            if (Abs(m_Rigidbody.position.x - start_y) >= 7)
            {
                Is_right_lane_change = false;  // 우측 차선 변경 종료
                mode_lat = 0;  // 차선 변경 모드 초기화
            }
        }


        // 좌측 차선 변경
        if (mode_lat == 2 && Is_left_lane_change == false && current_lane != 1 && Left_warning == false)
        {
            Is_left_lane_change = true;  // 좌측 차선 변경 시작
            start_y = m_Rigidbody.position.x;  // 좌측 차선 변경 시작 위치 기록
        }

        if (Is_left_lane_change == true)
        {
            // 좌측 차선 변경 중인 경우 측면 속도에 따라 좌측으로 이동
            Vector3 movement = transform.right * lateral_speed;
            m_Rigidbody.MovePosition(m_Rigidbody.position - movement);

            // 좌측으로 이동한 거리가 7 이상이면 좌측 차선 변경 종료
            if (Abs(start_y - m_Rigidbody.position.x) >= 7)
            {
                Is_left_lane_change = false;  // 좌측 차선 변경 종료
                mode_lat = 0;  // 차선 변경 모드 초기화
            }
        }

        // 차선 변경 중이 아니라면 중앙 차선으로 이동
        float current_lane_x = 0;
        if (Is_left_lane_change == false && Is_right_lane_change == false)
        {
            // 현재 차량의 현재 차선에 해당하는 x 좌표 설정
            switch (current_lane)
            {
                case 1:
                    current_lane_x = lane1_x;
                    break;
                case 2:
                    current_lane_x = lane2_x;
                    break;
                case 3:
                    current_lane_x = lane3_x;
                    break;
                case 4:
                    current_lane_x = lane4_x;
                    break;
                case 5:
                    current_lane_x = lane5_x;
                    break;
            }

            // 차량의 위치를 중앙 차선으로 이동
            Vector3 pos_vehicle = new Vector3(current_lane_x, m_Rigidbody.position.y, m_Rigidbody.position.z);
            this.transform.position = pos_vehicle;
        }
    }

    // 종횡 운동 함수: 현재 속도와 방향에 따라 차량을 전진 또는 후진시키는 함수
    private void VerticalMove()
    {
        // 현재 속도와 전방 방향을 기반으로 이동 벡터 계산
        Vector3 movement = transform.forward * m_Speed;

        // Rigidbody를 사용하여 차량의 위치를 새로운 위치로 이동
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    // 절댓값을 반환하는 함수
    // 입력값(val)이 음수일 경우에는 그대로 반환하고, 양수일 경우에는 음수로 변환하여 반환합니다.
    private float Abs(float val)
    {
        if (val < 0)
        {
            return -val;  // 입력값이 음수이면 그대로 반환
        }
        else
        {
            return val;   // 입력값이 양수이거나 0이면 그대로 반환
        }
    }

}