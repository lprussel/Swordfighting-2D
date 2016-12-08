using UnityEngine;
using System.Collections;
using System;

public class PlayerManager : MonoBehaviour, IHittable
{
	public enum PlayerState
	{
		IDLE,
		BLOCKING,
		TELEGRAPHING,
		ATTACKING,
		RECOILING,
		DASHING,
		JUMPING,
		HIT,
		CANT_MOVE
	}
    public PlayerState playerState;

    public enum FacingDirection
    {
        LEFT,
        RIGHT
    }
    public FacingDirection facingDirection;

    private Rigidbody rig;

	private float moveSpeed = 8f;
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

	private MultiplayerInput input;

	public LayerMask standardMask;
	public LayerMask ignorePlayerMask;

	public bool grounded;
	private float groundedHeight = 0.5f;
	private float jumpSpeed = 30f;

	public PlayerManager opponent;

	private float playerGotHitTime = .5f;

	private float recoilTime = .75f;

	public Transform footTransform;

	private float reposteTimer;
	private float maxReposteTime = .25f;
	[HideInInspector]
	public bool canReposte;

	[HideInInspector]
	public int health = 5;
	public static int maxHealth = 5;

	public Action OnTelegraph = delegate { };
	public Action OnAttack = delegate { };
	public Action OnDash = delegate { };
	public Action OnRecoil = delegate { };
	public Action OnHit = delegate { };

	#pragma warning disable 0649
	private Coroutine telegraphCoroutine;
	private Coroutine attackCoroutine;
	private Coroutine dashCoroutine;
	private Coroutine hitCoroutine;
	private Coroutine recoilCoroutine;

	private Coroutine currentCombatCoroutine;
	#pragma warning restore 0649

	void Awake ()
	{
		rig = GetComponent<Rigidbody> ();
		input = GetComponent<MultiplayerInput> ();

		input.OnReceiveAttackInput += OnReceiveAttackInput;
		input.OnReceiveDodgeInput += OnReceiveDodgeInput;
		input.OnReceiveJumpInput += OnReceiveJumpInput;

		input.OnBlockInputEnter += OnBlockInputEnter;
		input.OnBlockInputExit += OnBlockInputExit;

		health = maxHealth;

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

	public Rigidbody GetRigidbody ()
	{
		return rig;
	}

	public void ChangeState (PlayerState newState)
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
			case PlayerState.RECOILING:
				Recoil (opponent);
				break;
			case PlayerState.DASHING:
				Dash ();
				break;
			case PlayerState.JUMPING:
				rig.velocity = new Vector3 (rig.velocity.x, jumpSpeed, rig.velocity.z);
                ChangeState(PlayerState.IDLE);
				break;
			case PlayerState.HIT:
				TakeDamage (opponent);
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
				break;
			case PlayerState.BLOCKING:
				if (reposteTimer < maxReposteTime)
				{
					reposteTimer += Time.deltaTime;
					canReposte = true;
				}
				else
					canReposte = false;
				
				rig.velocity = new Vector3 (Mathf.Lerp (rig.velocity.x, 0, Time.deltaTime * 10), rig.velocity.y, rig.velocity.z);
				break;
			case PlayerState.TELEGRAPHING:
				break;
			case PlayerState.ATTACKING:
				break;
			case PlayerState.RECOILING:
				rig.velocity = new Vector3 (Mathf.Lerp (rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
				break;
			case PlayerState.DASHING:
				break;
			case PlayerState.JUMPING:
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
        facingDirection = transform.right.x > 0 ? FacingDirection.RIGHT : FacingDirection.LEFT;

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

		if (Physics.Raycast (footTransform.position, -transform.up, out hit, groundedHeight, ignorePlayerMask, QueryTriggerInteraction.UseGlobal))
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
		{
			reposteTimer = 0;
			ChangeState (PlayerState.IDLE);
		}
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
		OnTelegraph ();
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

		OnAttack ();

		Vector3 initialPosition = transform.position;

		float distanceMult = CalculateMoveDistance (transform.right, attackDistance, standardMask);

		bool hitPlayerFlag = false;

		AudioManager.instance.PlaySound (AudioManager.SoundSet.SLASH);

		while (t < attackTime)
		{
			t += Time.fixedDeltaTime;
			//rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);
			transform.position = Vector3.Lerp (initialPosition, initialPosition + new Vector3 (transform.right.x * attackDistance * distanceMult, 0, 0), attackCurve.Evaluate (t / attackTime));
			if ((t / attackTime) > .5f && !hitPlayerFlag)
			{
				RaycastHit[] hits = Physics.RaycastAll (transform.position, transform.right, attackDistance, standardMask, QueryTriggerInteraction.UseGlobal);
				for (int i = 0; i < hits.Length; i++)
				{
					hitPlayerFlag = true;

					RaycastHit hit = hits [i];
					if ((hit.collider.tag == "Interactive" || hit.collider.tag == "Player") && hit.collider.gameObject != gameObject)
					{
						IHittable target = hit.collider.GetComponent<IHittable> ();
						target.GotHit (this);
					}
				}
			}

			yield return new WaitForFixedUpdate ();
		}

		yield return new WaitForSeconds (attackEndDelay);

		ChangeState (PlayerState.IDLE);
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

	public void Dash ()
	{
		InterruptCoroutine (dashCoroutine, _Dash ());
	}

	IEnumerator _Dash ()
	{
		float t = 0;
		rig.velocity = new Vector3 (0, 0, 0);

		OnDash ();

		Vector3 initialPosition = transform.position;

		rig.isKinematic = true;

		AudioManager.instance.PlaySound (AudioManager.SoundSet.DASH);

		moveDirection = input.controllerInput.x == 0 ? (transform.right.x > 0 ? -1 : 1) : (input.controllerInput.x > 0 ? 1 : -1);

		float distanceMult = CalculateMoveDistance (Vector3.right * moveDirection, dashDistance, ignorePlayerMask);

		while (t < dashTime)
		{
			t += Time.fixedDeltaTime;
			//rig.velocity = new Vector3 (transform.right.x * attackSpeed, rig.velocity.y, 0);
			transform.position = (Vector3.Lerp (initialPosition, initialPosition + new Vector3 (dashDistance * distanceMult * moveDirection, 0, 0), dashCurve.Evaluate (t / dashTime)));

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

	public void GotHit (PlayerManager attackingPlayer)
	{
		if (playerState == PlayerState.DASHING)
			return;

		bool wasBlocked = false;

		if (playerState == PlayerState.BLOCKING && transform.right.x > 0 && attackingPlayer.transform.position.x > transform.position.x ||
		    playerState == PlayerState.BLOCKING && transform.right.x < 0 && attackingPlayer.transform.position.x < transform.position.x)
		{
			wasBlocked = true;
		}

		if (!wasBlocked)
		{
			ChangeState (PlayerState.HIT);
			OnHit ();
			EffectsManager.instance.StartCoroutine (EffectsManager.instance.OnPlayerHit (attackingPlayer, this));
			AudioManager.instance.PlaySound (AudioManager.SoundSet.HIT);

			int mult = attackingPlayer.transform.position.x > transform.position.x ? -1 : 1;
			rig.velocity = new Vector3 (25 * mult, 0, 0);
		}
		else
		{
			if (canReposte)
			{
				EffectsManager.instance.StartCoroutine (EffectsManager.instance.OnPlayerBlock (attackingPlayer, this));
				AudioManager.instance.PlaySound (AudioManager.SoundSet.REPOSTE);

				attackingPlayer.ChangeState (PlayerState.RECOILING);
			}
			else
			{
				EffectsManager.instance.StartCoroutine (EffectsManager.instance.OnPlayerBlock (attackingPlayer, this));
				AudioManager.instance.PlaySound (AudioManager.SoundSet.BLOCK);

				int mult = attackingPlayer.transform.position.x > transform.position.x ? -1 : 1;
				rig.velocity = new Vector3 (25 * mult, 0, 0);
			}
		}
	}

	public void TakeDamage (PlayerManager otherPlayer)
	{
		InterruptCoroutine (hitCoroutine, _TakeDamage (otherPlayer));
	}

	private IEnumerator _TakeDamage (PlayerManager otherPlayer)
	{
		if (health > 1)
		{
			health--;

			yield return new WaitForSeconds (playerGotHitTime);

			ChangeState (PlayerState.IDLE);
		}
		else
		{
			health--;
			yield return null;
			ChangeState (PlayerState.CANT_MOVE);
			GameManager.instance.PlayerWon ();
		}
	}

	public void Recoil (PlayerManager otherPlayer)
	{
		InterruptCoroutine (recoilCoroutine, _Recoil (otherPlayer));
	}

	private IEnumerator _Recoil (PlayerManager otherPlayer)
	{
		OnRecoil ();
		int mult = otherPlayer.transform.position.x > transform.position.x ? -1 : 1;
		rig.velocity = new Vector3 (25 * mult, 0, 0);

		yield return new WaitForSeconds (recoilTime);

		ChangeState (PlayerState.IDLE);
	}
}
