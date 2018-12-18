using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public PlayerStateMachine m_StateMachine;
        [SerializeField] public PlayerControlPayload m_Control;

        [SerializeField] public bool m_IsAI;

        [SerializeField] public int m_Index;
        
        private void Awake()
        {
            PlayerDriver driver;
            if (m_IsAI) driver = gameObject.AddComponent<AIDriver>();
            else driver = gameObject.AddComponent<KeyboardDriver>();

            m_Control.Actions = new PlayerActions();
            driver.Init(this);

            m_Control.Transform = transform;

            m_Control.Anim.Init();

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
        [SerializeField] public PlayerAnim Anim;
        [NonSerialized] public PlayerActions Actions;
        [NonSerialized] public Direction Facing;
        [NonSerialized] public Transform Transform;
    }

    public enum Direction
    {
        Right,
        Left
    }
}
