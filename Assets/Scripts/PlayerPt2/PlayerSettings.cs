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
        [SerializeField] public float m_JumpSpeed = 30f;

        [SerializeField] public float m_MoveSpeed = 8f;

        [SerializeField] public float m_DashDistance = 5f;
        [SerializeField] public float m_DashTime = .15f;
        [SerializeField] public float m_DashEndDelay = .01f;

        [SerializeField] public float m_GroundedHeight = .1f;

        [SerializeField] public LayerMask m_IgnorePlayerMask;
        [SerializeField] public AnimationCurve m_DashCurve;
    }
}
