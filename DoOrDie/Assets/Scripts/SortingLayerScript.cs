using UnityEngine;
using System.Collections;

public class SortingLayerScript : MonoBehaviour {
	
	private Transform m_transform;
	private SpriteRenderer sortLayer;

	public float m_OffsetZ;

	// Use this for initialization
	void Start () 
	{
		m_transform = GetComponent<Transform> ();
		sortLayer = GetComponent<SpriteRenderer> ();
		sortLayer.sortingOrder = -Mathf.FloorToInt((m_transform.transform.parent.transform.position.z+m_OffsetZ)*10);
	}
	
	// Update is called once per frame
	void Update () 
	{
		sortLayer.sortingOrder = -Mathf.FloorToInt((m_transform.transform.parent.transform.position.z+m_OffsetZ)*10);
	}

}
