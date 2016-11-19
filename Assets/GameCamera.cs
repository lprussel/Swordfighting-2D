using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public Transform[] players;
	public Vector3 offset;

	void Start ()
	{
		
	}

	void Update ()
	{
		Vector3 centerPos = Vector3.zero;
		for (int i = 0; i < players.Length; i++) {
			centerPos += players [i].transform.position;
		}
		centerPos /= players.Length;

		centerPos.z = 0;

		transform.position = centerPos + offset;
	}
}
