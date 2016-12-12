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
		DODGING,
        PATHING
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
		if (!initialized || aiState != AI_State.FOLLOWING && aiState != AI_State.PATHING)
			return;
		
		vectorToOpponent = MathUtilities.FlattenVector (opponent.transform.position, MathUtilities.Axis.Y) - MathUtilities.FlattenVector (transform.position, MathUtilities.Axis.Y);

		EmulateInput ();
	}

	void EmulateInput ()
	{
        if (aiState == AI_State.PATHING)
            return;

		if (vectorToOpponent.x > 0)
			multiplayerInput.mousePosition.x = transform.position.x + 5;
		else if (vectorToOpponent.x < 0)
			multiplayerInput.mousePosition.x = transform.position.x - 5;

        if (opponent.transform.position.y > transform.position.y + .5f || opponent.transform.position.y < transform.position.y - .5f)
        {
            if (opponent.transform.position.y > transform.position.y + .5f)
            {
                if (vectorToOpponent.x > -jumpRange && vectorToOpponent.x < jumpRange)
                {
                    if (playerManager.grounded)
                    {
                        multiplayerInput.OnReceiveJumpInput();
                    }
                    else
                    {
                        if (vectorToOpponent.x >= attackRange)
                        {
                            multiplayerInput.controllerInput.x = 1;
                        }
                        else if (vectorToOpponent.x <= -attackRange)
                        {
                            multiplayerInput.controllerInput.x = -1;
                        }
                    }
                }
                else
                {
                    if (pathingCoroutine != null)
                    {
                        StopCoroutine(pathingCoroutine);
                    }
                    pathingCoroutine = StartCoroutine(TryToFindALedge());
                }
            }
            else
            {
                if (pathingCoroutine != null)
                {
                    StopCoroutine(pathingCoroutine);
                }
                pathingCoroutine = StartCoroutine(TryToFall());
            }
        }
        else
        {
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

                int rand = Random.Range(0, 5);
                if (rand == 0 || rand == 1 || rand == 2)
                {
                    StartCoroutine(AttackSomeShit());
                }
                else if (rand == 3)
                {
                    multiplayerInput.OnReceiveDodgeInput();
                }
                else if (rand == 4)
                {
                    StartCoroutine(BlockSomeShit());
                }
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

    Coroutine pathingCoroutine;
    IEnumerator TryToFindALedge ()
    {
        aiState = AI_State.PATHING;

        float maxT = Random.Range(0.5f, 2.0f);
        float t = 0;

        float counter = 0.0f;

        int moveDir = (vectorToOpponent.x > 0) ? 1 : -1;

        while (!CheckAbove() && opponent.transform.position.y > transform.position.y + .5f)
        {
            if (t > maxT)
            {
                t = 0;
                moveDir = (vectorToOpponent.x > 0) ? 1 : -1;
                if (counter < 2)
                    counter += 1;
                maxT = Random.Range(0.5f, 1 + counter);
            }
            if (playerManager.grounded && CheckAboutToFall())
                moveDir = (vectorToOpponent.x > 0) ? 1 : -1;

            t += Time.deltaTime;

            multiplayerInput.controllerInput.x = moveDir;
            yield return null;
        }

        aiState = AI_State.FOLLOWING;

        if (opponent.transform.position.y > transform.position.y + .5f)
            multiplayerInput.OnReceiveJumpInput();
    }

    bool CheckBelow ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 2f, playerManager.ignorePlayerMask, QueryTriggerInteraction.UseGlobal))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckAbove ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 10, -Vector3.up, out hit, 10f, playerManager.ignorePlayerMask, QueryTriggerInteraction.UseGlobal))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckAboutToFall ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (playerManager.GetRigidbody().velocity + Vector3.up * -5).normalized, out hit, 5f, playerManager.ignorePlayerMask, QueryTriggerInteraction.UseGlobal))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    IEnumerator TryToFall ()
    {
        aiState = AI_State.PATHING;

        float maxT = Random.Range(0.5f, 2.0f);
        float t = 0;

        float counter = 0.0f;

        int moveDir = (vectorToOpponent.x > 0) ? 1 : -1;

        while (opponent.transform.position.y < transform.position.y - .5f)
        {
            if (t > maxT)
            {
                t = 0;
                moveDir = (vectorToOpponent.x > 0) ? 1 : -1;
                if (counter < 2)
                    counter += 1;
                maxT = Random.Range(0.5f, 1 + counter);
            }

            if (CheckAboutToFall())
                maxT += 1f;

            if (playerManager.grounded == false)
                break;

            t += Time.deltaTime;

            multiplayerInput.controllerInput.x = moveDir;
            yield return null;
        }

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
