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
    public void move(float inputVal)
    {
        if (float.IsNaN(inputVal))
        {
            inputVal = 0;
        }
        var force = this.transform.forward * acc * inputVal;
        Debug.Log("force = " + force);
        carRB.AddForceAtPosition(force, this.transform.position);
        
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
