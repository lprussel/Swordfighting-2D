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
            m_StateMap.Add(StateID.Recoiling, new RecoilingState(control));
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
        Recoiling,
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

        public Coroutine m_StateCoroutine;
        public abstract IEnumerator StateRoutine();

        public Action<StateID> RequestStateChange;

        public virtual void Enter()
        {
            Debug.Log("Begin " + m_ID.ToString());
            m_StateCoroutine = CoroutineRunner.StartCoroutine(StateRoutine());
        }

        public virtual void Exit()
        {
            CoroutineRunner.StopCoroutine(m_StateCoroutine);
            Debug.Log("End " + m_ID.ToString());
        }
    }

    [Serializable]
    public class MovingState : PlayerState
    {
        public MovingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Moving; } }

        public override IEnumerator StateRoutine()
        {
            while (true)
            {
                float x = m_Control.Actions.Move;
                float y = m_Control.Physics.m_Rigidbody.velocity.y;
                if (Mathf.Abs(x) > 0)
                {
                    m_Control.Facing = x > 0 ? Direction.Right : Direction.Left;
                    m_Control.Anim.LookDirection(m_Control.Facing);
                }

                if (m_Control.Grounded.Check()) m_Control.Anim.PlayRun(x, m_Control.Facing);
                else m_Control.Anim.PlayJump(x, y, m_Control.Facing);

                m_Control.Physics.Run(x);
                
                if (m_Control.Actions.Jump && m_Control.Grounded.Check())
                {
                    m_Control.Physics.Jump();
                }

                if (m_Control.Actions.Attack)
                {
                    RequestStateChange(StateID.Attacking);
                }

                if (m_Control.Actions.Block)
                {
                    RequestStateChange(StateID.Blocking);
                }

                if (m_Control.Actions.Dash)
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

        public override IEnumerator StateRoutine()
        {
            while (true)
            {
                m_Control.Physics.SlowToStop(10);
                m_Control.Anim.PlayBlock();
                if (!m_Control.Actions.Block)
                {
                    RequestStateChange(StateID.Moving);
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }

    [Serializable]
    public class AttackingState : PlayerState
    {
        public AttackingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Attacking; } }
         
        public override IEnumerator StateRoutine()
        {
            float direction = m_Control.Facing == Direction.Right ? 1 : -1;

            Vector3 initialPosition;
            Vector3 targetPosition;
            m_Control.Physics.BeginDash(direction, GameManager.PSettings.AttackDistance, GameManager.PSettings.EverythingMask, m_Control.BodyCenter.position, out initialPosition, out targetPosition);

            m_Control.Anim.PlayTelegraph();
            yield return new WaitForSeconds(.35f);

            bool hitPlayerFlag = false;
            
            m_Control.Anim.PlayRandomAttack();
            float t = 0;
            while (t <= GameManager.PSettings.AttackTime)
            {
                m_Control.Physics.ProgressDash(initialPosition, targetPosition, t, GameManager.PSettings.AttackTime, GameManager.PSettings.AttackCurve);
                if ((t / GameManager.PSettings.AttackTime) > .5f && !hitPlayerFlag)
                {
                    RaycastHit[] hits = Physics.RaycastAll(m_Control.BodyCenter.position, m_Control.Transform.right, GameManager.PSettings.AttackDistance, GameManager.PSettings.AttackMask, QueryTriggerInteraction.UseGlobal);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        hitPlayerFlag = true;

                        RaycastHit hit = hits[i];
                        if ((hit.collider.tag == "Interactive" || hit.collider.tag == "Player") && hit.collider.gameObject != m_Control.Transform.gameObject)
                        {
                            IHittable target = hit.collider.GetComponent<IHittable>();
                            target.GotHit(1, m_Control.Transform);
                        }
                    }
                }
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            m_Control.Physics.EndDash();
            yield return new WaitForSeconds(GameManager.PSettings.AttackEndDelay);
            RequestStateChange(StateID.Moving);
        }

        // early exit is important
        public override void Exit()
        {
            base.Exit();
            if (m_Control.Physics.IsKinematic()) m_Control.Physics.EndDash();
        }
    }

    [Serializable]
    public class DashingState : PlayerState
    {
        public DashingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Dashing; } }
        
        public override IEnumerator StateRoutine()
        {
            float direction = m_Control.Actions.Move == 0 ?
                (m_Control.Physics.m_Rigidbody.transform.right.x > 0 ? -1 : 1) : (m_Control.Actions.Move > 0 ? 1 : -1);

            Vector3 initialPosition;
            Vector3 targetPosition;
            m_Control.Physics.BeginDash(direction, GameManager.PSettings.DashDistance, GameManager.PSettings.IgnorePlayerMask, m_Control.BodyCenter.position, out initialPosition, out targetPosition);

            m_Control.Anim.PlayDash();

            float t = 0;
            while (t <= GameManager.PSettings.DashTime)
            {
                m_Control.Physics.ProgressDash(initialPosition, targetPosition, t, GameManager.PSettings.DashTime, GameManager.PSettings.DashCurve);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            m_Control.Physics.EndDash();

            yield return new WaitForSeconds(GameManager.PSettings.DashEndDelay);

            RequestStateChange(StateID.Moving);
        }

        // early exit is important
        public override void Exit()
        {
            base.Exit();
            if (m_Control.Physics.IsKinematic()) m_Control.Physics.EndDash();
        }
    }

    [Serializable]
    public class RecoilingState : PlayerState
    {
        public RecoilingState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Recoiling; } }

        public override IEnumerator StateRoutine()
        {
            m_Control.Anim.PlayHit();
            float t = 0;
            while (t < .25f)
            {
                m_Control.Physics.SlowToStop(5);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            RequestStateChange(StateID.Moving);
        }
    }

    [Serializable]
    public class HitState : PlayerState
    {
        public HitState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Hit; } }

        public override IEnumerator StateRoutine()
        {
            m_Control.Anim.PlayHit();
            float t = 0;
            while (t < .25f)
            {
                m_Control.Physics.SlowToStop(5);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            RequestStateChange(StateID.Moving);
        }
    }

    [Serializable]
    public class DeadState : PlayerState
    {
        public DeadState(PlayerControlPayload control) : base(control) { }

        public override StateID m_ID { get { return StateID.Dead; } }

        public override IEnumerator StateRoutine()
        {
            m_Control.Anim.PlayHit();
            float t = 0;
            while (t < .25f)
            {
                m_Control.Physics.SlowToStop(5);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
