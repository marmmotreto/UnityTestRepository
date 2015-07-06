using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	//list of carried boxes
	public List <Box> m_Boxes;
	//arrows pointing to target
	private Sprite[] m_Arrows;
	//text to show remaining time
	private Text[] m_Texts;
	//player script to apply stuns and score
	public Player m_Player;
	//box veriables
	public int m_MaxBoxAmount = 5;


	public bool m_OnMud;
	public bool m_OnHighGrass;
	public bool m_OnRoad;

	public bool m_PickUpAnimation;

	public float[] m_BoxSpeedMultipliers = new float[5];
	
	// Use this for initialization
	void Start () 
	{
		m_Player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_Boxes.Count > 0)
		{
			UpdateBoxes ();
		}
	}

	//returns the number of boxes in the list at the current moment
	public int GetBoxesAmount()
	{
		return m_Boxes.Count;
	}

	//if there is less than 5 boxes this function will add a new box to the list otherwise it will return false(fail)
	public bool AddBox(Box box)
	{
		if(m_Boxes.Count < 5)
		{
			AudioManager.Instance.PlayPickUpSnd();
			m_PickUpAnimation=true;
			m_Boxes.Add(box);
			m_Player.ResetCombo();
			return true;
		}
		else
		{
			return false;
		}
	}

	//will remove a box as long as it is in the list
	public bool RemoveBox(Box box)
	{
		if(m_Boxes.Count > 0)
		{
			return m_Boxes.Remove(box);
		}
		else
		{
			return false;
		}
	}

	//Returns a float of m_BoxSpeedMultiplier using m_Boxes.Count-1 as the index
	public float GetBoxMultiplier()
	{
		return m_BoxSpeedMultipliers[m_Boxes.Count];
	}

	//updates all the boxes in the list m_Boxes, if any return false the player is stunned and the box is recycled
	public void UpdateBoxes()
	{
		for(int i = 0; i < m_Boxes.Count; ++i)
		{
			if(!m_Boxes[i].UpdateByPlayer())
			{
				m_Player.ApplyStun();
				m_Player.m_RotController.RemoveTarget(m_Boxes[i].m_Goal);
				m_Boxes[i].Recycle();
				m_Boxes.Remove(m_Boxes[i]);
			}
		}
	}

	//APPLY SCORE TO PLAYER
	public void ApplyScore(float value)
	{
		Player.m_Score += value*m_Player.GetComboValue();
		HudManager.Instance.UpdateScore(Player.m_Score);
		m_Player.IncreaseCombo();
		HudManager.Instance.UpdateCombo(m_Player.m_ID,m_Player.GetComboValue());
	}

	void OnTriggerStay(Collider info)
	{
		//Debug.Log("Trigger: "+info.name);
		if(info.tag=="SpawnPoint")
		{
			if(InputManager.Instance.GetPickUp(m_Player.m_ID))
			{
				info.GetComponent<SpawnPoint>().Interact((Inventory)this);
			}
		}
		else
		{
			if(info.tag=="Mud")
				m_OnMud=true;
			else
			{
				if(info.tag=="HighGrass")
					m_OnHighGrass=true;
				else
				{
					if(info.tag=="Road")
						m_OnRoad=true;
				}
			}
		}
	}
	
	void OnTriggerExit(Collider info)
	{
		if(info.tag=="Mud")
			m_OnMud=false;
		else
		{
			if(info.tag=="HighGrass")
				m_OnHighGrass=false;
			else
			{
				if(info.tag=="Road")
					m_OnRoad=false;
			}
		}
	}


	/*
	void OnDrawGizmos()
	{
		Gizmos.color=Color.red;
		for(int i=0;i<m_Boxes.Count;i++)
			Gizmos.DrawLine(transform.position,m_Boxes[i].m_Goal.transform.position);
	}
	*/

}
