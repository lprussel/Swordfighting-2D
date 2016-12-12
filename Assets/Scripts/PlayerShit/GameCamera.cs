using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public Transform[] players;
	public Vector3 offset;
	public static GameCamera instance;
	[HideInInspector]
	public Camera thisCamera;

	private float minZoom = -10f;
	private float zoom;

	void Awake ()
	{
		instance = this;
		thisCamera = GetComponent<Camera> ();
		zoom = minZoom + -(Vector3.Distance(players[0].transform.position, players[1].transform.position) / 2);
	}

	void LateUpdate ()
	{
		Vector3 centerPos = Vector3.zero;

		zoom = Mathf.Lerp(zoom, minZoom + -(Vector3.Distance(players[0].transform.position, players[1].transform.position) / 2), Time.deltaTime * 5);

		for (int i = 0; i < players.Length; i++) {
			centerPos += players [i].transform.position;
		}
		centerPos /= players.Length;

		centerPos.z = 0;

		offset.z = zoom;

		transform.position = Vector3.Lerp(transform.position, centerPos + offset, Time.deltaTime * 10);
	}
}
