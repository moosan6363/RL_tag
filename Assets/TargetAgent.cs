using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class TargetAgent : Agent
{
    Rigidbody rBody;
    public RollerAgent roller_obj;
    private Vector3 stacked_roller_position;
    float summation_magnitude = 0f;

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
        this.summation_magnitude = 0f;
    }

    // 状態取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(roller_obj.transform.localPosition.x);
        sensor.AddObservation(roller_obj.transform.localPosition.z);
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
        Vector3 force = Vector3.ClampMagnitude(controlSignal, 1.0f) * 30f;
        rBody.AddForce(force);

        // 出力が大きいほど罰を与える
        AddReward(-force.magnitude * 2e-5f);

        summation_magnitude += force.magnitude;

        if (this.StepCount == this.MaxStep) {
            setMagnitudeStat();
        }
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
            setMagnitudeStat();
            EndEpisode();
        }
        // 壁に激突したら罰を与える
        else if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.3f);
        }
    }

    void setMagnitudeStat() {
        Academy.Instance.StatsRecorder.Add("TargetForceMagnitude", summation_magnitude  / this.StepCount);
    }
}
