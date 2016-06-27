using UnityEngine;
using System.Collections;

//The role of this class is the square shaped selector for building towers:
// When clicked somewhere not on anything, it means this object was clicked
// and if the place has a tower, then the tower is selected
// if no tower, then this object is selected,
// and it will be green or gray depending on whether or not it is a buildable area

public class TowerPlaceSelection : MonoBehaviour, IMySelectable {

	public int i, j;
	bool same;

	Renderer mr;

	void Awake()
	{
		mr = GetComponent<Renderer>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetNewCoords(int i, int j)
	{
		same = (this.i == i && this.j == j);
		this.i = i;
		this.j = j;
	}

	public void SetSelected(bool val)
	{
		mr.enabled = val;

		
		
		if(val)
		{
			var pos = BuildingManager.Instance.GetPosFromCoords(i, j) + new Vector3(0, 0.2f, 0);
			transform.position = pos;
			//Debug.LogFormat("can build there? : {0}",  BuildingManager.Instance.CanBuildAt(i, j));
			var mat = BuildingManager.Instance.CanBuildAt(i, j) ? BuildingManager.Instance.canBuildThereMaterial : BuildingManager.Instance.cannotBuildThereMaterial;
			var renderer = GetComponent<Renderer>();
			renderer.material = mat;
		}
		else
		{

		}
		
		
	}

	public bool SameAs(IMySelectable other)
	{
		if(this == other)
		{
			return same;
		}
		return false;
	}
}
