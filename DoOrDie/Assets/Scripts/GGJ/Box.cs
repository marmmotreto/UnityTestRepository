using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {

	public float m_Score;
	public SpawnPoint m_Goal;
	float m_ExpirationFactor=0.35f;
	float m_RemainingTime;
	public Vector3 m_HidePosition=new Vector3(-100,0,0);



	public enum BoxType
	{
		Type1,
		Type2,
		Type3,
		MAX
	}

	public BoxType m_Type=BoxType.Type1;


	public void SetUp(SpawnPoint _goal,BoxType _type)
	{
		m_Goal=_goal;
		m_Type=_type;
		transform.position=_goal.transform.position;
		//TODO: update sprite
	}

	public void PickUp()
	{
		SpawnManager.Instance.GetTarget((Box)this);
		m_Score=m_ExpirationFactor*Vector3.Distance(transform.position,m_Goal.transform.position);
		m_RemainingTime=m_Score;
		transform.position=m_HidePosition;
	}

	public int GetRemainingTime()
	{
		return Mathf.FloorToInt(m_RemainingTime);
	}


	public bool UpdateByPlayer()
	{
		m_RemainingTime-=Time.deltaTime;
		if(m_RemainingTime<=0)
			return false;
		return true;
	}

	public void Recycle()
	{
		SpawnManager.Instance.RecycleBox((Box)this);
	}
}
