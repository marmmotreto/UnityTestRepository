using UnityEngine;
using System.Collections;

public class GroundParticlesController : MonoBehaviour {

	public Movement m_PlayerMovement;
	public Inventory m_Inventory;
	public GameObject m_MudParticles;
	public GameObject m_GrassParticles;
	public GameObject m_WetParticles;
	
	// Update is called once per frame
	void Update () {
		if(HudManager.Instance.m_GameRunning)
		{
			if(m_PlayerMovement.m_Controller.velocity.magnitude>0)
			{
				if(m_Inventory.m_OnMud)
				{
					if(!m_MudParticles.activeSelf)
						m_MudParticles.SetActive(true);
				}
				else
				{
					if(m_MudParticles.activeSelf)
						m_MudParticles.SetActive(false);
				}

				if(m_Inventory.m_OnHighGrass)
				{
					if(!m_GrassParticles.activeSelf)
						m_GrassParticles.SetActive(true);
				}
				else
				{
					if(m_GrassParticles.activeSelf)
						m_GrassParticles.SetActive(false);
				}
				if(!m_WetParticles.activeSelf)
					m_WetParticles.SetActive(true);

			}
			else
			{
				if(m_WetParticles.activeSelf)
					m_WetParticles.SetActive(false);
				if(m_MudParticles.activeSelf)
					m_MudParticles.SetActive(false);
				if(m_GrassParticles.activeSelf)
					m_GrassParticles.SetActive(false);
			}
		}
	
	}
}
