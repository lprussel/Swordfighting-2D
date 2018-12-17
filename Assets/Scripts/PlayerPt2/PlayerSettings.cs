using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    [CreateAssetMenu(fileName = "PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        [SerializeField] public float JumpSpeed = 30f;

        [SerializeField] public float MoveSpeed = 8f;

        [SerializeField] public float DashDistance = 5f;
        [SerializeField] public float DashTime = .15f;
        [SerializeField] public float DashEndDelay = .01f;

        [SerializeField] public float GroundedHeight = .1f;

        [SerializeField] public LayerMask IgnorePlayerMask;
        [SerializeField] public AnimationCurve DashCurve;
    }
}
