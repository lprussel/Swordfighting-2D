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
			GameManager.instance.PlayerWon ();
		}
	}
}
