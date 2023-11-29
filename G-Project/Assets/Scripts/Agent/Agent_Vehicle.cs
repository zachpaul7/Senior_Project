using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Agent_Vehicle : Agent
{
    // 자동차의 네 바퀴와 타이어 정보를 담는 배열
    [SerializeField] private WheelCollider[] wheels = new WheelCollider[4];
    [SerializeField] private Transform[] tires = new Transform[4];

    // 초기 위치와 회전 정보를 저장하는 변수
    private Vector3 start_position = new Vector3();
    private Quaternion start_rotation = new Quaternion();

    // 자동차 제어에 사용되는 변수들
    [SerializeField] private float maxpower = 5f;
    [SerializeField] private float power = 1250f;
    [SerializeField] private float rot = 17.5f;

    [SerializeField] private int Speed_Input;
    private int car_speed;

    Rigidbody rb;
    private float pre_angle = 0;

    // 바퀴의 위치와 회전 정보 저장 변수
    private Vector3 wheel_position;
    private Quaternion wheel_rotation;

    void  Awake()
    {
        start_position = new Vector3(0, 0, 0);
        start_rotation = Quaternion.Euler(0, 0, 0);

        rb = this.GetComponent<Rigidbody>(); // Rigidbody 컴포넌트를 가져와 rb 변수에 할당
        rb.centerOfMass = new Vector3(0, 0, 0);
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = start_position;
        transform.localRotation = start_rotation;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        car_speed = Speed_Input;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var car_angle = Mathf.Floor(actions.ContinuousActions[0] * 10) / 10;

        if (rb.velocity.magnitude >= maxpower / 5)
        {
            if (rb.velocity.magnitude >= maxpower)
            {
                rb.velocity = rb.velocity.normalized * maxpower;
            }
        }

        for (int i = 2; i < 4; i++)
        {
            wheels[i].motorTorque = car_speed * power;

            wheels[i].GetWorldPose(out wheel_position, out wheel_rotation);
            tires[i].rotation = wheel_rotation;
        }

        for (int i = 0; i < 2; i++)
        {
            wheels[i].steerAngle = car_angle * rot;
            wheels[i].GetWorldPose(out wheel_position, out wheel_rotation);
            tires[i].rotation = wheel_rotation;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var angle_action = actionsOut.ContinuousActions;
        angle_action[0] = Input.GetAxis("Horizontal");
    }
}
