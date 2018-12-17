using System;
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

        private float m_JumpSpeed = 30f;

        private float m_MoveSpeed = 8f;

        private float m_DashDistance = 5f;
        private float m_DashTime = .15f;
        private float m_DashEndDelay = .01f;

        [SerializeField] private LayerMask m_IgnorePlayerMask;
        [SerializeField] private AnimationCurve m_DashCurve;

        public void Jump()
        {
            Vector3 currentVelocity = m_Rigidbody.velocity;
            currentVelocity.y = m_JumpSpeed;
            m_Rigidbody.velocity = currentVelocity;
        }
        
        // for the greatest control, it's better this not be a coroutine and be handled in PlayerStateMachine
        public IEnumerator Dash(float direction)
        {
            m_Rigidbody.velocity = new Vector3(0, 0, 0);

            Vector3 initialPosition = m_Rigidbody.position;

            m_Rigidbody.isKinematic = true;

            float distanceMult = CalculateMoveDistance(Vector3.right * direction, m_DashDistance, m_IgnorePlayerMask);

            float t = 0;
            while (t <= m_DashTime)
            {
                t += Time.fixedDeltaTime;
                //rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);
                m_Rigidbody.position = (Vector3.Lerp(initialPosition, initialPosition + new Vector3(m_DashDistance * distanceMult * direction, 0, 0), m_DashCurve.Evaluate(t / m_DashTime)));

                yield return new WaitForFixedUpdate();
            }

            m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);

            m_Rigidbody.isKinematic = false;

            yield return new WaitForSeconds(m_DashEndDelay);
        }

        float CalculateMoveDistance(Vector3 direction, float moveDistance, LayerMask mask)
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
            currentVelocity.x = x * m_MoveSpeed;
            m_Rigidbody.velocity = currentVelocity;
        }
    }
}
