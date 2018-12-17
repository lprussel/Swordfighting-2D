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
			col.GetComponent<PlayerManager> ().ChangeState (PlayerManager.PlayerState.CANT_MOVE);
			GameManager.PlayerWon ();
		}
	}
}
