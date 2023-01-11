using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

// RollerAgent
public class RollerAgent : Agent
{
    Rigidbody rBody; // RollerAgentのRigidBody
    public TargetAgent target_obj;
    private float old_distanceToTarget;

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
        this.transform.localPosition = new Vector3(Random.value * 2.0f + -5.0f, 0.5f, Random.value * 12.0f - 6.0f);
        this.old_distanceToTarget = Vector3.Distance(this.transform.localPosition, target_obj.transform.position);
    }

    // 状態取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target_obj.transform.position.x); //TargetのX座標
        sensor.AddObservation(target_obj.transform.position.z); //TargetのZ座標
        sensor.AddObservation(this.transform.position.x); //RollerAgentのX座標
        sensor.AddObservation(this.transform.position.z); //RollerAgentのZ座標
        sensor.AddObservation(rBody.velocity.x); // RollerAgentのX速度
        sensor.AddObservation(rBody.velocity.z); // RollerAgentのZ速度
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // RollerAgentに力を加える
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * 10);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target_obj.transform.position);

        // RollerAgentがTargetの位置にたどりついた時
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            target_obj.AddReward(-1.0f);
            target_obj.EndEpisode();
            EndEpisode();
        }

        // SubReward
        if (distanceToTarget >= old_distanceToTarget) {
            AddReward(-0.01f);
            target_obj.AddReward(0.01f);
        }

        old_distanceToTarget = distanceToTarget;
    }

    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}