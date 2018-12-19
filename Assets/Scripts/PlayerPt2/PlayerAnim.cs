using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerPt2
{
    [Serializable]
    public class PlayerAnim
    {
        [SerializeField] private Animation m_CharacterAnim;
        [SerializeField] private Animation m_SwordAnim;

        [SerializeField] private List<string> m_Attacks;

        private const string RunForward = "RunForward";
        private const string RunBackward = "RunBackward";
        private const string Idle = "Idle";

        private const string FallForward = "FallForward";
        private const string FallBackward = "FallBackward";

        private const string Jump = "Jump";

        private const string Dash = "Dash";
        private const string Telegraph = "Telegraph";
        private const string GotHit = "GotHit";

        private const string Block = "Block";

        public void Init()
        {
            m_CharacterAnim[RunForward].speed = 1.5f;
            m_CharacterAnim[RunBackward].speed = -1.5f;
            m_CharacterAnim[Dash].speed = 2f;
        }

        public void LookDirection(Direction facing)
        {
            m_CharacterAnim.transform.root.right = facing == Direction.Right ? Vector3.right : Vector3.left;
        }

        public void PlayRun(float x, Direction facing)
        {
            if (Mathf.Abs(x) > 0)
            {
                if (x > 0 && facing == Direction.Right) m_CharacterAnim.CrossFade(RunForward);

                else if (x < 0 && facing == Direction.Left) m_CharacterAnim.CrossFade(RunForward);

                else if (x > 0 && facing == Direction.Left) m_CharacterAnim.CrossFade(RunBackward);

                else if (x < 0 && facing == Direction.Right) m_CharacterAnim.CrossFade(RunBackward);
            }
            else
            {
                m_CharacterAnim.CrossFade(Idle, .5f);
            }
        }

        public void PlayJump(float x, float y, Direction facing)
        {
            if (y <= 0)
            {
                if (x > 0 && facing == Direction.Right) m_CharacterAnim.Play(FallForward);

                else if (x < 0 && facing == Direction.Left) m_CharacterAnim.Play(FallForward);

                else if (x > 0 && facing == Direction.Left) m_CharacterAnim.Play(FallBackward);

                else if (x < 0 && facing == Direction.Right) m_CharacterAnim.Play(FallBackward);
            }
            else
            {
                m_CharacterAnim.Play(Jump);
            }
        }

        public void PlayDash()
        {
            m_CharacterAnim.Stop();
            m_CharacterAnim.Play(Dash);
        }

        public void PlayTelegraph()
        {
            m_CharacterAnim.Stop();
            m_CharacterAnim.Play(Telegraph);
        }

        public void PlayRandomAttack()
        {
            int rand = UnityEngine.Random.Range(0, m_Attacks.Count);

            m_CharacterAnim.Stop();
            m_CharacterAnim.Play(m_Attacks[rand]);
            m_SwordAnim.Stop();
            m_SwordAnim.Play(m_Attacks[rand]);
        }

        public void PlayHit()
        {
            m_CharacterAnim.Stop();
            m_CharacterAnim.Play(GotHit);
        }

        public void PlayBlock()
        {
            m_CharacterAnim.Stop();
            m_CharacterAnim.Play(Block);
        }
    }

    [Serializable]
    public class OneShot
    {
        [SerializeField] public string Name;
        [SerializeField] public float Speed = 1;
        [NonSerialized] public float Length;
    }
}
