using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerPt2
{
    // how to handle interruptions?
    // how to handle shared controls eg having running controls while in attack state?
    [Serializable]
    public class PlayerStateMachine
    {
        [SerializeField] public StateID m_CurrentStateID;
        private PlayerState m_CurrentState;

        private Dictionary<StateID, PlayerState> m_StateMap;

        public PlayerStateMachine(PlayerControlPayload control)
        {
            m_StateMap = new Dictionary<StateID, PlayerState>();
            m_StateMap.Add(StateID.Moving, new MovingState(control));
            m_StateMap.Add(StateID.Blocking, new BlockingState(control));
            m_StateMap.Add(StateID.Attacking, new AttackingState(control));
            m_StateMap.Add(StateID.Dashing, new DashingState(control));
            m_StateMap.Add(StateID.Hit, new HitState(control));
            m_StateMap.Add(StateID.Dead, new DeadState(control));

            SetStateFromID(StateID.Moving);
        }

        ~PlayerStateMachine()
        {
        }

        public void SetStateFromID(StateID id)
        {
            SetStateInternal(GetStateFromID(id));
        }

        // for now, clobber state. later maybe don't
        private void SetStateInternal(PlayerState playerState)
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.Exit();
                m_CurrentState.RequestStateChange -= SetStateFromID;
            }

            m_CurrentState = playerState;
            m_CurrentStateID = m_CurrentState.m_ID;
            m_CurrentState.RequestStateChange += SetStateFromID;

            m_CurrentState.Enter();
        }

        public PlayerState GetStateFromID(StateID id)
        {
            PlayerState playerState;
            m_StateMap.TryGetValue(id, out playerState);
            return playerState;
        }
    }
    
    public enum StateID
    {
        Moving,
        Blocking,
        Attacking,
        Dashing,
        Hit,
        Dead
    }

    [Serializable]
    public abstract class PlayerState
    {
        public PlayerState(PlayerControlPayload control)
        {
            m_Control = control;
        }

        protected PlayerControlPayload m_Control;

        public abstract StateID m_ID { get; }

        public Coroutine StateCoroutine;
        public abstract IEnumerator RunState();

        public Action<StateID> RequestStateChange;

        public virtual void Enter()
        {
            Debug.Log("Begin " + m_ID.ToString());
            StateCoroutine = CoroutineRunner.StartCoroutine(RunState());
        }

        public virtual void Exit()
        {
            CoroutineRunner.StopCoroutine(StateCoroutine);
            Debug.Log("End " + m_ID.ToString());
        }
    }

    [Serializable]
    public class MovingState : PlayerState
    {
        public MovingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Moving; } }

        public override IEnumerator RunState()
        {
            while (true)
            {
                m_Control.Physics.Run(Input.GetAxisRaw("Horizontal0"));

                if (m_Control.Input.Actions.Jump && m_Control.Grounded.Check())
                {
                    m_Control.Physics.Jump();
                }

                if (m_Control.Input.Actions.Attack)
                {
                    RequestStateChange(StateID.Attacking);
                }

                if (m_Control.Input.Actions.Block)
                {
                    RequestStateChange(StateID.Blocking);
                }

                if (m_Control.Input.Actions.Dash)
                {
                    RequestStateChange(StateID.Dashing);
                }

                yield return null;
            }
        }
    }

    [Serializable]
    public class BlockingState : PlayerState
    {
        public BlockingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Blocking; } }

        public override IEnumerator RunState()
        {
            while (true)
            {
                if (!m_Control.Input.Actions.Block)
                {
                    RequestStateChange(StateID.Moving);
                }
                yield return null;
            }
        }
    }

    [Serializable]
    public class AttackingState : PlayerState
    {
        public AttackingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Attacking; } }

        public override IEnumerator RunState()
        {
            Debug.Log("Telegraph");
            yield return new WaitForSeconds(.25f);
            Debug.Log("Attack");
            yield return new WaitForSeconds(.5f);
            RequestStateChange(StateID.Moving);
        }
    }

    [Serializable]
    public class DashingState : PlayerState
    {
        public DashingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Dashing; } }
        
        public override IEnumerator RunState()
        {
            float direction = m_Control.Input.Actions.Move == 0 ?
                (m_Control.Physics.m_Rigidbody.transform.right.x > 0 ? -1 : 1) : (m_Control.Input.Actions.Move > 0 ? 1 : -1);

            yield return m_Control.Physics.Dash(direction);

            RequestStateChange(StateID.Moving);
        }
    }

    [Serializable]
    public class HitState : PlayerState
    {
        public HitState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Hit; } }

        public override IEnumerator RunState()
        {
            Debug.Log("Recoiling");
            yield return new WaitForSeconds(.25f);
            RequestStateChange(StateID.Moving);
        }
    }

    [Serializable]
    public class DeadState : PlayerState
    {
        public DeadState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Dead; } }

        public override IEnumerator RunState()
        {
            Debug.Log("Died");
            yield return null;
        }
    }
}
