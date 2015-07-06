using UnityEngine;
using System.Collections;

public class ConnectionEffect : MonoBehaviour
{
	public float speed;
	private Material m_ref;
	private float ETA;
	private Vector2 pos;
	private Vector2 invPos;
	// Use this for initialization
	void Start ()
	{
		pos = new Vector2 ();
		invPos = new Vector2 ();
		ETA = 0f;
		m_ref = gameObject.renderer.material;
	}
	
	// Update is called once per frame
	void Update ()
	{
		ETA += Time.deltaTime * speed;

		if(ETA > 10f) ETA -= 10f;

		pos.x = ETA % 1f;
		invPos.x = -pos.x;

		m_ref.SetTextureOffset ("_MainTex", pos);
		m_ref.SetTextureOffset ("_OtherTex", invPos);
	}
}
