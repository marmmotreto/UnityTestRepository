using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	private static SpawnManager _instance;
	public static SpawnManager Instance{ get{return _instance;}}


	void Awake()
	{
		_instance=this;
	}


	public List<SpawnPoint> m_Spawners=new List<SpawnPoint>();
	[SerializeField]
	private List<SpawnPoint> m_BusySpawners=new List<SpawnPoint>();
	const int m_SpawnLimit=15;
	int m_CurSpawned=0;
	const float m_SpawnCooldown=5.0f;
	public List<Box> m_Boxes=new List<Box>();
	[SerializeField]
	private List<Box> m_SpawnedBoxes=new List<Box>();

	float m_CurCreationTime=0;
	bool m_CreateNewBox=false;
	
	void Start()
	{
		Initialize();
	}

	void Initialize()
	{
		for(int i=0;i<m_SpawnLimit;i++)
		{
			SpawnBox();
		}

	}

	void Update()
	{
		if(m_CreateNewBox&&m_SpawnedBoxes.Count<m_SpawnLimit)
		{
			m_CurCreationTime+=Time.deltaTime;
			if(m_CurCreationTime>=m_SpawnCooldown)
			{
				m_CurCreationTime=0;
				SpawnBox();
				if(m_SpawnedBoxes.Count>=m_SpawnLimit)
					m_CreateNewBox=false;
			}
		}
	}

	void SpawnBox()
	{
		//get rando spawn point and move from available list to occupied list
		int rand=Random.Range(0,m_Spawners.Count);
		m_BusySpawners.Add(m_Spawners[rand]);
		m_Spawners[rand].Initialize(SpawnPoint.PointType.Box,m_Boxes[m_Boxes.Count-1]);
		m_SpawnedBoxes.Add(m_Boxes[m_Boxes.Count-1]);
		m_Boxes.RemoveAt(m_Boxes.Count-1);
		m_Spawners.RemoveAt(rand);

	}

	public void GetTarget(Box _box)
	{
		int rand=Random.Range(0,m_Spawners.Count);
		m_BusySpawners.Add(m_Spawners[rand]);
		m_Spawners[rand].Initialize(SpawnPoint.PointType.Target,_box);
		m_Spawners.RemoveAt(rand); 
	}

	public void RecycleBox(Box _box)
	{
		m_SpawnedBoxes.Remove(_box);
		m_Boxes.Add(_box);
		m_CurCreationTime=0;
		m_CreateNewBox=true;
	}

	//Recycle the Spawn point, make sure to send the box reference to a player before recycle the spawnpoint, otherwise you will lose the box reference 
	public void RecycleSpawnPoint(SpawnPoint _spawnPoint)
	{
		_spawnPoint.m_Type=SpawnPoint.PointType.None;
		_spawnPoint.m_BoxRef=null;
		m_Spawners.Add(_spawnPoint);
		m_BusySpawners.Remove(_spawnPoint);
	}

}
