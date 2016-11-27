using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioClip slashSound;
	public AudioClip hitSound;
	public AudioClip blockSound;
	public AudioClip dashSound;
	public AudioClip reposteSound;

	public AudioSource source;

	public enum SoundSet
	{
		SLASH,
		HIT,
		BLOCK,
		REPOSTE,
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
				break;
			case SoundSet.SLASH:
				soundToPlay = slashSound;
				break;
			case SoundSet.DASH:
				soundToPlay = dashSound;
				break;
			case SoundSet.REPOSTE:
				soundToPlay = reposteSound;
				break;
		}

		if (soundToPlay == null)
			return;

		source.PlayOneShot (soundToPlay);
	}
}
