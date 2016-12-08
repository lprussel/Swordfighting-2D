using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour
{
	public string noPlayerLayer;
	public string player1Layer;
	public string player2Layer;

	public PlayerManager player1;
	public PlayerManager player2;

	public Transform floorTransform;

	public bool player1Above;
	public bool player2Above;

	private bool changingLayer;

	void Start ()
	{
		gameObject.layer = LayerMask.NameToLayer (noPlayerLayer);
	}

	void Update ()
	{
		int newLayer = 0;

		player1Above = false;
		player2Above = false;

		player1Above = player1.footTransform.position.y >= floorTransform.position.y - .25f;
		player2Above = player2.footTransform.position.y >= floorTransform.position.y - .25f;

		bool delayChange = false;

		if (!player1Above && !player2Above)
		{
			newLayer = LayerMask.NameToLayer (noPlayerLayer);
			delayChange = true;
		}
		else if (player1Above && !player2Above)
		{
			newLayer = LayerMask.NameToLayer (player1Layer);
			delayChange = false;
		}
		else if (!player1Above && player2Above)
		{
			newLayer = LayerMask.NameToLayer (player2Layer);
			delayChange = false;
		}
		else if (player1Above && player2Above)
		{
			newLayer = LayerMask.NameToLayer ("Default");
			delayChange = false;
		}

		if (gameObject.layer != newLayer && !changingLayer)
		{
			StartCoroutine (ChangeLayer (newLayer, delayChange ? .1f : 0f));
		}
	}

	IEnumerator ChangeLayer (int newLayer, float length)
	{
		changingLayer = true;

		yield return new WaitForSeconds(length);

		gameObject.layer = newLayer;

		changingLayer = false;
	}
}
