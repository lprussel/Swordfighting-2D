using UnityEngine;
using System.Collections;
using PlayerPt2;

public interface IHittable
{
	void GotHit (int amount, Transform attackingPlayer);
}