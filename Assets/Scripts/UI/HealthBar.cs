using UnityEngine;
using System.Collections;
using PlayerPt2;

public class HealthBar : MonoBehaviour
{
	public Player player;
	private int currentDisplay;

	public Transform barTransform;

	void Update ()
	{
		if (player.m_Control.Health.m_Health != currentDisplay)
		{
			UpdateDisplay ();
		}
	}

	void UpdateDisplay ()
	{
		currentDisplay = player.m_Control.Health.m_Health;

		barTransform.localPosition = new Vector3 ((((float)currentDisplay / PlayerHealth.MaxHealth) / 2) - .5f, 0, -1);
		barTransform.localScale = new Vector3 (((float)currentDisplay / PlayerHealth.MaxHealth), 1, 1);
	}
}
