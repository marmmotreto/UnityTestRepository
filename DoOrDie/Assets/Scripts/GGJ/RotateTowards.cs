using UnityEngine;
using System.Collections;
using TMPro;

public class RotateTowards : MonoBehaviour
{
	public Color m_PointColor;
	public Renderer m_MaterialRef;
	[SerializeField]
	private bool m_HasTarget;
	[SerializeField]
	private Vector3 m_Target;
	[SerializeField]
	private Transform m_Flag;
	private Vector3 m_Rotation;
	private Vector3 m_Position;

	private Vector2 m_Target2D;
	private Vector2 m_Pos2D;
	
	private float c_op, c_ad, ang;

	private float m_minProximity = 15f;

	private SpawnPoint m_ref;

	public TextMeshProUGUI m_Text;

	public TextMeshProUGUI m_FlagText;

	private float m_originalAlpha;
	private Vector3 m_HideVector = new Vector3(0f, -50f, 0f);
	// Use this for initialization
	void Start ()
	{
		c_op = 0f;
		c_ad = 0f;
		ang = 0f;
		
		m_Rotation = new Vector3 ();
		m_Position = new Vector3 ();
		
		m_Rotation.x = 0f; 
		m_Rotation.z = 0f;

		m_MaterialRef.material.SetColor ("_TintColor", m_PointColor);
		m_Flag.GetChild(0).renderer.material.SetColor ("_TintColor", m_PointColor);

		Debug.Log ("Initialized");

		m_ref = null;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_HasTarget) CalcRotation();

		m_Target2D.x = m_Target.x;
		m_Target2D.y = m_Target.z;

		m_Pos2D.x = transform.position.x;
		m_Pos2D.y = transform.position.z;

		bool prox = Vector2.Distance (m_Target2D, m_Pos2D) < m_minProximity;

		m_Position.y = m_HasTarget ? ((prox) ? -10f : 0f) : -10f;

		m_Flag.position = prox ? (m_HasTarget ? m_Target : m_HideVector) : m_HideVector;

		transform.localPosition = m_Position;

		if(m_ref!=null)
		{	
			m_Text.text = m_ref.m_BoxRef.GetRemainingTime().ToString();
			m_FlagText.text = m_Text.text;
		}
	}
	
	public void AssignTarget(Transform target, SpawnPoint sp)
	{
		m_Target = target.position;
		m_ref = sp;
		m_HasTarget = true;
	}
	
	public void RemoveTarget()
	{
		m_Target = Vector3.zero;
		m_ref = null;
		m_HasTarget = false;
	}
	
	public bool HasTarget
	{
		get { return m_HasTarget; }
	}

	public SpawnPoint SpawnPointReference
	{
		get { return m_ref; }
	}

	private void CalcRotation()
	{
		c_op = m_Target.z - transform.position.z;
		c_ad = m_Target.x - transform.position.x;
		
		ang = Mathf.Atan (c_ad / c_op) * Mathf.Rad2Deg;
		
		m_Rotation.x = 0f; 
		m_Rotation.z = 0f;

		m_Rotation.y = ang + ((m_Target.z < transform.position.z) ? 180f : 0f);
		
		transform.eulerAngles = m_Rotation;
	}
}
