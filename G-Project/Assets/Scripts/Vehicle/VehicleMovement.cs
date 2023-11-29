using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    int random_val = 3;  // ������ ���� �����ϴ� ���� �ʱ�ȭ

    // �̵� �ӵ� �� ��Ÿ ������
    public float m_Speed = 0.5f;  // �̵� �ӵ��� ��Ÿ���� ���� �ʱ�ȭ
    private float lateral_speed;  // ������ ���� �ӵ��� ��Ÿ���� ���� �ʱ�ȭ

    // ��� �ð��� ��� �ð� ����
    public float wait_time_lon = 3f;  // ��Ⱦ ������ ��� �ð� �ʱ�ȭ
    public float wait_time_lat = 7f;  // �¿� ������ ��� �ð� �ʱ�ȭ

    private Rigidbody m_Rigidbody;  // Rigidbody�� ��� ����

    // ��Ⱦ � ����
    private float max_speed = 0.8f;  // �ִ� ��Ⱦ �ӵ� �ʱ�ȭ
    private float min_speed = 0.4f;  // �ּ� ��Ⱦ �ӵ� �ʱ�ȭ

    // ���� ���� ���� ����
    private bool Is_right_lane_change = false;  // ���� ���� ���� ���� �ʱ�ȭ
    private bool Is_left_lane_change = false;   // ���� ���� ���� ���� �ʱ�ȭ
    private float start_y = 0;  // ���� ���� ���� ��ġ �ʱ�ȭ

    private float sum_time_lon = 0f;  // ����� ��Ⱦ ��� �ð��� �� �ʱ�ȭ
    private float sum_time_lat = 0f;  // ����� �¿� ��� �ð��� �� �ʱ�ȭ

    private float mode_lon = 0;  // ��Ⱦ � ��� �ʱ�ȭ
    private int mode_lat = 0;   // �¿� � ��� �ʱ�ȭ

    // ���� ��ǥ
    private float lane1_x = -20;  // 1�� ������ x ��ǥ �ʱ�ȭ
    private float lane2_x = -10;  // 2�� ������ x ��ǥ �ʱ�ȭ
    private float lane3_x = 0;    // 3�� ������ x ��ǥ �ʱ�ȭ
    private float lane4_x = 10;   // 4�� ������ x ��ǥ �ʱ�ȭ
    private float lane5_x = 20;   // 5�� ������ x ��ǥ �ʱ�ȭ

    private int current_lane = 3;  // ���� ���� �ʱ�ȭ

    // 0: �ƹ��͵� �� ��
    // Mode_lon => 1: ����, 2: ����
    // Mode_lat => 1: ���� ���� ����, 2: ���� ���� ����

    // �ʱ�ȭ �Լ�
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();  // ������ �� Rigidbody ����
        m_Speed = Random.Range(0.4f, 0.6f);  // �ʱ� �ӵ� ����
    }

    // �� �����Ӹ��� ȣ��Ǵ� �Լ�
    private void FixedUpdate()
    {
        lateral_speed = m_Speed * 0.25f;  // ���� �ӵ� ���

        // ������ �����̴� ���� ������ ��� �ð��� ����
        float random_slider_val = random_val;
        wait_time_lon = 8f - random_slider_val;
        wait_time_lat = 8f - random_slider_val;

        sum_time_lon += Time.deltaTime;  // ��� �ð� ����
        sum_time_lat += Time.deltaTime;  // ��� �ð� ����

        // �־��� ��� �ð��� �ʰ��ϸ� ��Ⱦ � �� �¿� �̵� ��带 �������� �����ϴ� �κ�
        if (sum_time_lon > wait_time_lon)
        {
            mode_lon = Random.Range(0, 1f);  // 0���� 1 ������ ���� ���� mode_lon�� �Ҵ�
            sum_time_lon = 0;  // ��� �ð� �ʱ�ȭ
        }

        if (sum_time_lat > wait_time_lat)
        {
            mode_lat = Random.Range(0, 3);  // 0���� 2 ������ ���� ���� mode_lat�� �Ҵ�
            sum_time_lat = 0;  // ��� �ð� �ʱ�ȭ
        }

        bool Left_warning = false;
        bool Right_warning = false;
        float forward_dist = 0f;
        float forward_vel = 0f;
        bool Forward_warning = false;

        // �ٸ� ��ũ��Ʈ���� ����, �¿� ��� �� �Ÿ�, �ӵ� ���� ��������
        Left_warning = this.gameObject.GetComponent<VehicleController>().Left_warning;
        Right_warning = this.gameObject.GetComponent<VehicleController>().Right_warning;
        forward_dist = this.gameObject.GetComponent<VehicleController>().forward_distance;
        forward_vel = this.gameObject.GetComponent<VehicleController>().forward_velocity;

        // ���� �Ÿ� ��� ���Ǵ� ���� �ʱ�ȭ
        float forward_threshold = 5f + (m_Speed / 3f);  // ���� �Ÿ� ��� ���Ǵ� �Ӱ谪 �ʱ�ȭ
        float k_p = 0.001f;  // ��� ���� ��� �ʱ�ȭ
        float k_i = 0.001f;  // ���� ���� ��� �ʱ�ȭ

        // AEB�� ������ ������ ���� ���� �Ÿ��� ���� �� ������ �� ���� ����� ����
        if (forward_dist < 6f)
        {
            k_p = 0.5f;
        }

        float accel_prob = 0.75f;  // ���� Ȯ�� �ʱ�ȭ

        // ���濡 ������ ������ ���� ��� Ȱ��ȭ
        if ((forward_dist < forward_threshold) && (forward_dist != 0))
        {
            Forward_warning = true;
        }

        // ���� ��� ���� ��
        if (Forward_warning == false)
        {
            // ���� ����̰� ���� �ӵ��� �ִ� �ӵ� �̸��� ��
            if (mode_lon < accel_prob && mode_lon > 0 && m_Speed < max_speed)
            {
                m_Speed += 0.05f;  // �ӵ� ����
                mode_lon = 0;  // ��� �ʱ�ȭ
            }

            // ���� ����̰� ���� �ӵ��� �ּ� �ӵ� �ʰ��� ��
            if (mode_lon > accel_prob && m_Speed > min_speed)
            {
                m_Speed -= 0.05f;  // �ӵ� ����
                mode_lon = 0;  // ��� �ʱ�ȭ
            }
        }
        else
        {
            // ���� ��� ���� ��, �����Ÿ��� �����ϱ� ���� �ӵ� ���� (AEB�� ������ ����)
            m_Speed -= (k_p * (m_Speed - forward_vel) + k_i * (m_Speed - forward_vel) / Time.deltaTime);
            mode_lon = 0;  // ��� �ʱ�ȭ
        }

        VerticalMove();  // ��Ⱦ � �Լ� ȣ��

        // �ӵ��� �ִ� �ӵ��� �ʰ����� �ʵ��� �ִ� �ӵ� ����
        if (m_Speed > max_speed)
        {
            m_Speed = max_speed;
        }

        // �ӵ��� �ּ� �ӵ� �̸��� ���� �ʵ��� �ּ� �ӵ� ����
        if (m_Speed < min_speed)
        {
            m_Speed = min_speed;
        }

        // ���� ������ x ��ǥ�� �������� ���� ���� ����
        if (m_Rigidbody.position.x < (lane1_x + lane2_x) / 2)
        {
            current_lane = 1;  // ���� ������ ���� 1�� ��ġ
        }
        else if (m_Rigidbody.position.x < (lane2_x + lane3_x) / 2)
        {
            current_lane = 2;  // ���� ������ ���� 2�� ��ġ
        }
        else if (m_Rigidbody.position.x < (lane3_x + lane4_x) / 2)
        {
            current_lane = 3;  // ���� ������ ���� 3�� ��ġ
        }
        else if (m_Rigidbody.position.x < (lane4_x + lane5_x) / 2)
        {
            current_lane = 4;  // ���� ������ ���� 4�� ��ġ
        }
        else
        {
            current_lane = 5;  // ���� ������ ���� 5�� ��ġ
        }

        // ���� ���� ����
        if (mode_lat == 1 && Is_right_lane_change == false && current_lane != 5 && Right_warning == false)
        {
            Is_right_lane_change = true;  // ���� ���� ���� ����
            start_y = m_Rigidbody.position.x;  // ���� ���� ���� ���� ��ġ ���
        }

        if (Is_right_lane_change == true)
        {
            // ���� ���� ���� ���� ��� ���� �ӵ��� ���� �������� �̵�
            Vector3 movement = transform.right * lateral_speed;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

            // �������� �̵��� �Ÿ��� 7 �̻��̸� ���� ���� ���� ����
            if (Abs(m_Rigidbody.position.x - start_y) >= 7)
            {
                Is_right_lane_change = false;  // ���� ���� ���� ����
                mode_lat = 0;  // ���� ���� ��� �ʱ�ȭ
            }
        }


        // ���� ���� ����
        if (mode_lat == 2 && Is_left_lane_change == false && current_lane != 1 && Left_warning == false)
        {
            Is_left_lane_change = true;  // ���� ���� ���� ����
            start_y = m_Rigidbody.position.x;  // ���� ���� ���� ���� ��ġ ���
        }

        if (Is_left_lane_change == true)
        {
            // ���� ���� ���� ���� ��� ���� �ӵ��� ���� �������� �̵�
            Vector3 movement = transform.right * lateral_speed;
            m_Rigidbody.MovePosition(m_Rigidbody.position - movement);

            // �������� �̵��� �Ÿ��� 7 �̻��̸� ���� ���� ���� ����
            if (Abs(start_y - m_Rigidbody.position.x) >= 7)
            {
                Is_left_lane_change = false;  // ���� ���� ���� ����
                mode_lat = 0;  // ���� ���� ��� �ʱ�ȭ
            }
        }

        // ���� ���� ���� �ƴ϶�� �߾� �������� �̵�
        float current_lane_x = 0;
        if (Is_left_lane_change == false && Is_right_lane_change == false)
        {
            // ���� ������ ���� ������ �ش��ϴ� x ��ǥ ����
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

            // ������ ��ġ�� �߾� �������� �̵�
            Vector3 pos_vehicle = new Vector3(current_lane_x, m_Rigidbody.position.y, m_Rigidbody.position.z);
            this.transform.position = pos_vehicle;
        }
    }

    // ��Ⱦ � �Լ�: ���� �ӵ��� ���⿡ ���� ������ ���� �Ǵ� ������Ű�� �Լ�
    private void VerticalMove()
    {
        // ���� �ӵ��� ���� ������ ������� �̵� ���� ���
        Vector3 movement = transform.forward * m_Speed;

        // Rigidbody�� ����Ͽ� ������ ��ġ�� ���ο� ��ġ�� �̵�
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    // ������ ��ȯ�ϴ� �Լ�
    // �Է°�(val)�� ������ ��쿡�� �״�� ��ȯ�ϰ�, ����� ��쿡�� ������ ��ȯ�Ͽ� ��ȯ�մϴ�.
    private float Abs(float val)
    {
        if (val < 0)
        {
            return -val;  // �Է°��� �����̸� �״�� ��ȯ
        }
        else
        {
            return val;   // �Է°��� ����̰ų� 0�̸� �״�� ��ȯ
        }
    }

}