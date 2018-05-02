using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

	private AudioSource audioS;
	private bool mute = false;
	private int currentSong = 0;

	public AudioClip menuFull;
	public AudioClip menuStart;
	public AudioClip menuLoop;
	public AudioClip tutorial;




	public void SetSound(int which, bool doPlay, bool doLoop)
	{
		currentSong = which;

		if (which == 1)
		{ audioS.clip = menuFull; }
		else if (which == 2)
		{ audioS.clip = menuStart; }
		else if (which == 3)
		{ audioS.clip = menuLoop; }
		else if (which == 4)
		{ audioS.clip = tutorial; }

		audioS.loop = doLoop;

		if (doPlay && !mute)
		{ audioS.Play(); }
		else
		{ audioS.Stop(); }
	}



	void Start ()
	{
		audioS = GetComponent<AudioSource>();

		SetSound(1, true, true);
	}


	void DebugUpdate()
	{
		if (Input.GetKey("1"))
		{ SetSound(1, true, true); }
		else if (Input.GetKey("2"))
		{ SetSound(2, true, true); }
		else if (Input.GetKey("3"))
		{ SetSound(3, true, true); }
		else if (Input.GetKey("4"))
		{ SetSound(4, true, true); }
	}

	
	void Update ()
	{
		if (Input.GetKeyDown("m"))
		{
			if (mute)
			{
				mute = false;
				SetSound(currentSong, true, true);
			}
			else
			{
				mute = true;
				audioS.Stop();
			}
		}
	}

}
