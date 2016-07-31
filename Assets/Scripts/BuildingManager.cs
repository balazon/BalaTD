using UnityEngine;
using System.Collections;

public class BuildingManager : MonoBehaviour {

	public Material canBuildThereMaterial;
	public Material cannotBuildThereMaterial;

	public static BuildingManager Instance{ get; private set;}

	public Plane BuildingPlane;

	protected Tower[] towers;
	
	protected bool[] buildingLocations;

	protected int w;
	protected int h;


	protected Pathfinding.GridGraph gg;

	void Awake()
	{
		Instance = this;

	}

	// Use this for initialization
	void Start () {
		Reset();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void Reset()
	{
		foreach(var t in GameObject.FindGameObjectsWithTag("Tower"))
		{
			Destroy(t);
		}


		gg = AstarPath.active.astarData.gridGraph;

		BuildingPlane = new Plane(Vector3.up, gg.center);

		w = gg.width;
		h = gg.depth;


		towers = new Tower[w * h];
		
		buildingLocations = new bool[w * h];
		

		for(int i = 0; i < w * h; i++)
		{
			int ti = i % w;
			int tj = w - 1 - i / w;
			buildingLocations[ti + tj * w] = gg.nodes[i].Walkable;
		}
	}

	public bool GetTowerCoords(Tower t,out int i, out int j)
	{
		return GetCoordsFromVector(t.gameObject.transform.position, out i, out j);
	}

	//sets i and j to coords, returns if they are valid
	public bool GetCoordsFromVector(Vector3 pos, out int i, out int j)
	{
		i = (int)((pos.x - gg.center.x) / gg.nodeSize + w * 0.5f);
		j = (int)((gg.center.z - pos.z) / gg.nodeSize + h * 0.5f);
		//Debug.LogFormat("x: {0}, y: {1}", x, y);
		return i >= 0 && i < w && j >= 0 && j < h;
	}

	public Vector3 GetPosFromCoords(int i, int j)
	{
		float relx = (i - (w - 1) * 0.5f) * gg.nodeSize;
		float relz = (-j + (h - 1) * 0.5f) * gg.nodeSize;
		return gg.center + new Vector3(relx, 0.0f, relz);
	}
	
	public Tower GetTowerAtVec(Vector3 pos)
	{
		int i, j;
		if(GetCoordsFromVector(pos, out i, out j))
		{
			return GetTowerAt(i, j);
		}

		return null;
	}



	public Tower GetTowerAt(int i, int j)
	{
		return towers[j * w + i];
	}

	public bool CanBuildAtVec(Vector3 pos)
	{
		int i, j;
		if(GetCoordsFromVector(pos, out i, out j))
		{
			return CanBuildAt(i, j);
		}
		return false;
	}

	public bool CanBuildAt(int i, int j)
	{
		return GetTowerAt(i, j) == null && buildingLocations[j * w + i];
	}



	public Tower buildTower(ETower type, int i, int j)
	{
		return buildTowerAt(i, j, Configurations.Instance.GetBaseTowerPrefab(type), Configurations.Instance.GetTowerLevels(type));
	}


	protected Tower buildTowerAt(int i, int j, GameObject prefab, TowerLevelAttributes[] levels)
	{
		Vector3 towerPos = GetPosFromCoords(i, j) + Vector3.up;
		//Debug.LogFormat("tower: ij {0} {1}, pos: {2}", i, j, towerPos);
		var go = Instantiate(prefab, towerPos, Quaternion.identity) as GameObject;
		go.transform.parent = GameManager.DynamicObjectContainer.transform;
		Tower t = go.GetComponent<Tower>();
		t.init(levels);
		towers[j * w + i] = t;
		return t;
	}

	//sells the tower, returns its selling price
	public int sellTower(Tower t)
	{
		int i, j;

		if(GetTowerCoords(t, out i, out j))
		{
			towers[j * w + i] = null;
			buildingLocations[j * w + i] = true;
		}

		var soldPrice = Configurations.Instance.getSellingPriceOfTower(t.type, t.level);

		t.Finished();

		return soldPrice;
	}

}
