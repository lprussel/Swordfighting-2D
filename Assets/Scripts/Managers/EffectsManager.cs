using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsManager : MonoBehaviour
{
	public static EffectsManager instance;

	public GameObject[] hitEffects;
	public GameObject[] blockEffects;

	void Awake ()
	{
		instance = this;
	}

	void Start()
	{
		for (int i = 0; i < hitEffects.Length; i++)
		{
			hitEffects [i].SetActive (false);
		}
		for (int i = 0; i < blockEffects.Length; i++)
		{
			blockEffects [i].SetActive (false);
		}
	}

	public IEnumerator OnPlayerHit (Transform attackingPlayer, Transform attackedPlayer)
	{
		GameObject newHitEffect = GetEffectInSet (hitEffects);

		float t = 0;
		float maxT = .25f;

		newHitEffect.SetActive (true);
		newHitEffect.transform.right = attackingPlayer.transform.right;
		newHitEffect.transform.position = attackedPlayer.transform.position;
		newHitEffect.GetComponent<Animation> ().Play ("PlayerHit");

		while (t < maxT)
		{
			t += Time.deltaTime;
			newHitEffect.transform.position = attackedPlayer.transform.position;
			yield return null;
		}

		newHitEffect.SetActive (false);
	}

	public IEnumerator OnPlayerBlock (Transform attackedPlayer)
	{
		GameObject newBlockEffect = GetEffectInSet (blockEffects);

		float t = 0;
		float maxT = .25f;

		newBlockEffect.SetActive (true);
		newBlockEffect.transform.right = (attackedPlayer.transform.right + new Vector3(Random.Range(-.25f, .25f), Random.Range(0, 1f), Random.Range(-.25f, .25f))).normalized;
		newBlockEffect.GetComponent<Animation> ().Play ("PlayerBlock");

		while (t < maxT)
		{
			t += Time.deltaTime;
			newBlockEffect.transform.position = attackedPlayer.transform.position;
			yield return null;
		}

		newBlockEffect.SetActive (false);
	}

	private GameObject GetEffectInSet (GameObject[] objectSet)
	{
		for (int i = 0; i < objectSet.Length; i++)
		{
			if (!objectSet [i].activeInHierarchy)
			{
				return objectSet [i];
			}
		}
		return null;
	}
}
