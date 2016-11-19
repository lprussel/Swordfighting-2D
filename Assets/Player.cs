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
	private float attackSpeed = 200f;

	private float attackTime = .05f;
	private float telegraphTime = .25f;
	private float attackEndDelay = .1f;

	private float horizontalInput;

	private bool shouldDashAttack;

	//public Transform opponent;

	private Animation anim;

	void Start ()
	{
		rig = GetComponent<Rigidbody> ();
		anim = GetComponent<Animation> ();
		ChangeState (PlayerState.IDLE);
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
		horizontalInput = Input.GetAxisRaw ("Horizontal");
		if (Mathf.Abs (horizontalInput) > 0)
		{
			rig.velocity = new Vector3 (horizontalInput * moveSpeed, rig.velocity.y, 0);
		}
		else
		{
			rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		}

		//transform.right = (transform.position.x > opponent.position.x) ? Vector3.left : Vector3.right;

		if (Input.GetMouseButtonDown (0))
		{
			if (Mathf.Abs (horizontalInput) > 0)
				shouldDashAttack = true;
			ChangeState (PlayerState.TELEGRAPHING);
		}
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
			if (shouldDashAttack)
				rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);

			yield return null;
		}
		rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		yield return new WaitForSeconds (attackEndDelay);
		shouldDashAttack = false;
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
}
