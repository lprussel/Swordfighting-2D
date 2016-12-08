using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public PlayerManager[] players;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		players = FindObjectsOfType<PlayerManager> ();
	}

	public void PlayerWon ()
	{
		StartCoroutine (_PlayerWon ());
	}

	IEnumerator _PlayerWon ()
	{
		float t = 0;
		float maxT = 1;

		while (t < maxT)
		{
			t += Time.deltaTime;
			Time.timeScale = Mathf.Lerp(.1f, 1f, t / maxT);
			yield return null;
		}
		Time.timeScale = 1f;
		ResetGame ();
	}

	public void ResetGame ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
