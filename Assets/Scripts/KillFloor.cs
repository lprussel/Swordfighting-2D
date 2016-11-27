using UnityEngine;
using System.Collections;

public class KillFloor : MonoBehaviour
{

	void Start ()
	{
	
	}

	void Update ()
	{
	
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Player")
		{
			col.GetComponent<Player> ().ChangeState (Player.PlayerState.CANT_MOVE);
			GameManager.instance.PlayerWon ();
		}
	}
}
