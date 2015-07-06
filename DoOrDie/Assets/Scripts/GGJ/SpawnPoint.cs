using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	public SpriteRenderer m_SpriteRef;
	public SpriteRenderer m_PackageRef;
	
	public Sprite[] m_SpriteIdles;
	public Sprite[] m_SpritePickup;
	public Sprite[] m_SpriteReceive;
	public Sprite[] m_Packages;
	
	private bool  m_GetBox;
	private float m_GetBoxTime;
	private float m_ETAGetBoxTime = 1f;
	
	private int m_SpriteID;




	bool m_Interacting=false;
	public enum PointType
	{
		None,
		Box,
		Target,
		MAX
	}

	public PointType m_Type=PointType.None;

	public Box m_BoxRef;

	//Initialize the values for this spawn point
	public void Initialize(PointType _type,Box _boxRef)
	{
		m_Type=_type;
		m_BoxRef=_boxRef;
		if(_type==PointType.Box)
			m_BoxRef.SetUp((SpawnPoint)this,(Box.BoxType)Random.Range(0,(int)Box.BoxType.MAX));
		else
			m_BoxRef.m_Goal=(SpawnPoint)this;

		SpriteRegen (false);

	}

	public void Interact(Inventory _inventory)
	{
		if(m_Type==PointType.Box)
		{
			if(_inventory.AddBox(m_BoxRef))
			{
				m_BoxRef.PickUp();
				_inventory.m_Player.m_RotController.AssignTarget(m_BoxRef.m_Goal.transform, m_BoxRef.m_Goal);
				SpawnManager.Instance.RecycleSpawnPoint((SpawnPoint)this);
				SpriteRegen (false);
			}

			return;
		}

		if(m_Type==PointType.Target)
		{
			if(_inventory.RemoveBox(m_BoxRef))
			{
				_inventory.ApplyScore(m_BoxRef.m_Score);
				_inventory.m_Player.m_RotController.RemoveTarget(m_BoxRef.m_Goal);
				AudioManager.Instance.PlayDropOffSnd();
				m_BoxRef.Recycle();
				SpawnManager.Instance.RecycleSpawnPoint((SpawnPoint)this);
				SpriteRegen (true);

			}
		}


	}

	void Start()
	{
		m_SpriteID = Random.Range (0, m_SpriteIdles.Length);
		SpriteRegen (false);
	}

	void SpriteRegen(bool receive)
	{
		if(receive)
		{
			m_GetBox = true;
			m_GetBoxTime = m_ETAGetBoxTime;
			m_SpriteRef.sprite = m_SpriteReceive[m_SpriteID];
			m_PackageRef.sprite = null;
			return;
		}
		
		if(m_Type == PointType.Box)
		{
			m_SpriteRef.sprite = m_SpritePickup[m_SpriteID];
			m_PackageRef.sprite = m_Packages[Random.Range(0, m_Packages.Length)];
			return;
		}
		
		m_SpriteRef.sprite = m_SpriteIdles [m_SpriteID];
		m_PackageRef.sprite = null;
	}
	
	void Update()
	{
		if(m_GetBox)
		{
			m_GetBoxTime -= Time.deltaTime;
			
			if(m_GetBoxTime < 0f)
			{
				m_GetBox = false;
				SpriteRegen(false);
			}
		}
	}
}
