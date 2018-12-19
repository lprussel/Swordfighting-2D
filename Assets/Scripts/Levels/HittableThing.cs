using UnityEngine;
using System.Collections;

public class HittableThing : MonoBehaviour, IHittable
{
	public void GotHit (int amount, Transform otherPlayer)
	{
		Destroy (gameObject);
	}
}
