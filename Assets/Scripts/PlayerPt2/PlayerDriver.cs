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

            if (Input.GetKeyDown(KeyCode.Space)) actions.Jump = true;
            else m_Player.m_Control.Actions.Jump = false;

            if (Input.GetKeyDown(KeyCode.F)) actions.Attack = true;
            else m_Player.m_Control.Actions.Attack = false;

            if (Input.GetKey(KeyCode.B)) actions.Block = true;
            else m_Player.m_Control.Actions.Block = false;

            if (Input.GetKeyDown(KeyCode.E)) actions.Dash = true;
            else m_Player.m_Control.Actions.Dash = false;

            actions.Move = Input.GetAxisRaw(m_MoveAxis);
        }
    }

    public class AIDriver : PlayerDriver
    {
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
