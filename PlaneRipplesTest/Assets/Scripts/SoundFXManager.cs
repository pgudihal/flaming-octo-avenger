using UnityEngine;
using System.Collections;

public sealed class SoundFXManager : MonoBehaviour 
{

	public AudioClip[] FX;
	public enum FXNames{BreakRoll = 0};

	private AudioSource[] fxSources;

	// Use this for initialization
	void Start () 
	{
		fxSources = new AudioSource[FX.Length];
		for(int i = 0; i < FX.Length;i++)
		{
			fxSources[i] = gameObject.AddComponent<AudioSource>();
			fxSources[i].playOnAwake = false;
			fxSources[i].clip = FX[i];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playFX(SoundFXManager.FXNames name)
	{
		fxSources[(int)name].Play();
	}
}
