using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	static private AudioManager instance;
	static public AudioManager Instance{ get { return instance; } }

	public AudioClip m_ButtonSound;
	public AudioClip m_StunSound;
	public AudioClip[] m_RandomSound;

	public AudioClip m_DeathSound;
	public AudioClip m_PickUpSound;
	public AudioClip m_DropOffSound;

	public AudioSource m_SoundEffects;
	public AudioSource m_MusicIntro;
	public AudioSource m_MusicLoop;
	public AudioSource m_MenuMusic;
	// Use this for initialization
	void Start () 
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	//THIS SOUND WILL PLAY WHEN A BUTTON IS PRESSED
	public void PlayButtonSnd()
	{
		//return;//"BUTTONSOUND");
		m_SoundEffects.clip = m_ButtonSound;
		m_SoundEffects.Play ();
	}

	//THIS SOUND WILL PLAY WHEN THE PLAYER IS STUNNED
	public void PlayStunSnd()
	{
		return;//"STUNSOUND");
		m_SoundEffects.clip = m_StunSound;
		m_SoundEffects.Play ();
	}

	//THIS WILL PICK OUT A RANDOM SOUND FROM THE m_RandomSound[]
	public void PlayPlayerRandomSnd()
	{
		int len = m_RandomSound.Length;
		int choice = Random.Range(0,len-1);
		m_SoundEffects.clip = m_RandomSound[choice];
		m_SoundEffects.Play ();
	}

	//THIS WILL PLAY THE MUSIC 
	public void PlayMusic()
	{
		//return;//"MUSICPLAYING");
		m_MenuMusic.Stop();
		m_MusicIntro.Play();
		Invoke("PlayMusicLoop",m_MusicIntro.clip.length-0.5f);
	}

	public void PlayMenuMusic()
	{
		m_MusicIntro.Stop();
		m_MusicLoop.Stop();
		m_MenuMusic.Play();
	}

	void PlayMusicLoop()
	{
		m_MusicLoop.Play();
	}

	//THIS WILL PLAY A SOUND WHEN YOU LOSE THE GAME
	public void PlayDeathSnd()
	{
		return;//"DEATHSOUND");
		if(m_SoundEffects.isPlaying && m_SoundEffects.clip == m_DeathSound)
		{

		}
		else
		{
			if(!m_SoundEffects.clip == m_DeathSound)
			{
				m_SoundEffects.clip = m_DeathSound;
			}
			m_SoundEffects.Play ();
		}
	}

	//THIS WILL PLAY A SOUND WHEN YOU PICK UP A BOX
	public void PlayPickUpSnd()
	{
		return;//"PICKUP");
		m_SoundEffects.clip = m_PickUpSound;
		m_SoundEffects.Play ();
	}

	//THIS WILL PLAY A SOUND WHEN YOU DELIVER A BOX
	public void PlayDropOffSnd()
	{
		return;//"DROPOFF");
		m_SoundEffects.clip = m_DropOffSound;
		m_SoundEffects.Play ();
	}
}
