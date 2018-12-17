using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    public class KeyboardInput : MonoBehaviour
    {
        public PlayerActions Actions;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Actions.Jump = true;
            else Actions.Jump = false;

            if (Input.GetKeyDown(KeyCode.F)) Actions.Attack = true;
            else Actions.Attack = false;

            if (Input.GetKey(KeyCode.B)) Actions.Block = true;
            else Actions.Block = false;

            if (Input.GetKeyDown(KeyCode.E)) Actions.Dash = true;
            else Actions.Dash = false;

            Actions.Move = Input.GetAxisRaw("Horizontal0");
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
