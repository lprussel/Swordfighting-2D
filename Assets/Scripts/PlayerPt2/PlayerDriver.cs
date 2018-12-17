using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public abstract class PlayerDriver : MonoBehaviour
    {
        protected PlayerActions m_Actions;
        protected int m_PlayerIndex;

        public void Init(PlayerActions actions, int playerIndex) { m_Actions = actions; m_PlayerIndex = playerIndex; }
    }

    public class KeyboardDriver : PlayerDriver
    {
        private string m_MoveAxis;
        public void Update()
        {
            if (string.IsNullOrEmpty(m_MoveAxis)) m_MoveAxis = "Horizontal" + m_PlayerIndex.ToString();

            if (Input.GetKeyDown(KeyCode.Space)) m_Actions.Jump = true;
            else m_Actions.Jump = false;

            if (Input.GetKeyDown(KeyCode.F)) m_Actions.Attack = true;
            else m_Actions.Attack = false;

            if (Input.GetKey(KeyCode.B)) m_Actions.Block = true;
            else m_Actions.Block = false;

            if (Input.GetKeyDown(KeyCode.E)) m_Actions.Dash = true;
            else m_Actions.Dash = false;

            m_Actions.Move = Input.GetAxisRaw(m_MoveAxis);
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
