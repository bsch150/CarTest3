using UnityEngine;
using System;
using System.Collections;

public class WheelCollider2 : MonoBehaviour
{
    //private variables
    private bool isGrounded;
    private float wheelRadius = 1;
    private Rigidbody carRB;
    private float acc = 10000;
    private float velocity = 0;
    private float maxSteerAngle = 25;
    private float currentForce = 0;
    private float maxCurrentForce = 35000;
    float rollingDecayRate = .99f;


    void calculateIsGrounded()
    {
        RaycastHit hitInfo;
        var result = Physics.Raycast(new Ray(this.transform.position, -this.transform.up),out hitInfo);
        if (result)
        {
            if ((hitInfo.point - this.transform.position).magnitude <= wheelRadius)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

        }
    }
    public void steer(float hAxis)
    {
        this.transform.localEulerAngles = new Vector3(0, hAxis * maxSteerAngle, 0);
    }
    public void move(float inputVal)
    {
        if (float.IsNaN(inputVal))
        {
            inputVal = 0;
        }
        if (isGrounded)
        {
            currentForce += (inputVal * acc);
            currentForce = Math.Min(maxCurrentForce, Math.Abs(currentForce)) * ((currentForce < 0) ? -1 : 1);
            var force = this.transform.forward * currentForce;
            //Debug.Log("force = " + force);
            carRB.AddForceAtPosition(force, this.transform.position);
            Debug.Log("transofrm pos = " + this.transform.position);
        }
        else
        {
        }
        currentForce *= rollingDecayRate;
    }
    public void assignCarRigidBody(Rigidbody rb)
    {
        carRB = rb;
    }
    public void assignWheelRadius(float rad)
    {
        wheelRadius = rad;
    }
    public void FixedUpdate()
    {
        calculateIsGrounded();
    }
    public bool IsGrounded() {
        return isGrounded;
    }
}
