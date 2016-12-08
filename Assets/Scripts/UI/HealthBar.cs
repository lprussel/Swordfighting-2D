using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
	public PlayerManager player;
	private int currentDisplay;

	public Transform barTransform;

	void Update ()
	{
		if (player.health != currentDisplay)
		{
			UpdateDisplay ();
		}
	}

	void UpdateDisplay ()
	{
		currentDisplay = player.health;

		barTransform.localPosition = new Vector3 ((((float)currentDisplay / PlayerManager.maxHealth) / 2) - .5f, 0, -1);
		barTransform.localScale = new Vector3 (((float)currentDisplay / PlayerManager.maxHealth), 1, 1);
	}
}
