using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    [Serializable]
    public class GroundedChecker
    {
        [SerializeField] private Transform m_FeetPosition;

        private int m_LastCheckedFrame;
        private bool m_LastGroundedValue;

        public bool Check()
        {
            if (m_LastCheckedFrame != Time.frameCount)
            {
                RaycastHit hit;

                if (Physics.Raycast(m_FeetPosition.position, -Vector3.up, out hit, GameManager.PlayerSettings.m_GroundedHeight, GameManager.PlayerSettings.m_IgnorePlayerMask, QueryTriggerInteraction.UseGlobal))
                {
                    m_LastGroundedValue = true;
                }
                else
                {
                    m_LastGroundedValue = false;
                }

                m_LastCheckedFrame = Time.frameCount;
            }

            return m_LastGroundedValue;
        }
    }
}
