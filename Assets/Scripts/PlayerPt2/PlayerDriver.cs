using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public abstract class PlayerDriver : MonoBehaviour
    {
        protected Player m_Player;

        public void Init(Player player) { m_Player = player; }
    }

    public class KeyboardDriver : PlayerDriver
    {
        private string m_MoveAxis;
        public void Update()
        {
            if (string.IsNullOrEmpty(m_MoveAxis)) m_MoveAxis = "Horizontal" + m_Player.m_Index.ToString();

            PlayerActions actions = m_Player.m_Control.Actions;

            actions.Jump = Input.GetButtonDown("Jump" + m_Player.m_Index.ToString());

            actions.Attack = Input.GetButtonDown("Attack" + m_Player.m_Index.ToString());

            actions.Dash = Input.GetButtonDown("Dodge" + m_Player.m_Index.ToString());

            actions.Block = Input.GetButton("Block" + m_Player.m_Index.ToString());
            
            actions.Move = Input.GetAxisRaw(m_MoveAxis);
        }
    }

    public class AIDriver : PlayerDriver
    {
        private void Update()
        {
            Player otherPlayer = GameManager.GetOtherPlayer(m_Player.m_Index);
            float distToPlayer = Vector3.Distance(transform.position, otherPlayer.transform.position);

            PlayerActions actions = m_Player.m_Control.Actions;

            float diffX = otherPlayer.transform.position.x - transform.position.x;
            float diffY = otherPlayer.transform.position.y - transform.position.y;
            
            if (distToPlayer < GameManager.PSettings.AttackDistance && diffY < .5f)
            {
                actions.Move = 0f;
                actions.Jump = false;

                if (otherPlayer.m_StateMachine.m_CurrentStateID == StateID.Attacking)
                {
                    actions.Attack = false;
                    actions.Block = true;
                }
                else
                {
                    actions.Block = false;
                    actions.Attack = true;
                }
            }
            else
            {
                actions.Block = false;
                actions.Attack = false;
                if (diffX > 0)
                {
                    actions.Move = 1f;
                }
                else
                {
                    actions.Move = -1f;
                }

                if (diffY > GameManager.PSettings.JumpSpeed * .1f)
                {
                    actions.Jump = true;
                }
                else
                {
                    actions.Jump = false;
                }
            }
        }
    }

    [Serializable]
    public class PlayerActions
    {
        // event
        public bool Jump;

        // event
        public bool Attack;

        // event
        public bool Dash;

        // toggle
        public bool Block;

        // axis
        public float Move;
    }
}
