using UnityEngine;
using System.Collections;

public class InputEmulator : MonoBehaviour
{
	private MultiplayerInput multiplayerInput;
	private PlayerManager playerManager;

	private PlayerManager opponent;
	private Vector3 vectorToOpponent;

	private float attackRange = 5.0f;

	private float jumpRange = 10.0f;

	public enum AI_State
	{
		FOLLOWING,
		BLOCKING,
		ATTACKING,
		DODGING
	}
	public AI_State aiState;

    private bool initialized;

	public void Initialize (PlayerManager playerManager, MultiplayerInput multiplayerInput, PlayerManager opponent)
	{
		this.multiplayerInput = multiplayerInput;
        this.playerManager = playerManager;
        this.opponent = opponent;

        initialized = true;
	}

	void Update ()
	{
		if (!initialized || aiState != AI_State.FOLLOWING)
			return;
		
		vectorToOpponent = MathUtilities.FlattenVector (opponent.transform.position, MathUtilities.Axis.Y) - MathUtilities.FlattenVector (transform.position, MathUtilities.Axis.Y);

		EmulateInput ();
	}

	void EmulateInput ()
	{
		if (vectorToOpponent.x > 0)
			multiplayerInput.mousePosition.x = transform.position.x + 5;
		else if (vectorToOpponent.x < 0)
			multiplayerInput.mousePosition.x = transform.position.x - 5;

		if (vectorToOpponent.x > -jumpRange && vectorToOpponent.x < jumpRange)
		{
			if (opponent.transform.position.y > transform.position.y + .25f)
			{
				multiplayerInput.OnReceiveJumpInput ();
			}
		}

		if (vectorToOpponent.x >= attackRange)
		{
			multiplayerInput.controllerInput.x = 1;
		}
		else if (vectorToOpponent.x <= -attackRange)
		{
			multiplayerInput.controllerInput.x = -1;
		}
		else if (vectorToOpponent.x < attackRange && vectorToOpponent.x > -attackRange)
		{
			multiplayerInput.controllerInput.x = 0;

			if (playerManager.playerState != PlayerManager.PlayerState.IDLE)
				return;

			int rand = Random.Range (0, 5);
			if (rand == 0 || rand == 1 || rand == 2)
			{
				StartCoroutine (AttackSomeShit ());
			}
			else if (rand == 3)
			{
				multiplayerInput.OnReceiveDodgeInput ();
			}
			else if (rand == 4)
			{
				StartCoroutine (BlockSomeShit ());
			}
		}
	}

	IEnumerator BlockSomeShit ()
	{
		aiState = AI_State.BLOCKING;

		float blockTime = 0;
		float maxBlockTime = 2;

		while (Mathf.Abs(vectorToOpponent.x) < attackRange * 2 && blockTime < maxBlockTime)
		{
			multiplayerInput.OnBlockInputEnter ();

			blockTime += Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds (Random.Range(.25f, 1f));

		multiplayerInput.OnBlockInputExit ();

		aiState = AI_State.FOLLOWING;
	}

	IEnumerator AttackSomeShit ()
	{
		aiState = AI_State.ATTACKING;

		multiplayerInput.OnReceiveAttackInput ();

		yield return new WaitForSeconds (Random.Range(.25f, 1f));

		aiState = AI_State.FOLLOWING;
	}

	IEnumerator DodgeSomeShit ()
	{
		aiState = AI_State.DODGING;

		multiplayerInput.OnReceiveDodgeInput ();

		yield return new WaitForSeconds (Random.Range(.5f, 1f));

		aiState = AI_State.FOLLOWING;
	}
}
