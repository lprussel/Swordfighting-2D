using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerPt2
{
    [Serializable]
    public class PlayerHealth
    {
        public const int MaxHealth = 5;
        public int m_Health = MaxHealth;

        public void ReduceHealth(int amount)
        {
            m_Health = Mathf.Clamp(m_Health - amount, 0, MaxHealth);
        }
    }
}
