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

	public void ResetGame ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
