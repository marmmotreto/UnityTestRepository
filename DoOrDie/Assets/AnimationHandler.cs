using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour {

	private Animator m_Animator;
	private CharacterController m_CharControl;
	private Player m_Player;
	private Inventory m_Inventory;
	private Movement m_Movement;

	private const float m_Threshold = 1f;
	public ParticleSystem m_DeathParticles;


	bool m_deadDone=false;

	// Use this for initialization
	void Start() 
	{
		m_Animator = GetComponent<Animator>();
		m_CharControl = GetComponentInParent<CharacterController>();
		m_Player = GetComponentInParent<Player>();
		m_Inventory = GetComponentInParent<Inventory>();
		m_Movement = GetComponentInParent<Movement>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_deadDone)
			return;

		if(HudManager.Instance.m_IsGameOver&&!m_Player.IsAlive())
		{
			m_deadDone=true;
			m_DeathParticles.Play(true);
			m_Animator.Play("Death");
		}


		if(m_Inventory.m_PickUpAnimation)
		{
			m_Animator.Play("PickUp");
			if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("PickUp"))
			{
				if(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
				{
					m_Inventory.m_PickUpAnimation = false;
				}
			}
		}
		else if(m_Player.IsAlive())
		{
			if(m_Player.GetFrenzy() < 3.0f)
			{
				m_Animator.Play("Frenzy");
				if(m_CharControl.velocity.x > 0.1f)
				{
					transform.localScale = new Vector3(2.5f,2.5f,0);
				}
				else if(m_CharControl.velocity.x < -0.1f)
				{
					transform.localScale = new Vector3(-2.5f,2.5f,0);
				}
			}
			else if((m_CharControl.velocity.x < -m_Threshold || m_CharControl.velocity.x > m_Threshold ||
			         m_CharControl.velocity.z < -m_Threshold || m_CharControl.velocity.z > m_Threshold) &&
			        (m_Inventory.GetBoxesAmount() >= 3 || m_Player.IsStunned()) /* ADD AN OR FOR SLOW TERRAIN*/)
			{
				m_Animator.Play("Slow_Run");
				if(m_CharControl.velocity.x > 0.1f)
				{
					transform.localScale = new Vector3(2.5f,2.5f,0);
				}
				else if(m_CharControl.velocity.x < -0.1f)
				{
					transform.localScale = new Vector3(-2.5f,2.5f,0);
				}
			}
			else if((m_CharControl.velocity.x < -m_Threshold || m_CharControl.velocity.x > m_Threshold ||
			         m_CharControl.velocity.z < -m_Threshold || m_CharControl.velocity.z > m_Threshold) 
			        && m_Movement.CheckBuff())
			{ 
				m_Animator.Play("Fast_Run");
				if(m_CharControl.velocity.x > 0.1f)
				{
					transform.localScale = new Vector3(2.5f,2.5f,0);
				}
				else if(m_CharControl.velocity.x < -0.1f)
				{
					transform.localScale = new Vector3(-2.5f,2.5f,0);
				}
			}
			else if(m_CharControl.velocity.x > m_Threshold || m_CharControl.velocity.x < -m_Threshold ||
			        m_CharControl.velocity.z < -m_Threshold || m_CharControl.velocity.z > m_Threshold )
			{
				if(m_Inventory.m_OnMud||m_Inventory.m_OnHighGrass)
					m_Animator.Play("Slow_Run");
				else
					m_Animator.Play("Normal_Run");
				if(m_CharControl.velocity.x > 0.1f)
				{
					transform.localScale = new Vector3(2.5f,2.5f,0);
				}
				else if(m_CharControl.velocity.x < -0.1f)
				{
					transform.localScale = new Vector3(-2.5f,2.5f,0);
				}
			}

			else
			{
				m_Animator.Play("Idle_Animation");
			}
		}
	}
}