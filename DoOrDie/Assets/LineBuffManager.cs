using UnityEngine;
using System.Collections;

public class LineBuffManager : MonoBehaviour {

	public Player m_Player1;
	public Player m_Player2;
	public Movement m_Move;
	private LineRenderer m_Line;

	private Vector3 m_LineStart = Vector3.zero;
	private Vector3 m_LineEnd = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		m_Line = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_Move.CheckBuff())
		{
			m_LineEnd = new Vector3(m_Player2.transform.position.x,0f,m_Player2.transform.position.z);
			m_LineStart = new Vector3(m_Player1.transform.position.x,0f,m_Player1.transform.position.z);
			m_Line.enabled = true;
			if(m_LineStart.z > m_LineEnd.z)
			{
				m_Line.sortingOrder = -Mathf.FloorToInt(m_LineStart.z*10);
			}
			else
			{
				m_Line.sortingOrder = -Mathf.FloorToInt(m_LineEnd.z*10);
			}
			m_Line.SetPosition(0,m_LineStart);
			m_Line.SetPosition(1,m_LineEnd);
		}
		else
		{
			m_Line.enabled = false;
		}
	}
}
