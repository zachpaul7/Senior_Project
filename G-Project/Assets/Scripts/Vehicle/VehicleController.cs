using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    // �ٸ� ��ũ��Ʈ(TargetMovement)���� ���� ������
    public bool Left_warning = false; // ���� ��� �÷���
    public bool Right_warning = false; // ������ ��� �÷���
    public float forward_distance = 0f; // ���� ��ֹ������� �Ÿ�
    public float forward_velocity = 0f; // ���� ��ֹ��� �ӵ�

    // �پ��� �������� ����ĳ��Ʈ�� �� ����� ���
    private float range_f = 30f; // ���� ������ �ִ� �Ÿ�
    private float range_side = (7f + 3.5f); // ���� ������ �Ÿ�
    private float range_diag = (7f + 3.5f) * Mathf.Sqrt(2); // �밢�� ������ �Ÿ�

    // ����ĳ��Ʈ �� ���� ����
    Ray shootRay = new Ray(); // ���̸� �߻��� �� ����� ���� ��ü
    RaycastHit shootHit_forward; // ���� ���̰� �ε��� ��� ����

    // �پ��� ����ĳ��Ʈ ������ ��Ÿ���� ���͵�
    private Vector3 forward_direction = new Vector3(0, 0, 1);
    private Vector3 left_direction = new Vector3(-1, 0, 0);
    private Vector3 right_direction = new Vector3(1, 0, 0);

    private Vector3 LF_direction = new Vector3(-1, 0, 1); // ���� ����
    private Vector3 LF_direction2 = new Vector3(-1, 0, 2); // ���� ����2

    private Vector3 LR_direction = new Vector3(-1, 0, -1); // ���� �Ĺ�
    private Vector3 LR_direction2 = new Vector3(-1, 0, -2); // ���� �Ĺ�2

    private Vector3 RF_direction = new Vector3(1, 0, 1); // ���� ����
    private Vector3 RF_direction2 = new Vector3(1, 0, 2); // ���� ����2

    private Vector3 RR_direction = new Vector3(1, 0, -1); // ���� �Ĺ�
    private Vector3 RR_direction2 = new Vector3(1, 0, -2); // ���� �Ĺ�2

    // �Ÿ� ���� �ʱ�ȭ
    private float forward_dist = 0f;

    void FixedUpdate()
    {
        // FixedUpdate���� Shoot �޼��� ȣ��
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
        // ������ �߻� ������ ���� ���� ������Ʈ�� ��ġ�� ����
        shootRay.origin = transform.position;

        // ����ĳ��Ʈ�� ���� �߰� ���� ����
        Vector3 origin2 = shootRay.origin + new Vector3(0, 0.5f, 3);
        Vector3 origin3 = shootRay.origin + new Vector3(0, 0.5f, -3);
        Vector3 frontPoint = shootRay.origin + new Vector3(0, 0.5f, 4);
        Vector3 frontLeftPoint = shootRay.origin + new Vector3(-1.5f, 0.5f, 4);
        Vector3 frontRightPoint = shootRay.origin + new Vector3(1.5f, 0.5f, 4);

        // �Ÿ� ���� �ʱ�ȭ
        forward_dist = 0f;

        // ����ĳ��Ʈ �ʱ�ȭ
        Left_warning = false; // ���� ��� �ʱ�ȭ
        Right_warning = false; // ���� ��� �ʱ�ȭ

        forward_velocity = 0.8f; // ���� �ӵ� �ʱ�ȭ

        GameObject hostAgent = GameObject.FindGameObjectWithTag("agent");
        //float hostAgentSpeed = hostAgent.GetComponent<VehicleAgent>().vehicle_speed;
        float hostAgentSpeed = 0f;

        // ���� ���� ����
        if (Physics.Raycast(frontLeftPoint, forward_direction, out shootHit_forward, range_f))
        {
            ProcessRaycastHit(hostAgent, hostAgentSpeed);
        }

        // ���� ���� ����
        if (Physics.Raycast(frontRightPoint, forward_direction, out shootHit_forward, range_f))
        {
            ProcessRaycastHit(hostAgent, hostAgentSpeed);
        }

        // ���� �߾� ����
        if (Physics.Raycast(frontPoint, forward_direction, out shootHit_forward, range_f))
        {
            ProcessRaycastHit(hostAgent, hostAgentSpeed);
        }

        // ���� ����
        ProcessSideRay(shootRay.origin, left_direction);

        // ���� ����
        ProcessSideRay(shootRay.origin, right_direction);

        // ���� ���� 2
        ProcessSideRay(origin2, left_direction);

        // ���� ���� 2
        ProcessSideRay(origin2, right_direction);

        // ���� ���� 3
        ProcessSideRay(origin3, left_direction);

        // ���� ���� 3
        ProcessSideRay(origin3, right_direction);

        // ���� ���� ����
        ProcessDiagonalRay(origin2, LF_direction);

        // ���� ���� ���� 2
        ProcessDiagonalRay(origin2, LF_direction2);

        // ���� �Ĺ� ����
        ProcessDiagonalRay(origin3, LR_direction);

        // ���� �Ĺ� ���� 2
        ProcessDiagonalRay(origin3, LR_direction2);

        // ���� ���� ����
        ProcessDiagonalRay(origin2, RF_direction);

        // ���� ���� ���� 2
        ProcessDiagonalRay(origin2, RF_direction2);

        // ���� �Ĺ� ����
        ProcessDiagonalRay(origin3, RR_direction);

        // ���� �Ĺ� ���� 2
        ProcessDiagonalRay(origin3, RR_direction2);

        // ���� �Ÿ��� ���� �Ÿ� ����
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