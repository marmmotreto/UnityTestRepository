using UnityEngine;
using System.Collections;

public class RotatorController : MonoBehaviour
{
	public RotateTowards[] m_Rotators;

	private int m_maxRotators;
	private int m_availableRotators;

	void Start()
	{
		m_maxRotators = m_Rotators.Length;
		m_availableRotators = m_maxRotators;
	}

	void Update()
	{
	}

	public bool AssignTarget(Transform t, SpawnPoint sp)
	{
		if(m_availableRotators < 1) return false;

		for(int i = 0; i < m_maxRotators; i++)
		{
			if(m_Rotators[i].HasTarget) continue;

			m_Rotators[i].AssignTarget(t, sp);

			m_availableRotators--;
			return true;
		}

		return false;
	}

	public bool RemoveTarget(SpawnPoint sp)
	{
		for(int i = 0; i < m_maxRotators; i++)
		{
			if(m_Rotators[i].SpawnPointReference != sp) continue;

			m_Rotators[i].RemoveTarget();

			m_availableRotators++;

			return true;
		}

		return false;
	}
}
