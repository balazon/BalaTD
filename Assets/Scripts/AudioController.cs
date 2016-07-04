using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public float listenerDistanceFromBuildingPlane = 5.0f;

	protected Camera cam;

	

	void Awake()
	{
		cam = Camera.main;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	
	void LateUpdate () {
		gameObject.transform.position = GetListenerPos();
	}


	Vector3 GetListenerPos()
	{
		Ray r = cam.ScreenPointToRay(new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f, 0.0f));
		float enter = 0.0f;
		if (BuildingManager.Instance.BuildingPlane.Raycast(r, out enter))
		{
			var hitPos = r.origin + r.direction * enter;
			return hitPos - (cam.transform.forward * listenerDistanceFromBuildingPlane);
		}
		return Vector3.zero;
	}
	
}
