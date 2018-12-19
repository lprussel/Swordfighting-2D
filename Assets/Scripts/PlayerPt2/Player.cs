using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public class Player : MonoBehaviour, IHittable
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

        public void GotHit(int amount, Transform hitBy)
        {
            if (m_StateMachine.m_CurrentStateID == StateID.Dashing) return;

            if (m_StateMachine.m_CurrentStateID == StateID.Blocking && transform.right.x > 0 && hitBy.position.x > transform.position.x ||
            m_StateMachine.m_CurrentStateID == StateID.Blocking && transform.right.x < 0 && hitBy.position.x < transform.position.x)
            {
                // check if can reposte
                if (true)
                {
                    //EffectsManager.instance.StartCoroutine(EffectsManager.instance.OnPlayerBlock(this));
                    AudioManager.instance.PlaySound(AudioManager.SoundSet.BLOCK);

                    int dir = hitBy.position.x > transform.position.x ? -1 : 1;
                    m_Control.Physics.m_Rigidbody.velocity = new Vector3(25 * dir, 0, 0);
                }
                else
                {
                    AudioManager.instance.PlaySound(AudioManager.SoundSet.REPOSTE);

                    Player player = hitBy.GetComponent<Player>();
                    if (player != null)
                    {
                        player.m_StateMachine.SetStateFromID(StateID.Recoiling);
                    }
                }
            }
            else
            {
                EffectsManager.instance.StartCoroutine(EffectsManager.instance.OnPlayerHit(GameManager.GetOtherPlayer(m_Index).transform, transform));
                AudioManager.instance.PlaySound(AudioManager.SoundSet.HIT);

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
    }

    [Serializable]
    public class PlayerControlPayload
    {
        [SerializeField] public PlayerHealth Health;
        [SerializeField] public PlayerPhysics Physics;
        [SerializeField] public GroundedChecker Grounded;
        [SerializeField] public PlayerAnim Anim;
        [SerializeField] public Transform BodyCenter;
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
