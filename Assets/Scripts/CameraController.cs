using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour {

	//tapping and dragging less then this means a click
	public float sqrMinMoveMagnitude = 100.0f;

	protected float targetfov;

	Vector3[] boundPositions;
	Vector3[] translates;
	Camera cam;


	public float minDistance = 10.0f;
	public float maxDistance = 80.0f;

	//these variables are used for scrollzooming
	public float zoomIncrement = 0.1f;
	protected float zoomTranslateRange;
	protected float currentTranslation;
	protected float scrollV = 0.0f;
	protected float targetTranslation;
    protected float startTranslation;

	public float panBoundX = 50.0f;
	public float panBoundMinZ = -70.0f;
	public float panBoundMaxZ = 30.0f;
	//panDistortionZ is how much the initial panning boundary has to be translated on the z axis for delta y = 1.0f height reduction
	// This is needed because the camera looks at the map at an angle, so the boundary is a skewed box (a parallelepiped with a rect at top/bot)
	protected float panDistortionZ;
	protected float camStartY;
	

	void Awake()
	{
		boundPositions = new Vector3[15];
		translates = new Vector3[15];
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        targetTranslation = currentTranslation;
        
    }
	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches = false;

		targetfov = cam.fieldOfView;
		scrolling = false;


		Vector3 planeAtCenterPos = GetPlanePosition(new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
		float distance = Vector3.Distance(planeAtCenterPos, cam.transform.position);
		
		zoomTranslateRange = maxDistance - minDistance;
		currentTranslation = (maxDistance - distance) / zoomTranslateRange;
		startTranslation = currentTranslation;
		panDistortionZ = cam.transform.forward.z / cam.transform.forward.y;
		camStartY = cam.transform.position.y;
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
			return hitPos;
		}
		return Vector3.zero;
	}


	//index of -1 means mouse
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

		if(Input.multiTouchEnabled && Input.touchCount == 0)
		{
			sum = Vector3.zero;
		}

		if(Input.multiTouchEnabled && Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(0))
		{
			touchPan = true;

			for(int i = 0; i < Input.touchCount; i++)
			{
				int id = Input.GetTouch(i).fingerId;
				if(Input.GetTouch(i).phase == TouchPhase.Began)
				{
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
				}
			}

			

		}

		if(mousePanning && Input.mousePresent)
		{

			tr = StartMousePlanePos - GetTouchPlanePosition(-1);
		}


		if(mousePanning || touchPan)
		{
			tr.y = 0;
			sum.y = 0;

			Vector3 camPos = cam.transform.position;
			float zDist = (camPos.y - camStartY) * panDistortionZ;

			sum = new Vector3(Mathf.Clamp(sum.x, -panBoundX, panBoundX), sum.y, Mathf.Clamp(sum.z, panBoundMinZ + zDist, panBoundMaxZ + zDist));
            cam.transform.position += tr + sum;

			camPos = cam.transform.position;

			cam.transform.position = new Vector3(Mathf.Clamp(camPos.x, -panBoundX, panBoundX), camPos.y, Mathf.Clamp(camPos.z, panBoundMinZ + zDist, panBoundMaxZ + zDist));
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
			Vector3 planeAtCenterPos = GetPlanePosition(new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
			float distance = Vector3.Distance(planeAtCenterPos, cam.transform.position);
			
            float diff = (boundPositions[0] - boundPositions[1]).magnitude / (GetTouchPlanePosition(0) - GetTouchPlanePosition(1)).magnitude;

			float newDist = Mathf.Clamp(diff * distance, minDistance, maxDistance);
			cam.transform.position = planeAtCenterPos + cam.transform.forward * (-newDist);

			//sync scrolling variable
			currentTranslation = (maxDistance - newDist) / zoomTranslateRange;
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
				targetTranslation = currentTranslation;
			}
			scrolling = true;
            var relInc = (d > 0) ? zoomIncrement : -zoomIncrement;
            targetTranslation = Mathf.Clamp01(targetTranslation + relInc);
		}
		if(scrolling)
		{
            float newTranslation = Mathf.SmoothDamp(currentTranslation, targetTranslation, ref scrollV, 0.1f);
            cam.transform.position = cam.transform.position + cam.transform.forward * (newTranslation - currentTranslation) * zoomTranslateRange;
            currentTranslation = newTranslation;
        }
		if(Mathf.Abs(currentTranslation - targetTranslation) < 0.02)
		{
            float newTranslation = targetTranslation;
            cam.transform.position = cam.transform.position + cam.transform.forward * (newTranslation - currentTranslation) * zoomTranslateRange;
            currentTranslation = newTranslation;
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
