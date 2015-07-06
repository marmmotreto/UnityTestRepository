using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class HudManager : MonoBehaviour {

	//Serch tiene la culpa

	private static HudManager _instance;
	public static HudManager Instance{ get{return _instance;}}

	public TextMeshProUGUI m_Score;
	public TextMeshProUGUI m_Combo1;
	public TextMeshProUGUI m_Combo2;


	public Camera m_EntranceCam;
	public Canvas m_EntranceCanvas;

	public Camera m_FinishCam;
	public Camera m_GameplayCam;

	public Camera m_CreditsCam;
	public Canvas m_CreditsCanvas;

	public bool m_IsGameOver=false;

	public bool m_GameRunning=false;

	public Image m_P1Status;
	public Image m_P2Status;

	public Sprite m_SpriteP1Pressed;
	public Sprite m_SpriteP2Pressed;


	bool p1Pressed=false;
	bool p2Pressed=false;

	bool m_GameplayCalled=false;
	bool m_CoolDownDone=false;

	public TextMeshProUGUI m_ScoreText;

	void Awake()
	{
		_instance=this;
	}

	public void UpdateScore(float score)
	{
		m_Score.text="N/A";
	}

	public void UpdateCombo(int _player,float _combo)
	{
		if(_player==1)
			m_Combo1.text=_combo.ToString();
		else
			m_Combo2.text=_combo.ToString();
	}

	void Update()
	{
		if(!m_GameplayCalled)
		{
			if(!p1Pressed)
			{
				if(InputManager.Instance.GetPickUp(1))
				{
					p1Pressed=true;
					m_P1Status.sprite=m_SpriteP1Pressed;
				}
			}

			if(!p2Pressed)
			{
				if(InputManager.Instance.GetPickUp(2))
				{
					p2Pressed=true;
					m_P2Status.sprite=m_SpriteP2Pressed;
				}
			}

			if(p1Pressed&&p2Pressed)
			{
				m_GameplayCalled=true;
				AudioManager.Instance.PlayMusic();
				Invoke("ShowGameplay",1);
			}
		}

		if(m_IsGameOver&&m_CoolDownDone)
		{
			if(!p1Pressed&&!p2Pressed)
			{
				if(InputManager.Instance.GetPickUp(1)||InputManager.Instance.GetPickUp(2))
				{
					p1Pressed=true;
					p2Pressed=true;
					Application.LoadLevel(0);
				}
			}
		}
	}

	IEnumerator ExitCooldown()
	{
		yield return new WaitForSeconds(2);
		m_CoolDownDone=true;
		m_CreditsCam.enabled=false;
		m_GameplayCam.enabled=false;
		m_FinishCam.enabled=true;
		m_EntranceCam.enabled=false;
		AudioManager.Instance.PlayMenuMusic();
	}


	public void OpenURLPage()
	{
		Application.OpenURL("http://1simpleidea.mx/");
	}

	public void ShowCredits()
	{
		m_CreditsCam.enabled=true;
		m_GameplayCam.enabled=false;
		m_FinishCam.enabled=false;
		m_EntranceCam.enabled=false;
		m_CreditsCanvas.enabled=true;
		m_EntranceCanvas.enabled=false;
	}

	public void ShowGameplay()
	{
		m_GameRunning=true;
		m_CreditsCam.enabled=false;
		m_GameplayCam.enabled=true;
		m_FinishCam.enabled=false;
		m_EntranceCam.enabled=false;
	}

	public void ShowFinish()
	{
		p1Pressed=false;
		p2Pressed=false;
		m_IsGameOver=true;
		StartCoroutine("ExitCooldown");
		m_ScoreText.text=Player.m_Score.ToString("N0");
	}

	public void ShowEntrance()
	{
		m_CreditsCam.enabled=false;
		m_GameplayCam.enabled=false;
		m_FinishCam.enabled=false;
		m_EntranceCam.enabled=true;
		m_EntranceCanvas.enabled=true;
		m_CreditsCanvas.enabled=false;

	}

}
