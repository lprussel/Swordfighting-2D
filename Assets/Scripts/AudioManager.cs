using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioClip slashSound;
	public AudioClip hitSound;
	public AudioClip blockSound;
	public AudioClip dashSound;

	public AudioSource source;

	public enum SoundSet
	{
		SLASH,
		HIT,
		BLOCK,
		DASH
	}

	void Awake ()
	{
		instance = this;
	}

	public void PlaySound (SoundSet sound)
	{
		AudioClip soundToPlay = null;
		switch (sound)
		{
			case SoundSet.BLOCK:
				soundToPlay = blockSound;
				break;
			case SoundSet.HIT:
				soundToPlay = hitSound;
				Debug.Log ("HIT SOUND");
				break;
			case SoundSet.SLASH:
				soundToPlay = slashSound;
				break;
			case SoundSet.DASH:
				soundToPlay = dashSound;
				break;
		}

		if (soundToPlay == null)
			return;

		source.PlayOneShot (soundToPlay);
	}
}
