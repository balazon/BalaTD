using UnityEngine;
using System.Collections;

public class AudioFX : MonoBehaviour {



	public AudioSource source;

	public AudioClip[] clips;


	public static AudioFX Instance { get; private set;}

//	protected bool paused;
//	public bool Paused
//	{
//		get
//		{
//			return paused;
//		}
//		set
//		{
//			paused = value;
//			AudioListener.pause = paused;
//		}
//	}

	public bool FocusLost {get; set;}

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		FocusLost = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void PlayAudioClip(int index)
	{
		if(FocusLost)
		{
			return;
		}
		source.PlayOneShot(clips[index]);

	}



}
