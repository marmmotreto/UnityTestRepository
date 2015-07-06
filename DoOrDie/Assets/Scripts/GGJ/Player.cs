using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {
	//health variables
	float m_Frenzy;
	const float m_MaxFrenzy = 15.0f;
	//whether to gain health or lose it
	const float m_RegenPerSecond = 0.02f;
	const float m_DecreasePerSecond = 0.1f;
	//score
	public static float m_Score = 0.0f;
	//stun variables
	const float m_StunTime = 4.0f;
	float m_RemainingStunTime = 0.0f;
	//inventory
	public Inventory m_Inventory;
	//animator
	Animator m_Animator;
	//To decide which player is which (set in inspector) player1 = 1, player2 = 2.
	public int m_ID;

	int m_ComboIndex=0;


	public GameObject m_FrenzyObj;
	public TextMeshProUGUI m_TextMesh;

	public RotatorController m_RotController;

	// Use this for initialization
	void Start () 
	{
		m_Inventory = GetComponent<Inventory>();
		m_Frenzy = m_MaxFrenzy;
		StartCoroutine("UpdateFrenzy");
	}

	//Returns whether the player is dead or alive;
	public bool IsAlive()
	{
		if(m_Frenzy > 0.0f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//Returns true if player still has stun time remaining
	public bool IsStunned()
	{
		if(m_RemainingStunTime > 0.0f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//will increase remaining stun time to make sure player is stunned
	public void ApplyStun()
	{
		AudioManager.Instance.PlayStunSnd();
		m_RemainingStunTime += m_StunTime;
	}

	void Update()
	{
		if(m_RemainingStunTime>0)
		m_RemainingStunTime = Mathf.Max(0,m_RemainingStunTime-Time.deltaTime);
	}

	public void IncreaseCombo()
	{
		m_ComboIndex=Mathf.Min(m_Inventory.m_MaxBoxAmount-1,m_ComboIndex+1);
	}

	public void ResetCombo()
	{
		m_ComboIndex=0;
	}

	public float GetComboValue()
	{
		return 1+(m_ComboIndex*0.25f);
	}

	//this function will handle the players life changing
	IEnumerator UpdateFrenzy()
	{
		yield return new WaitForSeconds(0.1f);
		if(HudManager.Instance.m_GameRunning)
		if(m_Inventory.GetBoxesAmount() > 0)
		{
			m_Frenzy += m_RegenPerSecond;
			m_Frenzy = Mathf.Clamp(m_Frenzy,0,m_MaxFrenzy);
		}
		else
		{
			m_Frenzy -= m_DecreasePerSecond;
			m_Frenzy = Mathf.Clamp(m_Frenzy,0,m_MaxFrenzy);
		}

		if(m_Frenzy>0.0f)
		{
			if(m_Inventory.GetBoxesAmount()==0&&!m_FrenzyObj.activeSelf)
			{
				m_FrenzyObj.SetActive(true);

			}
			else
			{
				if(m_Inventory.GetBoxesAmount() >0)
				{	
					if(m_FrenzyObj.activeSelf)
						m_FrenzyObj.SetActive(false);
				}
			}
			m_TextMesh.text=m_Frenzy.ToString("N0");
			StartCoroutine("UpdateFrenzy");
		}
		else
		{
			m_FrenzyObj.SetActive(false);
			AudioManager.Instance.PlayDeathSnd();
			HudManager.Instance.ShowFinish();
		}

	}

	public float GetFrenzy()
	{
		return m_Frenzy;
	}
}
