using UnityEngine;
using System.Collections;
using System;

public class MultiplayerInput : MonoBehaviour
{
	public int player;
	[HideInInspector]
	public Vector2 controllerInput;

	public Action OnReceiveAttackInput = delegate { };
	public Action OnReceiveDodgeInput = delegate { };

	void Update ()
	{
		if (player == 0)
		{
			controllerInput.x = Input.GetAxisRaw ("Horizontal");
			controllerInput.y = Input.GetAxisRaw ("Vertical");

			if (Input.GetKeyDown (KeyCode.Space))
			{
				OnReceiveDodgeInput ();
			}
			if (Input.GetMouseButtonDown (0))
			{
				OnReceiveAttackInput ();
			}
		}
	}
}
