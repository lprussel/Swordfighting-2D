using UnityEngine;
using System.Collections;
using PlayerPt2;

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
			col.GetComponent<Player>().m_StateMachine.SetStateFromID(StateID.Dead);
			GameManager.PlayerWon ();
		}
	}
}
