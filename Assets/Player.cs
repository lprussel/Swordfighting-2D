using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public enum PlayerState
	{
		IDLE,
		TELEGRAPHING,
		ATTACKING,
		HIT,
		CANT_MOVE
	}
	public PlayerState playerState;

	public int playerNum = 0;

	private Rigidbody rig;

	private float moveSpeed = 3f;
	private float attackSpeed = 100f;

	private float attackTime = .025f;
	private float telegraphTime = .15f;
	private float attackEndDelay = .1f;

	private float horizontalInput;

	private Animation anim;

	private MultiplayerInput input;

	void Start ()
	{
		rig = GetComponent<Rigidbody> ();
		anim = GetComponent<Animation> ();
		input = GetComponent<MultiplayerInput> ();

		input.OnReceiveAttackInput += OnReceiveAttackInput;
		input.OnReceiveDodgeInput += OnReceiveDodgeInput;

		ChangeState (PlayerState.IDLE);
	}

	void OnDisable ()
	{
		input.OnReceiveAttackInput += OnReceiveAttackInput;
		input.OnReceiveDodgeInput += OnReceiveDodgeInput;
	}

	void ChangeState (PlayerState newState)
	{
		if (newState == playerState)
			return;

		switch (newState)
		{
		case PlayerState.IDLE:
			anim.CrossFade ("Idle");
			break;
		case PlayerState.TELEGRAPHING:
			Telegraph ();
			break;
		case PlayerState.ATTACKING:
			Attack ();
			break;
		case PlayerState.HIT:
			break;
		case PlayerState.CANT_MOVE:
			break;
		}

		playerState = newState;
	}

	void Update ()
	{
		switch (playerState)
		{
		case PlayerState.IDLE:
			Idle ();
			break;
		case PlayerState.TELEGRAPHING:
			break;
		case PlayerState.ATTACKING:
			break;
		case PlayerState.HIT:
			break;
		case PlayerState.CANT_MOVE:
			break;
		}
	}

	void Idle ()
	{
		horizontalInput = input.controllerInput.x;

		if (Mathf.Abs (horizontalInput) > .05)
		{
			rig.velocity = new Vector3 (horizontalInput * moveSpeed, rig.velocity.y, 0);
			transform.right = rig.velocity.x > 0 ? Vector3.right : Vector3.left;
		}
		else
		{
			rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		}
	}

	void OnReceiveAttackInput ()
	{
		if (playerState == PlayerState.IDLE)
			ChangeState (PlayerState.TELEGRAPHING);
	}

	void OnReceiveDodgeInput ()
	{

	}

	private Coroutine telegraphCoroutine;
	void Telegraph ()
	{
		InterruptCoroutine (telegraphCoroutine, _Telegraph());
	}

	IEnumerator _Telegraph ()
	{
		rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		anim.Play ("Telegraph");
		yield return new WaitForSeconds (telegraphTime);
		ChangeState (PlayerState.ATTACKING);
	}

	private Coroutine attackCoroutine;
	void Attack ()
	{
		InterruptCoroutine (attackCoroutine, _Attack());
	}

	IEnumerator _Attack ()
	{
		float t = 0;
		rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		anim.Play ("Slash");

		while (t < attackTime)
		{
			t += Time.deltaTime;
			rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);

			yield return null;
		}

		rig.velocity = new Vector3 (0, rig.velocity.y, 0);

		yield return new WaitForSeconds (attackEndDelay);

		ChangeState (PlayerState.IDLE);
	}

	void InterruptCoroutine (Coroutine currentCoroutine, IEnumerator newCoroutine)
	{
		if (currentCoroutine == null)
		{
			currentCoroutine = StartCoroutine (newCoroutine);
		}
		else
		{
			StopCoroutine (currentCoroutine);
			currentCoroutine = StartCoroutine (newCoroutine);
		}
	}

	public void GotHit (Player otherPlayer)
	{
		ChangeState (PlayerState.HIT);
		rig.velocity = otherPlayer.rig.velocity;
	}
}
