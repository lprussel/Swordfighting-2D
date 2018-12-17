﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    // move these control dialing values into the payload
    // maybe using a scriptable object wrapper?
    [Serializable]
    public class PlayerPhysics
    {
        [SerializeField] public Rigidbody m_Rigidbody;
        
        public void Jump()
        {
            Vector3 currentVelocity = m_Rigidbody.velocity;
            currentVelocity.y = GameManager.PSettings.JumpSpeed;
            m_Rigidbody.velocity = currentVelocity;
        }

        public void BeginDash(float direction, out Vector3 initialPosition, out Vector3 targetPosition)
        {
            InstantStop();
            ToggleKinematic(true);

            initialPosition = m_Rigidbody.position;
            float distanceMult = CalculateMoveDistance(Vector3.right * direction, GameManager.PSettings.DashDistance, GameManager.PSettings.IgnorePlayerMask);
            targetPosition = initialPosition + new Vector3(GameManager.PSettings.DashDistance * distanceMult * direction, 0, 0);
        }
        
        public void ProgressDash(Vector3 initialPosition, Vector3 targetPosition, float t)
        {
            float nT = Mathf.Clamp01(t / GameManager.PSettings.DashTime);
            m_Rigidbody.position = (Vector3.Lerp(initialPosition, targetPosition, GameManager.PSettings.DashCurve.Evaluate(nT)));
        }

        public void EndDash()
        {
            ToggleKinematic(false);
        }

        private float CalculateMoveDistance(Vector3 direction, float moveDistance, LayerMask mask)
        {
            float acceptableDistance;
            RaycastHit hit;

            if (Physics.Raycast(m_Rigidbody.position, direction.normalized, out hit, moveDistance, mask, QueryTriggerInteraction.UseGlobal))
            {
                acceptableDistance = (hit.point - m_Rigidbody.position).magnitude / moveDistance;
            }
            else
            {
                acceptableDistance = 1;
            }

            return acceptableDistance;
        }

        public void Run(float x)
        {
            Vector3 currentVelocity = m_Rigidbody.velocity;
            currentVelocity.x = x * GameManager.PSettings.MoveSpeed;
            m_Rigidbody.velocity = currentVelocity;
        }

        public void SlowToStop(float rate)
        {
            float x = Mathf.Lerp(m_Rigidbody.velocity.x, 0, Time.fixedDeltaTime * rate);
            m_Rigidbody.velocity = new Vector3(x, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
        }

        public void InstantStop() { m_Rigidbody.velocity = Vector3.zero; }

        public void ToggleKinematic(bool wantsKinematic) { if (IsKinematic() != wantsKinematic) m_Rigidbody.isKinematic = wantsKinematic; }
        public bool IsKinematic() { return m_Rigidbody.isKinematic; }
    }
}
