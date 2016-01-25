using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("EBrake");
            float jump = CrossPlatformInputManager.GetAxis("Jump");
            float axisToggle = CrossPlatformInputManager.GetAxis("AxisToggle");
            float boost = CrossPlatformInputManager.GetAxis("Boost");
            m_Car.Move(h, v, v, handbrake,jump,axisToggle,boost);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
