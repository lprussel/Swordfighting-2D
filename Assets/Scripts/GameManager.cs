using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public Player[] players;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		players = FindObjectsOfType<Player> ();
	}

	public void PlayerWon ()
	{
		StartCoroutine (_PlayerWon ());
	}

	IEnumerator _PlayerWon ()
	{
		Time.timeScale = .25f;
		yield return new WaitForSeconds (1);
		Time.timeScale = 1f;
		ResetGame ();
	}

	public void ResetGame ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
