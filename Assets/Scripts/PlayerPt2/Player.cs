using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerStateMachine m_StateMachine;
        [SerializeField] private PlayerControlPayload m_Control;

        private void Awake()
        {
            m_StateMachine = new PlayerStateMachine(m_Control);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Hit(1);
            }
        }

        public void Hit(int amount)
        {
            m_Control.Health.ReduceHealth(amount);
            if (m_Control.Health.m_Health == 0)
            {
                m_StateMachine.SetStateFromID(StateID.Dead);
            }
            else
            {
                m_StateMachine.SetStateFromID(StateID.Hit);
            }
        }
    }

    [Serializable]
    public class PlayerControlPayload
    {
        [SerializeField] public PlayerHealth Health;
        [SerializeField] public PlayerPhysics Physics;
        [SerializeField] public GroundedChecker Grounded;
        [SerializeField] public KeyboardInput Input;
    }
}
