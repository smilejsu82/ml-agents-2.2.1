using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    public float moveSpeed = 1f;
    public float rotSpeed = 5f;
    public enum eState
    {
        None = -1, Idle, Run
    }

    Animator anim;
    eState state = eState.Idle;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    public override void OnEpisodeBegin()
    {
        this.h = 0;
        this.v = 0;
        this.state = eState.Idle;
        this.transform.localPosition = Vector3.zero;

        // Move the target to a new spot
        target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Debug.LogFormat("target: {0}", target);
        //Debug.LogFormat("sensor: {0}", sensor); //null .... why?    vector  observation space size가 0이기 때문 

        sensor.AddObservation(this.target.localPosition);   //3
        sensor.AddObservation(this.transform.localPosition);    //3
        sensor.AddObservation(this.h);   //1
        sensor.AddObservation(this.v);   //1
    }


    private int h;
    private int v;
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.LogFormat("{0}, {1}", actions.DiscreteActions[0], actions.DiscreteActions[1]);
        this.h = actions.DiscreteActions[0] - 1;    //0, 1, 2   -> -1, 0, 1
        this.v = actions.DiscreteActions[1] - 1;    //0, 1, 2   -> -1, 0, 1 

        //Debug.LogFormat("h: {0} v: {1}", this.h, this.v);       


        Vector3 dir = new Vector3(h, 0, v);

        if (v == 0 && h == 0)
        {
            if (this.state != eState.Idle)
            {
                this.state = eState.Idle;
                this.anim.Play("StandA_idleA", -1, 0);
                this.anim.SetBool("IsRun", false);
            }
        }
        else
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            var toAngle = Vector3.up * angle;
            //this.transform.eulerAngles = toAngle;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(toAngle), Time.deltaTime * this.rotSpeed);

            if (this.state != eState.Run)
            {
                this.state = eState.Run;
                this.anim.SetBool("IsRun", true);
            }

            var movement = Vector3.forward * this.moveSpeed * Time.deltaTime;
            this.transform.Translate(movement);
        }

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        //Debug.Log(distanceToTarget);
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        //훈련시 속도가 빨라 Collider를 뚫고 지나가는거 방지 
        if (((this.transform.localPosition.x < -6f) || (this.transform.localPosition.x > 6f))
            || ((this.transform.localPosition.z > 6f) || (this.transform.localPosition.z < -6f)))
        {
            SetReward(-0.1f);
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionOut = actionsOut.DiscreteActions;

        float h = Input.GetAxisRaw("Horizontal");       // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical");         // -1, 0, 1

        discreteActionOut[0] = (int)h;
        discreteActionOut[1] = (int)v;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Wall"))
        {
            //Debug.Log("<color=red>EndEpisode</color>");
            SetReward(-0.1f);
            EndEpisode();
        }
    }
}
