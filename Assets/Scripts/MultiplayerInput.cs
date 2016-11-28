using UnityEngine;
using System.Collections;
using System;

public class MultiplayerInput : MonoBehaviour
{
	public int player;
	[HideInInspector]
	public Vector2 controllerInput;

	private Vector3 mouseVector;
	public Vector3 mousePosition;

	public Action OnReceiveAttackInput = delegate { };
	public Action OnReceiveDodgeInput = delegate { };
	public Action OnBlockInputEnter = delegate { };
	public Action OnBlockInputExit = delegate { };
	public Action OnReceiveJumpInput = delegate { };

	private Plane gamePlane;

	void Start ()
	{
		gamePlane.SetNormalAndPosition (-Vector3.forward, Vector3.zero);
	}

	void Update ()
	{
		if (player == 0)
		{
			controllerInput.x = Input.GetAxisRaw ("Horizontal");
			controllerInput.y = Input.GetAxisRaw ("Vertical");

			Vector3 cameraPosition = GameCamera.instance.transform.position;

			mouseVector = GameCamera.instance.thisCamera.ScreenToWorldPoint
				(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, GameCamera.instance.thisCamera.nearClipPlane));

			mouseVector = mouseVector - cameraPosition;

			MathUtilities.LinePlaneIntersection (cameraPosition, mouseVector, Vector3.zero, -Vector3.forward, out mousePosition);

			if (Input.GetKeyDown (KeyCode.L))
			{
				OnReceiveDodgeInput ();
			}
			if (Input.GetKeyDown (KeyCode.J))
			{
				OnReceiveAttackInput ();
			}
			if (Input.GetKeyDown (KeyCode.K))
			{
				OnBlockInputEnter ();
			}
			if (Input.GetKeyUp (KeyCode.K))
			{
				OnBlockInputExit ();
			}
			if (Input.GetKeyDown (KeyCode.Space))
			{
				OnReceiveJumpInput ();
			}
		}
	}
}
