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

        [SerializeField] public bool m_IsAI;
        [NonSerialized] PlayerDriver m_Driver;

        [SerializeField] public int m_PlayerIndex;
        
        private void Awake()
        {
            if (m_IsAI) m_Driver = gameObject.AddComponent<AIDriver>();
            else m_Driver = gameObject.AddComponent<KeyboardDriver>();

            m_Control.Actions = new PlayerActions();
            m_Driver.Init(m_Control.Actions, m_PlayerIndex);

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
        [NonSerialized] public PlayerActions Actions;
    }
}
