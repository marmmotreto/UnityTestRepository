using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	static private InputManager instance;
	static public InputManager Instance{ get { return instance; } }

	float m_XvalP1 = 0.0f;
	float m_YvalP1 = 0.0f;
	float m_XvalP2 = 0.0f;
	float m_YvalP2 = 0.0f;
	Vector3 m_InputDirP1 = Vector3.zero;
	Vector3 m_InputDirP2 = Vector3.zero;


	// Use this for initialization
	void Start () 
	{
		if(instance == null)
		{
			instance = this;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_XvalP1 = 0.0f;
		m_YvalP1 = 0.0f;
		m_XvalP2 = 0.0f; 
		m_YvalP2 = 0.0f;


		//PLAYER 1's INPUT
		m_XvalP1 = Input.GetAxis("Horizontal1");
		m_YvalP1 = Input.GetAxis("Vertical1");


		//PLAYER 2's INPUT
		m_XvalP2 = Input.GetAxis("Horizontal2");
		m_YvalP2 = Input.GetAxis("Vertical2");


		//ASSIGN THE VALUES
		m_InputDirP1 = new Vector3(m_XvalP1,0,m_YvalP1);
		m_InputDirP2 = new Vector3(m_XvalP2,0,m_YvalP2);
	}

	public Vector3 GetInputDir(int playerID)
	{
		if(playerID == 1)
		{
			return m_InputDirP1.normalized;
		}
		else if(playerID == 2)
		{
			return m_InputDirP2.normalized;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public bool GetPickUp(int playerID)
	{
		if(playerID == 1)
		{
			return Input.GetAxis("PlayerPick1") == 1;
		}
		else if(playerID == 2)
		{
			return Input.GetAxis("PlayerPick2") == 1;
		}
		else
		{
			return false;
		}
	}

}
