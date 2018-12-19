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
            PlayerActions actions = m_Player.m_Control.Actions;
            actions.Block = true;
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
