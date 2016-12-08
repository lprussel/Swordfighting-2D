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
				if (Mathf.Abs (rig.velocity.y) > .05f && !playerManager.grounded)
				{
					if (rig.velocity.y > 0)
						playerAnimation.Play ("Jump");
					else
					{
						if (rig.velocity.x > 0 && transform.right.x > 0)
							playerAnimation.Play ("FallForward");
						else if (rig.velocity.x < 0 && transform.right.x < 0)
							playerAnimation.Play ("FallForward");
						else if (rig.velocity.x > 0 && transform.right.x < 0)
							playerAnimation.Play ("FallBackward");
						else if (rig.velocity.x < 0 && transform.right.x > 0)
							playerAnimation.Play ("FallBackward");
					}

					return;
				}
				else if (playerManager.grounded && Mathf.Abs (rig.velocity.x) > 0)
				{
					if (rig.velocity.x > 0 && transform.right.x > 0)
						playerAnimation.CrossFade ("RunForward");
					else if (rig.velocity.x < 0 && transform.right.x < 0)
						playerAnimation.CrossFade ("RunForward");
					else if (rig.velocity.x > 0 && transform.right.x < 0)
						playerAnimation.CrossFade ("RunBackward");
					else if (rig.velocity.x < 0 && transform.right.x > 0)
						playerAnimation.CrossFade ("RunBackward");
				}
				else
					playerAnimation.CrossFade ("Idle", .5f);
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
				if (Mathf.Abs (rig.velocity.y) > .05f)
				{
					if (rig.velocity.y > 0)
						playerAnimation.Play ("Jump");
					else
					{
						if (rig.velocity.x > 0 && transform.right.x > 0)
							playerAnimation.Play ("FallForward");
						else if (rig.velocity.x < 0 && transform.right.x < 0)
							playerAnimation.Play ("FallForward");
						else if (rig.velocity.x > 0 && transform.right.x < 0)
							playerAnimation.Play ("FallBackward");
						else if (rig.velocity.x < 0 && transform.right.x > 0)
							playerAnimation.Play ("FallBackward");
					}

					return;
				}
				break;
			case PlayerManager.PlayerState.HIT:
				break;
			case PlayerManager.PlayerState.CANT_MOVE:
				break;
		}
	}

	void OnTelegraph ()
	{
		playerAnimation.Play ("Telegraph");
		Debug.Log ("TELEGRAPH!");
	}

	void OnAttack ()
	{
		string attackName = GetAttackName ();

		playerAnimation.Stop ();
		swordAnimation.Stop ();

		playerAnimation.Play (attackName);
		swordAnimation.Play (attackName);

		Debug.Log ("ATTACK!");
	}

	void OnDash ()
	{
		playerAnimation.Stop ();
		playerAnimation.Play ("Dash");
		Debug.Log ("DASH!");
	}

	void OnRecoil ()
	{
		playerAnimation.Stop ();
		playerAnimation.Play ("GotHit");
		Debug.Log ("RECOIL!");
	}

	string GetAttackName ()
	{
		int rand = Random.Range (0, 3) + 1;
		string name = "Slash" + rand.ToString ();
		return name;
	}
}
