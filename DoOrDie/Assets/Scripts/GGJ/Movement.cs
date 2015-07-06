using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	//--------------------------------------------------------------------------------------------------------------------
	[SerializeField]
	private Player m_ME;
	[SerializeField]
	private Player m_Other;					      //Stores a reference to the other player

	//--------------------------------------------------------------------------------------------------------------------
	public enum GROUND_TYPE
	{
		NORMAL,
		LOW_FRICTION,
		HIGH_FRICTION,
		MAX
	};

	private GROUND_TYPE m_CurGroundType;     	  //Current surface the player is walking on

	//--------------------------------------------------------------------------------------------------------------------
	private const float m_BaseSpeed = 1f;    	  //Player's default movement speed

	//--------------------------------------------------------------------------------------------------------------------
	private const float m_BuffRadius = 3f;   	  //The maximum radius the players need to have between
										     	  //each other to have the movement buff

	private const float m_BuffBonus = 0.15f;    	  //The movement buff's value

	private float m_BuffRemainingTime;       	  //Remaining time for the buff to remain active

	private const float m_BuffExitTime = 1f; 	  //The time that has to elapse for the buff to wear out
											 	  //for when the players are no longer within the m_BuffRadius

	private bool m_OnBuff;	 					  //Is the player on buff (is the buff active)?

	private float m_EventBonus=0;

	//--------------------------------------------------------------------------------------------------------------------
	private const float m_StunSpeedFactor = -0.3f; //The movement penalty that occurs when the player is stunned

	private const float m_HighGrassBonus = -0.2f;

	private const float m_MudBonus = -0.4f;

	private const float m_RoadBonus = 0.5f;

	private float m_GroundBonus;
	

	//--------------------------------------------------------------------------------------------------------------------
	private Vector3 m_CurDirection;				  //The direction the player is moving along

	private Vector3 m_SpeedVector;				  //The actual speed the player is going to move

	private float m_FloorFriction;				  //UNDONE Friction coefficient



	//--------------------------------------------------------------------------------------------------------------------
	public CharacterController m_Controller;

	// Use this for initialization
	void Start ()
	{
		m_FloorFriction = 3f;
		m_SpeedVector = new Vector3 ();
		m_Controller = gameObject.GetComponent<CharacterController> ();
	}

	// Update is called once per frame
	void Update ()
	{
		if(HudManager.Instance.m_IsGameOver||!HudManager.Instance.m_GameRunning)
			return;
		//If not alive, return
		if(!m_ME.IsAlive()) return;
		m_EventBonus=0;
		m_GroundBonus=0;
		//Get the direction
		m_CurDirection  = InputManager.Instance.GetInputDir(m_ME.m_ID) * m_BaseSpeed;

		//add buffbonus if its on buff state
		if(CheckBuff())
			m_EventBonus=m_BuffBonus;

		//add stun bonus
		if(m_ME.IsStunned())
			m_EventBonus+=m_StunSpeedFactor;

		//Add boxes bonus
		m_EventBonus+=m_ME.m_Inventory.GetBoxMultiplier();
	

		if(m_ME.m_Inventory.m_OnMud)
			m_GroundBonus=m_MudBonus;

		if(m_ME.m_Inventory.m_OnHighGrass)
			m_GroundBonus+=m_HighGrassBonus;

		if(m_ME.m_Inventory.m_OnRoad)
			m_GroundBonus+=m_RoadBonus;
			
		//How many boxes the player has
		m_CurDirection *= (1+(m_GroundBonus+m_EventBonus));

	//	m_Controller.Move(m_CurDirection*Time.deltaTime);
	//	return;

		//Apply friction
		m_CurDirection += -m_SpeedVector * m_FloorFriction;

		//Obtain resulting speed vector
		m_SpeedVector = m_SpeedVector + (m_CurDirection * Time.deltaTime);

		//Move the character
		m_Controller.Move((m_SpeedVector.magnitude > 0.01f) ? m_SpeedVector : Vector3.zero);
	}

	public bool CheckBuff()
	{
		//If the players are closer then the max radius
		m_OnBuff = (Vector3.Distance (m_Other.transform.position, transform.position) < m_BuffRadius);

		//If the player is on buff, restart the timer. If not, decrease it
		m_BuffRemainingTime = (m_OnBuff) ? m_BuffExitTime : m_BuffRemainingTime - Time.deltaTime;

		//Return whether the buff is still active or not
		return (m_OnBuff) ? m_OnBuff : (m_BuffRemainingTime > 0f);
	}

	public Vector3 GetSpeed()
	{
		return m_SpeedVector;
	}
}
