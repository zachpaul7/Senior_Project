using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    // 다른 스크립트(TargetMovement)에서 사용될 변수들
    public bool Left_warning = false; // 왼쪽 경고 플래그
    public bool Right_warning = false; // 오른쪽 경고 플래그
    public float forward_distance = 0f; // 앞쪽 장애물까지의 거리
    public float forward_velocity = 0f; // 앞쪽 장애물의 속도

    // 다양한 방향으로 레이캐스트할 때 사용할 상수
    private float range_f = 30f; // 전방 레이의 최대 거리
    private float range_side = (7f + 3.5f); // 측면 레이의 거리
    private float range_diag = (7f + 3.5f) * Mathf.Sqrt(2); // 대각선 레이의 거리

    // 레이캐스트 및 방향 변수
    Ray shootRay = new Ray(); // 레이를 발사할 때 사용할 레이 객체
    RaycastHit shootHit_forward; // 전방 레이가 부딪힌 대상 정보

    // 다양한 레이캐스트 방향을 나타내는 벡터들
    private Vector3 forward_direction = new Vector3(0, 0, 1);
    private Vector3 left_direction = new Vector3(-1, 0, 0);
    private Vector3 right_direction = new Vector3(1, 0, 0);

    private Vector3 LF_direction = new Vector3(-1, 0, 1); // 좌측 전방
    private Vector3 LF_direction2 = new Vector3(-1, 0, 2); // 좌측 전방2

    private Vector3 LR_direction = new Vector3(-1, 0, -1); // 좌측 후방
    private Vector3 LR_direction2 = new Vector3(-1, 0, -2); // 좌측 후방2

    private Vector3 RF_direction = new Vector3(1, 0, 1); // 우측 전방
    private Vector3 RF_direction2 = new Vector3(1, 0, 2); // 우측 전방2

    private Vector3 RR_direction = new Vector3(1, 0, -1); // 우측 후방
    private Vector3 RR_direction2 = new Vector3(1, 0, -2); // 우측 후방2

    // 거리 변수 초기화
    private float forward_dist = 0f;

    void FixedUpdate()
    {
        // FixedUpdate마다 Shoot 메서드 호출
        Shoot();
        VisualizeRays();
    }

    void VisualizeRays()
    {
        // Visualize forward rays
        Debug.DrawRay(transform.position, forward_direction * range_f, Color.blue);
        Debug.DrawRay(transform.position, left_direction * range_side, Color.red);
        Debug.DrawRay(transform.position, right_direction * range_side, Color.red);

        // Visualize additional points
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 3), forward_direction * range_f, Color.yellow);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, -3), forward_direction * range_f, Color.yellow);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 4), forward_direction * range_f, Color.green);
        //Debug.DrawRay(transform.position + new Vector3(-1.5f, 0.5f, 4), forward_direction * range_f, Color.green);
        //Debug.DrawRay(transform.position + new Vector3(1.5f, 0.5f, 4), forward_direction * range_f, Color.green);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 2), left_direction * range_side, Color.magenta);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, -2), left_direction * range_side, Color.magenta);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 3), right_direction * range_side, Color.magenta);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.5f, -3), right_direction * range_side, Color.magenta);
    }

    void Shoot()
    {
        // 레이의 발사 지점을 현재 게임 오브젝트의 위치로 설정
        shootRay.origin = transform.position;

        // 레이캐스트에 사용될 추가 지점 정의
        Vector3 origin2 = shootRay.origin + new Vector3(0, 0.5f, 3);
        Vector3 origin3 = shootRay.origin + new Vector3(0, 0.5f, -3);
        Vector3 frontPoint = shootRay.origin + new Vector3(0, 0.5f, 4);
        Vector3 frontLeftPoint = shootRay.origin + new Vector3(-1.5f, 0.5f, 4);
        Vector3 frontRightPoint = shootRay.origin + new Vector3(1.5f, 0.5f, 4);

        // 거리 변수 초기화
        forward_dist = 0f;

        // 레이캐스트 초기화
        Left_warning = false; // 좌측 경고 초기화
        Right_warning = false; // 우측 경고 초기화

        forward_velocity = 0.8f; // 전진 속도 초기화

        GameObject hostAgent = GameObject.FindGameObjectWithTag("agent");
        //float hostAgentSpeed = hostAgent.GetComponent<VehicleAgent>().vehicle_speed;
        float hostAgentSpeed = 0f;

        // 전방 좌측 레이
        if (Physics.Raycast(frontLeftPoint, forward_direction, out shootHit_forward, range_f))
        {
            ProcessRaycastHit(hostAgent, hostAgentSpeed);
        }

        // 전방 우측 레이
        if (Physics.Raycast(frontRightPoint, forward_direction, out shootHit_forward, range_f))
        {
            ProcessRaycastHit(hostAgent, hostAgentSpeed);
        }

        // 전방 중앙 레이
        if (Physics.Raycast(frontPoint, forward_direction, out shootHit_forward, range_f))
        {
            ProcessRaycastHit(hostAgent, hostAgentSpeed);
        }

        // 좌측 레이
        ProcessSideRay(shootRay.origin, left_direction);

        // 우측 레이
        ProcessSideRay(shootRay.origin, right_direction);

        // 좌측 레이 2
        ProcessSideRay(origin2, left_direction);

        // 우측 레이 2
        ProcessSideRay(origin2, right_direction);

        // 좌측 레이 3
        ProcessSideRay(origin3, left_direction);

        // 우측 레이 3
        ProcessSideRay(origin3, right_direction);

        // 좌측 전방 레이
        ProcessDiagonalRay(origin2, LF_direction);

        // 좌측 전방 레이 2
        ProcessDiagonalRay(origin2, LF_direction2);

        // 좌측 후방 레이
        ProcessDiagonalRay(origin3, LR_direction);

        // 좌측 후방 레이 2
        ProcessDiagonalRay(origin3, LR_direction2);

        // 우측 전방 레이
        ProcessDiagonalRay(origin2, RF_direction);

        // 우측 전방 레이 2
        ProcessDiagonalRay(origin2, RF_direction2);

        // 우측 후방 레이
        ProcessDiagonalRay(origin3, RR_direction);

        // 우측 후방 레이 2
        ProcessDiagonalRay(origin3, RR_direction2);

        // 앞쪽 거리에 현재 거리 저장
        forward_distance = forward_dist;
    }

    void ProcessRaycastHit(GameObject hostAgent, float hostAgentSpeed)
    {
        forward_dist = shootHit_forward.distance;

        if (shootHit_forward.rigidbody != null)
        {
            if (shootHit_forward.rigidbody.tag == "Player")
            {
                forward_velocity = hostAgentSpeed;
            }
            else if (shootHit_forward.rigidbody.GetComponent<VehicleMovement>() != null)
            {
                forward_velocity = shootHit_forward.rigidbody.GetComponent<VehicleMovement>().m_Speed;
            }
        }
    }

    void ProcessSideRay(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, range_side))
        {
            if (direction == left_direction)
            {
                Left_warning = true;
            }
            else if (direction == right_direction)
            {
                Right_warning = true;
            }
        }
    }

    void ProcessDiagonalRay(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, range_diag))
        {
            Left_warning = true;
        }
    }
}