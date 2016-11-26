using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public enum PlayerState
	{
		IDLE,
		BLOCKING,
		TELEGRAPHING,
		ATTACKING,
		DASHING,
		JUMPING,
		HIT,
		CANT_MOVE
	}

	public PlayerState playerState;

	private Rigidbody rig;

	private float moveSpeed = 3f;
	private int moveDirection;

	//private float attackSpeed = 100f;
	private float attackDistance = 5f;
	private float attackTime = .05f;

	public AnimationCurve attackCurve;

	private float telegraphTime = .35f;
	private float attackEndDelay = .1f;

	public AnimationCurve dashCurve;

	private float dashDistance = 5f;
	private float dashTime = .15f;
	private float dashEndDelay = .01f;

	private float horizontalInput;

	private Animation anim;

	private MultiplayerInput input;

	public LayerMask standardMask;
	public LayerMask ignorePlayerMask;

	[HideInInspector]
	public bool grounded;
	private float height = 1.5f;
	private float jumpSpeed = 30f;

	public Player opponent;

	private float playerGotHitTime = .5f;

	public Transform footTransform;

	#pragma warning disable 0649
	private Coroutine telegraphCoroutine;
	private Coroutine attackCoroutine;
	private Coroutine dashCoroutine;
	private Coroutine hitCoroutine;

	private Coroutine currentCombatCoroutine;
	#pragma warning restore 0649

	void Start ()
	{
		rig = GetComponent<Rigidbody> ();
		anim = GetComponent<Animation> ();
		input = GetComponent<MultiplayerInput> ();

		input.OnReceiveAttackInput += OnReceiveAttackInput;
		input.OnReceiveDodgeInput += OnReceiveDodgeInput;
		input.OnReceiveJumpInput += OnReceiveJumpInput;

		input.OnBlockInputEnter += OnBlockInputEnter;
		input.OnBlockInputExit += OnBlockInputExit;

		ChangeState (PlayerState.IDLE);
	}

	void OnDestroy ()
	{
		input.OnReceiveAttackInput -= OnReceiveAttackInput;
		input.OnReceiveDodgeInput -= OnReceiveDodgeInput;
		input.OnReceiveJumpInput -= OnReceiveJumpInput;

		input.OnBlockInputEnter -= OnBlockInputEnter;
		input.OnBlockInputExit -= OnBlockInputExit;
	}

	void ChangeState (PlayerState newState)
	{
		if (newState == playerState)
			return;

		playerState = newState;

		switch (newState)
		{
			case PlayerState.IDLE:
				break;
			case PlayerState.BLOCKING:
				break;
			case PlayerState.TELEGRAPHING:
				Telegraph ();
				break;
			case PlayerState.ATTACKING:
				Attack ();
				break;
			case PlayerState.DASHING:
				Dash ();
				break;
			case PlayerState.JUMPING:
				rig.velocity = new Vector3 (rig.velocity.x, jumpSpeed, rig.velocity.z);
				break;
			case PlayerState.HIT:
				GotHit (opponent);
				break;
			case PlayerState.CANT_MOVE:
				break;
		}
	}

	void Update ()
	{
		switch (playerState)
		{
			case PlayerState.IDLE:
				HandleMovement ();
				anim.CrossFade ("Idle");
				break;
			case PlayerState.BLOCKING:
				rig.velocity = new Vector3 (Mathf.Lerp (rig.velocity.x, 0, Time.deltaTime * 10), rig.velocity.y, rig.velocity.z);
				anim.CrossFade ("Block", .1f);
				break;
			case PlayerState.TELEGRAPHING:
				break;
			case PlayerState.ATTACKING:
				break;
			case PlayerState.DASHING:
				break;
			case PlayerState.JUMPING:
				HandleMovement ();
				break;
			case PlayerState.HIT:
				rig.velocity = new Vector3 (Mathf.Lerp (rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
				break;
			case PlayerState.CANT_MOVE:
				rig.velocity = new Vector3 (Mathf.Lerp (rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
				break;
		}
	}

	void HandleMovement ()
	{
		horizontalInput = input.controllerInput.x;
		//transform.right = input.mousePosition.x > transform.position.x ? Vector3.right : Vector3.left;
		transform.right = opponent.transform.position.x > transform.position.x ? Vector3.right : Vector3.left;

		CheckGrounded ();

		if (Mathf.Abs (horizontalInput) > .05)
		{
			rig.velocity = new Vector3 (horizontalInput * moveSpeed, rig.velocity.y, 0);
		}
		else
		{
			rig.velocity = new Vector3 (0, rig.velocity.y, 0);
		}
	}

	void CheckGrounded ()
	{
		RaycastHit hit;

		if (Physics.Raycast (transform.position, -transform.up, out hit, height, ignorePlayerMask, QueryTriggerInteraction.UseGlobal))
		{
			grounded = true;
			if (playerState == PlayerState.JUMPING)
				ChangeState (PlayerState.IDLE);
		}
		else
			grounded = false;
	}

	void OnReceiveAttackInput ()
	{
		if (playerState == PlayerState.IDLE || playerState == PlayerState.JUMPING)
			ChangeState (PlayerState.TELEGRAPHING);
	}

	void OnReceiveDodgeInput ()
	{
		if (playerState == PlayerState.IDLE || playerState == PlayerState.JUMPING)
			ChangeState (PlayerState.DASHING);
	}

	void OnBlockInputEnter ()
	{
		if (playerState == PlayerState.IDLE || playerState == PlayerState.JUMPING)
			ChangeState (PlayerState.BLOCKING);
	}

	void OnBlockInputExit ()
	{
		if (playerState == PlayerState.BLOCKING)
			ChangeState (PlayerState.IDLE);
	}

	void OnReceiveJumpInput ()
	{
		if (playerState == PlayerState.IDLE && grounded)
			ChangeState (PlayerState.JUMPING);
	}

	void Telegraph ()
	{
		InterruptCoroutine (telegraphCoroutine, _Telegraph ());
	}

	IEnumerator _Telegraph ()
	{
		rig.useGravity = false;
		rig.velocity = new Vector3 (0, 0, 0);
		anim.Play ("Telegraph");
		yield return new WaitForSeconds (telegraphTime);
		ChangeState (PlayerState.ATTACKING);
	}

	void Attack ()
	{
		InterruptCoroutine (attackCoroutine, _Attack ());
	}

	IEnumerator _Attack ()
	{
		float t = 0;
		rig.velocity = new Vector3 (0, 0, 0);
		rig.useGravity = true;

		string attackName = GetAttackName();
		anim.Play (attackName);

		Vector3 initialPosition = transform.position;

		float distanceMult = CalculateMoveDistance (transform.right, attackDistance, standardMask);

		bool hitPlayerFlag = false;

		AudioManager.instance.PlaySound (AudioManager.SoundSet.SLASH);

		while (t < attackTime)
		{
			t += Time.fixedDeltaTime;
			//rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);
			transform.position = Vector3.Lerp (initialPosition, initialPosition + new Vector3 (transform.right.x * attackDistance * distanceMult, 0, 0), attackCurve.Evaluate(t / attackTime));
			if ((t / attackTime) > .5f && !hitPlayerFlag)
			{
				RaycastHit[] hits = Physics.RaycastAll (transform.position, transform.right, attackDistance, standardMask, QueryTriggerInteraction.UseGlobal);
				for (int i = 0; i < hits.Length; i++)
				{
					hitPlayerFlag = true;

					RaycastHit hit = hits [i];
					if (hit.collider.tag == "Player" && hit.collider.gameObject != gameObject)
					{
						Player target = hit.collider.GetComponent<Player> ();
						target.CheckHit (this);
					}
				}
			}

			yield return new WaitForFixedUpdate ();
		}

		yield return new WaitForSeconds (attackEndDelay);

		ChangeState (PlayerState.IDLE);
	}

	string GetAttackName ()
	{
		int rand = Random.Range (0, 3) + 1;
		string name = "Slash" + rand.ToString ();
		return name;
	}

	float CalculateMoveDistance (Vector3 direction, float moveDistance, LayerMask mask)
	{
		float acceptableDistance;
		RaycastHit hit;

		if (Physics.Raycast (transform.position, direction.normalized, out hit, moveDistance, mask, QueryTriggerInteraction.UseGlobal))
		{
			acceptableDistance = (hit.point - transform.position).magnitude / attackDistance;
		}
		else
		{
			acceptableDistance = 1;
		}

		return acceptableDistance;
	}

	public void Dash()
	{
		InterruptCoroutine (dashCoroutine, _Dash ());
	}

	IEnumerator _Dash()
	{
		float t = 0;
		rig.velocity = new Vector3 (0, 0, 0);

		Vector3 initialPosition = transform.position;

		rig.isKinematic = true;

		AudioManager.instance.PlaySound (AudioManager.SoundSet.DASH);

		moveDirection = input.controllerInput.x == 0 ? (transform.right.x > 0 ? -1 : 1) : (input.controllerInput.x > 0 ? 1 : -1);

		float distanceMult = CalculateMoveDistance (Vector3.right * moveDirection, dashDistance, ignorePlayerMask);

		while (t < dashTime)
		{
			t += Time.fixedDeltaTime;
			//rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);
			transform.position = (Vector3.Lerp (initialPosition, initialPosition + new Vector3 (dashDistance * distanceMult * moveDirection, 0, 0), dashCurve.Evaluate(t / dashTime)));

			yield return new WaitForFixedUpdate ();
		}

		rig.velocity = new Vector3 (0, rig.velocity.y, 0);

		rig.isKinematic = false;

		yield return new WaitForSeconds (dashEndDelay);
		
		ChangeState (PlayerState.IDLE);
	}

	void InterruptCoroutine (Coroutine currentCoroutine, IEnumerator newCoroutine)
	{
		rig.useGravity = true;

		if (currentCombatCoroutine != null)
		{
			StopCoroutine (currentCombatCoroutine);
		}

		if (currentCoroutine == null)
		{
			currentCoroutine = StartCoroutine (newCoroutine);
		}
		else
		{
			StopCoroutine (currentCoroutine);
			currentCoroutine = StartCoroutine (newCoroutine);
		}

		currentCombatCoroutine = currentCoroutine;
	}

	public void CheckHit (Player otherPlayer)
	{
		if (playerState == PlayerState.DASHING)
			return;

		bool wasBlocked = false;

		if (playerState == PlayerState.BLOCKING && transform.right.x > 0 && otherPlayer.transform.position.x > transform.position.x ||
			playerState == PlayerState.BLOCKING && transform.right.x < 0 && otherPlayer.transform.position.x < transform.position.x)
		{
			wasBlocked = true;
		}

		if (!wasBlocked)
		{
			ChangeState (PlayerState.HIT);
			anim.Play ("GotHit");
			EffectsManager.instance.StartCoroutine (EffectsManager.instance.OnPlayerHit (otherPlayer, this));
			AudioManager.instance.PlaySound (AudioManager.SoundSet.HIT);
		}
		else
		{
			EffectsManager.instance.StartCoroutine (EffectsManager.instance.OnPlayerBlock (otherPlayer, this));
			AudioManager.instance.PlaySound (AudioManager.SoundSet.BLOCK);
		}

		int mult = otherPlayer.transform.position.x > transform.position.x ? -1 : 1;
		rig.velocity = new Vector3 (25 * mult, 0, 0);
	}

	public void GotHit (Player otherPlayer)
	{
		InterruptCoroutine (hitCoroutine, _GotHit (otherPlayer));
	}

	private IEnumerator _GotHit (Player otherPlayer)
	{
		yield return new WaitForSeconds (playerGotHitTime);

		ChangeState (PlayerState.IDLE);
	}
}
