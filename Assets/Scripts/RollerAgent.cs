using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Areas;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    public Transform target;
    private Rigidbody rBody;

    private void Start()
    {
        this.rBody = this.GetComponent<Rigidbody>();
    }

    //1
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }

    //2 매프레임마다 호출됨 
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        Debug.Log("CollectObservations");

        sensor.AddObservation(this.target.localPosition);   //3
        sensor.AddObservation(this.transform.localPosition);    //3

        sensor.AddObservation(this.rBody.velocity.x);   //1
        sensor.AddObservation(this.rBody.velocity.z);   //1
        
    }

    public float speed = 10;
    //4 매프레임마다 호출됨 
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        //Debug.Log("OnActionReceived");

        //Vector3 dir = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        
        Debug.LogFormat("OnActionReceived : {0}, {1}", actions.ContinuousActions[0], actions.ContinuousActions[1]);

        //float force = 10;
        //this.rBody.AddForce(dir.normalized * speed);
        //this.rBody.MovePosition(this.transform.position + (dir.normalized * this.speed * Time.deltaTime));
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rBody.AddForce(controlSignal * this.speed);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        //Debug.Log(distanceToTarget);
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    //private void Update()
    //{
    //    Debug.DrawLine(this.transform.position, this.target.position, Color.red);
    //}

    //3 매프레임마다 호출됨 
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);

        ActionSegment<float> continuousActionOut = actionsOut.ContinuousActions;
        continuousActionOut[0] = Input.GetAxis("Horizontal");
        continuousActionOut[1] = Input.GetAxis("Vertical");
        //Debug.LogFormat("Heuristic: {0:F2}, {1:F2}", continuousActionOut[0], continuousActionOut[1]);
    }

}
