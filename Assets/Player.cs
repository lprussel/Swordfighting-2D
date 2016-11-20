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

	//private float attackSpeed = 100f;
	private float attackDistance = 5f;
	private float attackTime = .01f;

	private float telegraphTime = .15f;
	private float attackEndDelay = .1f;

	private float horizontalInput;

	private Animation anim;

	private MultiplayerInput input;

	public LayerMask mask;

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
		input.OnReceiveAttackInput -= OnReceiveAttackInput;
		input.OnReceiveDodgeInput -= OnReceiveDodgeInput;
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
				rig.velocity = new Vector3 (Mathf.Lerp (rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
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
		InterruptCoroutine (telegraphCoroutine, _Telegraph ());
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
		InterruptCoroutine (attackCoroutine, _Attack ());
	}

	IEnumerator _Attack ()
	{
		float t = 0;
		rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		anim.Play ("Slash");

		Vector3 initialPosition = transform.position;

		float distanceMult = CalculateAttackDistance ();

		bool hitPlayerFlag = false;

		while (t < attackTime)
		{
			t += Time.fixedDeltaTime;
			//rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);
			rig.MovePosition (Vector3.Lerp (initialPosition, initialPosition + new Vector3 (transform.right.x * attackDistance * distanceMult, 0, 0), t / attackTime));
			if ((t / attackTime) > .5f && !hitPlayerFlag)
			{
				RaycastHit[] hits = Physics.RaycastAll (transform.position, transform.right, attackDistance, mask, QueryTriggerInteraction.UseGlobal);
				for (int i = 0; i < hits.Length; i++)
				{
					hitPlayerFlag = true;

					RaycastHit hit = hits [i];
					if (hit.collider.tag == "Player" && hit.collider.gameObject != gameObject)
						hit.collider.GetComponent<Player> ().GotHit (this);
				}
			}

			yield return new WaitForFixedUpdate ();
		}

		rig.velocity = new Vector3 (0, rig.velocity.y, 0);

		yield return new WaitForSeconds (attackEndDelay);

		ChangeState (PlayerState.IDLE);
	}

	float CalculateAttackDistance ()
	{
		float acceptableDistance;
		RaycastHit hit;

		if (Physics.Raycast (transform.position, transform.right, out hit, attackDistance, mask, QueryTriggerInteraction.UseGlobal))
		{
			acceptableDistance = (hit.point - transform.position).magnitude / attackDistance;
		}
		else
		{
			acceptableDistance = 1;
		}

		return acceptableDistance;
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
		Renderer[] rends = GetComponentsInChildren<Renderer> ();

		for (int i = 0; i < rends.Length; i++)
		{
			rends [i].material.color = Color.red;
		}

		ChangeState (PlayerState.HIT);
		rig.velocity = new Vector3 (15, 0, 0);
	}
}
