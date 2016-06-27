using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour {

	public float sqrMinMoveMagnitude = 100.0f;


	//for smooth mouse scrolling (smoothdamp function uses it)
	protected float scrollv = 0.0f;

	protected float targetfov;

	Vector3[] boundPositions;
	Vector3[] translates;
	Camera cam;




	void Awake()
	{
		boundPositions = new Vector3[15];
		translates = new Vector3[15];
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

	}
	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches = false;

		targetfov = cam.fieldOfView;
		scrolling = false;
	}

	// Update is called once per frame
	void Update () {

		if(GameManager.ThePlayer.Paused || GameManager.ThePlayerController.State == EControllerState.IN_MENU)
		{
			return;
		}

		handleScrolling();
		handlePanning();
		handlePinching();
		handleClick();
	}


	Vector3 GetPlanePosition(Vector2 screenPos)
	{
		Ray r = cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0.0f));
		float enter = 0.0f;
		if(BuildingManager.Instance.BuildingPlane.Raycast(r, out enter))
		{
			var hitPos = r.origin + r.direction * enter;
			//Debug.LogFormat("Time: {0}, touch {1}: {2}, hit: {3}", Time.time,  index, touchPos.ToString(), hitPos);
			return hitPos;
		}
		return Vector3.zero;
	}


	//-1 meaning mouse
	Vector3 GetTouchPlanePosition(int index)
	{
		Vector3 screenPoint = index == -1 ? Input.mousePosition : new Vector3(Input.GetTouch(index).position.x, Input.GetTouch(index).position.y, 0.0f);
		return GetPlanePosition(screenPoint);
	}

	Vector3 GetPlanePosition(Vector3 screenPoint)
	{
		var ray = cam.ScreenPointToRay(screenPoint);


		float enter;
		if(BuildingManager.Instance.BuildingPlane.Raycast(ray, out enter))
		{
			var hitPos = ray.origin + ray.direction * enter;

			return hitPos;
		}
		return Vector3.zero;
	}




	Vector3 StartMousePlanePos;
	bool mousePanning;

	Vector3 sum = Vector3.zero;
	void handlePanning()
	{
		var MouseCurrentPlanePos = GetTouchPlanePosition(-1);

		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(-1))
		{
			StartMousePlanePos = MouseCurrentPlanePos;
			mousePanning = true;
		}

		bool touchPan = false;
		Vector3 tr = Vector3.zero;
		if(Input.multiTouchEnabled && Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(0))
		{
			touchPan = true;

			for(int i = 0; i < Input.touchCount; i++)
			{
				int id = Input.GetTouch(i).fingerId;
				if(Input.GetTouch(i).phase == TouchPhase.Began)
				{
					//Debug.LogFormat("touch {0} begin, cam: {2}", id, translates[id], cam.transform.position);

					boundPositions[id] = GetTouchPlanePosition(i);
					translates[id] = Vector3.zero;
				}
				if(Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary)
				{
					var currentTr = (boundPositions[id] - GetTouchPlanePosition(i)) / Input.touchCount;
					translates[id] = currentTr;
					tr += currentTr;
				}
				if(Input.GetTouch(i).phase == TouchPhase.Ended)
				{
					sum += translates[id] * Input.touchCount;
					//Debug.LogFormat("touch {0} end, tr: {1}, cam: {2}, sum: {3}", id, translates[id], cam.transform.position, sum);
				}
			}

			//cam.transform.position += tr + sum;

		}

		if(mousePanning && Input.mousePresent)
		{

			tr = StartMousePlanePos - GetTouchPlanePosition(-1);
		}


		if(mousePanning || touchPan)
		{
			tr.y = 0;
			sum.y = 0;
			cam.transform.position += tr + sum;

//			Debug.LogFormat("avgdelta2: {0}", avgDelta);
//			avgDelta.y = 0;
//			cam.transform.position -= avgDelta;
		}

		if(Input.GetMouseButtonUp(0))
		{
			mousePanning = false;
		}

	}




	//IMPORTANT: this way of pinching relies on boundpositions that are calculated in HandlePanning
	// so make sure to call handlePanning before calling handlePinching
	void handlePinching()
	{
		if(Input.touchCount > 1)
		{
			float diff = (boundPositions[0] - boundPositions[1]).magnitude / (GetTouchPlanePosition(0) - GetTouchPlanePosition(1)).magnitude;
			//Debug.LogFormat("diff: {0}", diff);
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView * diff, 5.0f, 80.0f);
		}
	}

	bool scrolling;
	void handleScrolling()
	{
		var d = Input.GetAxis("Mouse ScrollWheel");
		if(Mathf.Abs(d) > 0.1)
		{
			if(!scrolling)
			{
				targetfov = cam.fieldOfView;
			}
			scrolling = true;
			var relfov = (d > 0) ? -5.0f : 5.0f;
			targetfov += relfov;
			targetfov = Mathf.Clamp(targetfov, 10.0f, 80.0f);
		}
		if(scrolling)
		{
			cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetfov, ref scrollv, 0.1f);
		}
		if(Mathf.Abs(cam.fieldOfView - targetfov) < 0.1)
		{
			scrolling = false;
		}

	}

	bool tapBegin;
	Vector2 tapStartPos;
	bool clickBegin;
	Vector2 clickStartPos;
	void handleClick()
	{
		
		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(0))
		{
			tapBegin = true;
			tapStartPos = Input.GetTouch(0).position;
		}
		if(tapBegin && Input.touchCount != 1)
		{
			tapBegin = false;
		}
		if(tapBegin && Input.touchCount == 1)
		{
			if((tapStartPos - Input.GetTouch(0).position).sqrMagnitude > sqrMinMoveMagnitude)
			{
				tapBegin = false;
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				tapBegin = false;
				GameManager.ThePlayerController.Clicked(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f), GetTouchPlanePosition(0));
			}
		}


		//Debug.LogFormat("t: {0}, clickbegin {1}, md {2}, mu {3}", Time.time, clickBegin, Input.GetMouseButtonDown(0), Input.GetMouseButtonUp(0));

		if(!Input.mousePresent)
		{
			return;
		}
		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(-1))
		{
			clickBegin = true;
			clickStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}
		if(clickBegin)
		{
			if((clickStartPos - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).sqrMagnitude > sqrMinMoveMagnitude)
			{
				clickBegin = false;
			}
			else if(Input.GetMouseButtonUp(0))
			{
				clickBegin = false;
				GameManager.ThePlayerController.Clicked(Input.mousePosition, GetTouchPlanePosition(-1));
			}
		}
	}





	void DebugAll()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("tc: {0}\n", Input.touchCount);
		for(int i = 0; i < Input.touchCount; i++)
		{
			sb.AppendFormat("{0}: {1}\n", i, Input.GetTouch(i).phase.ToString());
		}
		Debug.Log(sb.ToString());
	}
}
