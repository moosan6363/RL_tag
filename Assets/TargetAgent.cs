using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class TargetAgent : Agent
{
    [System.NonSerialized]
    public Rigidbody rBody; // RollerAgentのRigidBody
    public RollerAgent roller_obj;
    private Vector3 stacked_roller_position;

    // 初期化時に呼ばれる
    public override void Initialize()
    {
        // RollerAgentのRigidBodyの参照の取得
        this.rBody = GetComponent<Rigidbody>();
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(Random.value * 2.0f + 3.0f, 0.5f, Random.value * 12.0f - 6.0f);
        this.stacked_roller_position = roller_obj.transform.localPosition;
    }

    // 状態取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(roller_obj.transform.localPosition.x);
        sensor.AddObservation(roller_obj.transform.localPosition.z);
        // sensor.AddObservation(roller_obj.rBody.velocity.x);
        // sensor.AddObservation(roller_obj.rBody.velocity.z);
        sensor.AddObservation(stacked_roller_position.x);
        sensor.AddObservation(stacked_roller_position.z);
        sensor.AddObservation(this.transform.localPosition.x); 
        sensor.AddObservation(this.transform.localPosition.z);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        stacked_roller_position = roller_obj.transform.localPosition;
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // RollerAgentに力を加える
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(Vector3.ClampMagnitude(controlSignal, 1.0f) * 30);

        // // 速度が大きいほど報酬を与える
        // AddReward(rBody.velocity.sqrMagnitude * 1e-5f);

        // // 出力が大きいほど罰を与える
        // AddReward(-controlSignal.sqrMagnitude * 2e-4f);
    }
    
    public void add_reward(float reward) {
        AddReward(reward);
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

   　// 壁に激突したら罰を与える
    void OnCollisionEnter(Collision collision)
    {
        // RollerAgentに接触したら終了
        if (collision.gameObject.name == "RollerAgent") {
            AddReward(-1.0f);
            EndEpisode();
        }
        // 壁に激突したら罰を与える
        else if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.3f);
        }
    }
}
