using UnityEngine;
using System.Collections;

public class HittableThing : MonoBehaviour, IHittable
{
	public void GotHit (PlayerManager otherPlayer)
	{
		Destroy (gameObject);
	}
}
