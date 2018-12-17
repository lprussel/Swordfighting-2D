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

            actions.Jump = Input.GetKeyDown(KeyCode.Space);

            actions.Attack = Input.GetKeyDown(KeyCode.F);

            actions.Dash = Input.GetKeyDown(KeyCode.E);

            actions.Block = Input.GetKey(KeyCode.B);
            
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
