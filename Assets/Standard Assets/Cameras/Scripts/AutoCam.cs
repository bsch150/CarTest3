using System;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace UnityStandardAssets.Cameras
{
    [ExecuteInEditMode]
    public class AutoCam : PivotBasedCameraRig
    {
        [SerializeField] private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
        [SerializeField] private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
        [SerializeField] private float m_RollSpeed = 10.0f;// How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField] private bool m_FollowVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
        [SerializeField] private bool m_FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
        [SerializeField] private float m_SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        [SerializeField] private float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] private float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation

        private float m_LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
        private float m_CurrentTurnAmount; // How much to turn the camera
        private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
        private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )
        private int counter = 0;
		private int playerNum =1;
        private Vector3 rotateOn = new Vector3(0,0,0);

        void setTarget(Transform newT)
        {
			//Debug.Log ("setting target to " + newT + ", pNum = " + playerNum);
            m_Target = newT;
        }
		void assignPlayerNum(int num){
			playerNum = num;
			//Debug.Log ("playerNum = " + playerNum);
		}
        void toggle()
        {
            if (counter < 0)
            {
                Debug.Log("toggling");
                counter = 10;
                m_FollowTilt = !m_FollowTilt;
                if (!m_FollowTilt)
                {
                   // m_FollowTilt = true;
                    m_MoveSpeed = 10;
                    m_TurnSpeed = 10;
                    m_RollSpeed = 0;

                }
                else
                {
                    m_MoveSpeed = 50;
                    m_TurnSpeed = 50;
                    m_SpinTurnLimit = 500;
                    m_RollSpeed = 55;
                }
            }
        }
        void setRotateOn(Vector2 vec)
        {

            rotateOn = new Vector3(vec[0], vec[1], 0);
        }

        protected override void FollowTarget(float deltaTime)
        {
            counter--;
         
            // if no target, or no time passed then we quit early, as there is nothing to do
            if (!(deltaTime > 0) || m_Target == null)
            {
                return;
            }

            // initialise some vars, we'll be modifying these in a moment
            var targetForward = m_Target.forward;
            var targetUp = m_Target.up;

            if (m_FollowVelocity && Application.isPlaying)
            {
                // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
                // but only if the object is traveling faster than a given threshold.

                if (targetRigidbody.velocity.magnitude > m_TargetVelocityLowerLimit)
                {
                    // velocity is high enough, so we'll use the target's velocty
                    targetForward = targetRigidbody.velocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);
            }
            else
            {
                // we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

                // This section allows the camera to stop following the target's rotation when the target is spinning too fast.
                // eg when a car has been knocked into a spin. The camera will resume following the rotation
                // of the target when the target's angular velocity slows below the threshold.
                var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z)*Mathf.Rad2Deg;
                if (m_SpinTurnLimit > 0)
                {
                    var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle))/deltaTime;
                    var desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit*0.75f, targetSpinSpeed);
                    var turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
                    if (Application.isPlaying)
                    {
                        m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount,
                                                             ref m_TurnSpeedVelocityChange, turnReactSpeed);
                    }
                    else
                    {
                        // for editor mode, smoothdamp won't work because it uses deltaTime internally
                        m_CurrentTurnAmount = desiredTurnAmount;
                    }
                }
                else
                {
                    m_CurrentTurnAmount = 1;
                }
                m_LastFlatAngle = currentFlatAngle;
            }

            // camera position moves towards target position:
            var temp = Vector3.Lerp(transform.position, m_Target.position , deltaTime * m_MoveSpeed);
            //temp = rotateAroundAxis(transform.right, (float)(Math.PI/6.0), temp);
            transform.position = temp;
            //doCamRotation();

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')
            if (!m_FollowTilt)
            {
                targetForward.y = 0;
                if (targetForward.sqrMagnitude < float.Epsilon)
                {
                    targetForward = transform.forward ;
                }
            }
            var rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);

            // and aligning with the target object's up direction (i.e. its 'roll')
            m_RollUp = m_RollSpeed > 0 ? Vector3.Slerp(m_RollUp, targetUp, m_RollSpeed * deltaTime) : Vector3.up;
            //m_RollUp =  Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, m_TurnSpeed*m_CurrentTurnAmount*deltaTime);
            //transform.rotation = Quaternion.LookRotation(rotateOn);
			//Debug.Log (playerNum + " focused on " + m_Target);
        }
        Vector3 rotateAroundAxis(Vector3 axis, float theta, Vector3 vec)
        {
            Vector3 w = axis;
            w = Vector3.Normalize(w);
            Vector3 t = w;
            if (Math.Abs(w.x) == Math.Min(Math.Abs(w.x),Math.Min(Math.Abs(w.y),Math.Abs(w.z)))) {
                t.x = 1;
            }else if (Math.Abs(w.y) == Math.Min(Math.Abs(w.x), Math.Min(Math.Abs(w.y), Math.Abs(w.z)))) {
                t.y = 1;
            } else if(Math.Abs(w.x) == Math.Min(Math.Abs(w.x), Math.Min(Math.Abs(w.y), Math.Abs(w.z)))) {
                t.z = 1;
            }
            Vector3 u = Vector3.Cross(w, t);
            u = Vector3.Normalize(u);
            Vector3 v = Vector3.Cross(w, u);
            Vector3 res = vec;
            res = new Vector3(u.x * res.x + u.y * res.y + u.z * res.z,
            v.x * res.x + v.y * res.y + v.z * res.z,
            w.x * res.x + w.y * res.y + w.z * res.z);
            res = new Vector3((float)(Math.Cos(theta) * res.x + Math.Sin(theta) * res.y),
            (float)(-Math.Sin(theta) * res.x + Math.Cos(theta) * res.y), res.z);
            res = new Vector3(u.x * res.x + v.x * res.y + w.x * res.z,
            u.y * res.x + v.y * res.y + w.y * res.z,
            u.z * res.x + v.z * res.y + w.z * res.z);
            return res;
        }
        float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        /*

  PVector res = vec.get();
  res = new PVector(u.x*res.x + u.y*res.y + u.z*res.z, 
  v.x*res.x + v.y*res.y + v.z*res.z, 
  w.x*res.x + w.y*res.y + w.z*res.z);
  res = new PVector(cos(theta)*res.x + sin(theta)*res.y, 
  -sin(theta)*res.x + cos(theta)*res.y, res.z);
  res = new PVector(u.x*res.x + v.x*res.y + w.x*res.z, 
  u.y*res.x + v.y*res.y + w.y*res.z, 
  u.z*res.x + v.z*res.y + w.z*res.z);
  return res;
}
    */
    }
}
