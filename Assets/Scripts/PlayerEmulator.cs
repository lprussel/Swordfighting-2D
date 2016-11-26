using UnityEngine;
using System.Collections;

public class PlayerEmulator : MonoBehaviour
{
	private MultiplayerInput thisInput;
	private Player thisPlayer;

	public Player opponent;
	private Vector3 vectorToOpponent;

	private float attackRange = 5.0f;

	//private float jumpRange = 4.0f;

	public enum AI_State
	{
		FOLLOWING,
		BLOCKING,
		ATTACKING,
		DODGING
	}
	public AI_State aiState;

	void Start ()
	{
		thisInput = GetComponent<MultiplayerInput> ();
		thisPlayer = GetComponent<Player> ();
	}

	void Update ()
	{
		if (aiState != AI_State.FOLLOWING)
			return;
		
		vectorToOpponent = MathUtilities.FlattenVector (opponent.transform.position, MathUtilities.Axis.Y) - MathUtilities.FlattenVector (transform.position, MathUtilities.Axis.Y);

		EmulateInput ();
	}

	void EmulateInput ()
	{
		if (!thisPlayer.grounded)
			return;

		if (vectorToOpponent.x > 0)
			thisInput.mousePosition.x = transform.position.x + 5;
		else if (vectorToOpponent.x < 0)
			thisInput.mousePosition.x = transform.position.x - 5;

		if (vectorToOpponent.x >= attackRange)
		{
			thisInput.controllerInput.x = 1;
		}
		else if (vectorToOpponent.x <= -attackRange)
		{
			thisInput.controllerInput.x = -1;
		}
		else if (vectorToOpponent.x < attackRange && vectorToOpponent.x > -attackRange)
		{
			thisInput.controllerInput.x = 0;

			if (thisPlayer.playerState != Player.PlayerState.IDLE)
				return;

			int rand = Random.Range (0, 5);
			if (rand == 0 || rand == 1 || rand == 2)
			{
				StartCoroutine (AttackSomeShit ());
			}
			else if (rand == 3)
			{
				thisInput.OnReceiveDodgeInput ();
			}
			else if (rand == 4)
			{
				StartCoroutine (BlockSomeShit ());
			}
		}

		//if (vectorToOpponent.x > -jumpRange && vectorToOpponent.x < jumpRange)
		//{
		//	if (opponent.transform.position.y > transform.position.y + 1f)
		//	{
		//		thisInput.OnReceiveJumpInput ();
		//	}
		//}
	}

	IEnumerator BlockSomeShit ()
	{
		aiState = AI_State.BLOCKING;

		float blockTime = 0;
		float maxBlockTime = 2;

		while (Mathf.Abs(vectorToOpponent.x) < attackRange * 2 && blockTime < maxBlockTime)
		{
			thisInput.OnBlockInputEnter ();

			blockTime += Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds (Random.Range(.25f, 1f));

		thisInput.OnBlockInputExit ();

		aiState = AI_State.FOLLOWING;
	}

	IEnumerator AttackSomeShit ()
	{
		aiState = AI_State.ATTACKING;

		thisInput.OnReceiveAttackInput ();

		yield return new WaitForSeconds (Random.Range(.25f, 1f));

		aiState = AI_State.FOLLOWING;
	}

	IEnumerator DodgeSomeShit ()
	{
		aiState = AI_State.DODGING;

		thisInput.OnReceiveDodgeInput ();

		yield return new WaitForSeconds (Random.Range(.5f, 1f));

		aiState = AI_State.FOLLOWING;
	}
}
