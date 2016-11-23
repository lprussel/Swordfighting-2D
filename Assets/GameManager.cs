using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public Player[] players;
	private int playerCount;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		players = FindObjectsOfType<Player> ();
		playerCount = players.Length;
	}

	public void ResetGame ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
