using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using PlayerPt2;

public static class GameManager
{
	public static PlayerManager[] Players;
    public static PlayerSettings PSettings;

    static GameManager()
    {
        PSettings = Resources.Load("PlayerSettings") as PlayerSettings;

        Players = GameObject.FindObjectsOfType<PlayerManager>();
    }
    
    public static PlayerManager GetOtherPlayer (int thisPlayerNumber)
    {
        int otherPlayerNumber = thisPlayerNumber == 0 ? 1 : 0;
        return Players[otherPlayerNumber];
    }

    public static void PlayerWon()
    {
        CoroutineRunner.StartCoroutine(PlayerWonRoutine());
    }

    public static IEnumerator PlayerWonRoutine ()
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

	public static void ResetGame ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
