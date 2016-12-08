using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
	public Animation playerAnimation;
	public Animation swordAnimation;
	public PlayerManager playerManager;
	private Rigidbody rig;

	void Start ()
	{
		rig = playerManager.GetRigidbody();

		playerAnimation ["RunForward"].speed = 1.5f;
		playerAnimation ["RunBackward"].speed = -1.5f;
		playerAnimation ["Dash"].speed = 2f;

		playerManager.OnTelegraph += OnTelegraph;
		playerManager.OnAttack += OnAttack;
		playerManager.OnDash += OnDash;
		playerManager.OnRecoil += OnRecoil;
		playerManager.OnHit += OnRecoil;
	}

	void OnDestroy ()
	{
		playerManager.OnTelegraph -= OnTelegraph;
		playerManager.OnAttack -= OnAttack;
		playerManager.OnDash -= OnDash;
		playerManager.OnRecoil -= OnRecoil;
		playerManager.OnHit -= OnRecoil;
	}

	void Update ()
	{
		switch (playerManager.playerState)
		{
			case PlayerManager.PlayerState.IDLE:
                if (playerManager.grounded)
                    GroundedAnimation();
                else
                    InAirAnimation();
				break;
			case PlayerManager.PlayerState.BLOCKING:
				playerAnimation.Play ("Block");
				break;
			case PlayerManager.PlayerState.TELEGRAPHING:
				break;
			case PlayerManager.PlayerState.ATTACKING:
				break;
			case PlayerManager.PlayerState.RECOILING:
				break;
			case PlayerManager.PlayerState.DASHING:
				break;
			case PlayerManager.PlayerState.JUMPING:
				break;
			case PlayerManager.PlayerState.HIT:
				break;
			case PlayerManager.PlayerState.CANT_MOVE:
				break;
		}
	}

    void GroundedAnimation()
    {
        if (Mathf.Abs(rig.velocity.x) > 0)
        {
            if (rig.velocity.x > 0 && playerManager.facingDirection == PlayerManager.FacingDirection.RIGHT)
                playerAnimation.CrossFade("RunForward");
            else if (rig.velocity.x < 0 && playerManager.facingDirection == PlayerManager.FacingDirection.LEFT)
                playerAnimation.CrossFade("RunForward");
            else if (rig.velocity.x > 0 && playerManager.facingDirection == PlayerManager.FacingDirection.LEFT)
                playerAnimation.CrossFade("RunBackward");
            else if (rig.velocity.x < 0 && playerManager.facingDirection == PlayerManager.FacingDirection.RIGHT)
                playerAnimation.CrossFade("RunBackward");
        }
        else
            playerAnimation.CrossFade("Idle", .5f);
    }

    void InAirAnimation ()
    {
        if (rig.velocity.y <= 0)
        {
            if (rig.velocity.x > 0 && playerManager.facingDirection == PlayerManager.FacingDirection.RIGHT)
                playerAnimation.Play("FallForward");
            else if (rig.velocity.x < 0 && playerManager.facingDirection == PlayerManager.FacingDirection.LEFT)
                playerAnimation.Play("FallForward");
            else if (rig.velocity.x > 0 && playerManager.facingDirection == PlayerManager.FacingDirection.LEFT)
                playerAnimation.Play("FallBackward");
            else if (rig.velocity.x < 0 && playerManager.facingDirection == PlayerManager.FacingDirection.RIGHT)
                playerAnimation.Play("FallBackward");
        }
        else
            playerAnimation.Play("Jump");
    }

	void OnTelegraph ()
	{
		playerAnimation.Play ("Telegraph");
	}

	void OnAttack ()
	{
		string attackName = GetAttackName ();

		playerAnimation.Stop ();
		swordAnimation.Stop ();

		playerAnimation.Play (attackName);
		swordAnimation.Play (attackName);
	}

	void OnDash ()
	{
		playerAnimation.Stop ();
		playerAnimation.Play ("Dash");
	}

	void OnRecoil ()
	{
		playerAnimation.Stop ();
		playerAnimation.Play ("GotHit");
	}

	string GetAttackName ()
	{
		int rand = Random.Range (0, 3) + 1;
		string name = "Slash" + rand.ToString ();
		return name;
	}
}
