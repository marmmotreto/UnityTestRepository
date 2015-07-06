using UnityEngine;
using System.Collections;

public class StaticSortingLayer : MonoBehaviour {

	private Transform m_transform;
	private SpriteRenderer sortLayer;
	private int m_SortedLayer;


	// Use this for initialization
	void Start () 
	{
		m_transform = GetComponent<Transform> ();
		sortLayer = GetComponent<SpriteRenderer> ();
		if(sortLayer != null)
		{
			sortLayer.sortingOrder = -Mathf.FloorToInt(m_transform.position.z*10);
		}
		m_SortedLayer = -Mathf.FloorToInt(m_transform.position.z*10);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
