using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//for making pause happen only from in_game, no need for pause in the menu
public enum EControllerState {IN_MENU, IN_GAME};

//This class handles general input (zooming, panning, clicks not on UI elements, etc), and stores the player's selected object
// (concrete button clicks are handled in GuiHandler)
public class PlayerController : MonoBehaviour {

	public LayerMask layerMask;
	public Camera cam;






	public GameObject selected;

	public Vector3 CursorPlanePosition {get; private set;}

	public EControllerState State {get; set;}


	public MyObservable observable {get; private set;}


	void Awake()
	{
		
		observable = new MyObservable();

		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () {
		State = EControllerState.IN_MENU;

		Input.simulateMouseWithTouches = false;
		Debug.LogFormat("sim mouse: {0}", Input.simulateMouseWithTouches);
	}



	// Update is called once per frame
	void Update () {
		
		checkPause();



	}





	public void SetNewSelected(GameObject go)
	{
		DeselectOld();

		selected = go;

		if(go != null)
		{
			go.GetComponent<IMySelectable>().SetSelected(true);
		}
		observable.notifyObservers();
	}

	IMySelectable DeselectOld()
	{
		//deselect older selection
		IMySelectable oldSel = null;
		if(selected != null)
		{
			oldSel = selected.GetComponent<IMySelectable>();
			if(oldSel != null)
			{
				oldSel.SetSelected(false);
			}
		}

		return oldSel;
	}

	public void Clicked(Vector3 CursorScreenPosition, Vector3 CursorPlanePosition)
	{
		this.CursorPlanePosition = CursorPlanePosition;
		handleClicked(CursorScreenPosition);
		observable.notifyObservers();
	}

	void handleClicked(Vector3 CursorScreenPosition)
	{
		Debug.Log("clicked");

		IMySelectable oldSel = DeselectOld();
		
		
		RaycastHit hit;
		//Ray r = cam.ScreenPointToRay(Input.mousePosition);
		Ray r = cam.ScreenPointToRay(CursorScreenPosition);
		if(Physics.Raycast(r, out hit, Mathf.Infinity, layerMask))
		{
			GameObject go = hit.collider.gameObject;
			Debug.LogFormat("GameObject hit: {0}", go.name);
			IMySelectable sel = go.GetComponent<IMySelectable>();



			if(sel == null)
			{
				sel = go.GetComponentInParent<IMySelectable>();
				if(sel != null)
				{
					go = go.transform.parent.gameObject;
				}
				else
				{
					go = clickedPlane();
					if(go == null)
					{
						selected = null;
						return;
					}
					sel = go.GetComponent<IMySelectable>();
				}
			}



			if(sel.SameAs(oldSel))
			{
				selected = null;
				return;
			}

			if(sel != null)
			{
				selected = go;
				sel.SetSelected(true);
			}
		
		}
		else
		{
			Debug.Log("no GameObject hit");
			if(clickedPlane() == null)
			{
				selected = null;
				return;
			}
		}

	}

	public GameObject TowerPlaceAt(Vector3 pos)
	{
		var placeSel = GameObject.Find("TowerPlaceSelection");
		var towPS = placeSel.GetComponent<TowerPlaceSelection>();

		int i, j;
		if(!BuildingManager.Instance.GetCoordsFromVector(pos, out i, out j))
		{
			return null;
		}
		towPS.SetNewCoords(i, j);

		return placeSel;
	}

	GameObject clickedPlane()
	{
		Tower t = BuildingManager.Instance.GetTowerAtVec(CursorPlanePosition);
		
		if(t != null)
		{
			t.SetSelected(true);
			return t.gameObject;
		}
		else
		{
			return TowerPlaceAt(CursorPlanePosition);
		}
	}



	public Tower GetSelectedTower()
	{
		Tower t = null;
		if(selected != null)
		{
			t = selected.GetComponent<Tower>();
		}
		return t;
	}

	void checkPause()
	{
		if(State == EControllerState.IN_GAME && (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape)) )
		{
			GameManager.ThePlayer.Paused = !GameManager.ThePlayer.Paused;
		}
	}


	void OnApplicationFocus(bool focusStatus)
	{
		if(State == EControllerState.IN_GAME && !focusStatus)
		{
			GameManager.ThePlayer.Paused = !focusStatus;

		}
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if(State == EControllerState.IN_GAME && pauseStatus)
		{
			GameManager.ThePlayer.Paused = pauseStatus;

		}
	}







//
//
//	//input detection starts here - scroll, pinch, pan, click
//
//	bool scrolling;
//	void handleScrolling()
//	{
//		var d = Input.GetAxis("Mouse ScrollWheel");
//		if(Mathf.Abs(d) > 0.1)
//		{
//			if(!scrolling)
//			{
//				targetfov = cam.fieldOfView;
//			}
//			scrolling = true;
//			var relfov = (d > 0) ? -5.0f : 5.0f;
//			targetfov += relfov;
//			targetfov = Mathf.Clamp(targetfov, 10.0f, 80.0f);
//		}
//		if(scrolling)
//		{
//			cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetfov, ref scrollv, 0.1f);
//		}
//		if(Mathf.Abs(cam.fieldOfView - targetfov) < 0.1)
//		{
//			scrolling = false;
//		}
//
//	}
//
//
//	void handlePinchZoom()
//	{
//		if(Input.touchCount > 1)
//		{
//			var t1 = Input.GetTouch(0);
//			var t2 = Input.GetTouch(1);
//
//			//var oldDist = ((t2.position - t2.deltaPosition * (Time.deltaTime / t2.deltaTime)) - (t1.position - t1.deltaPosition * (Time.deltaTime / t1.deltaTime))).magnitude;
//			var oldDist = ((t2.position - t2.deltaPosition) - (t1.position - t1.deltaPosition)).magnitude;
//			var dist = (t2.position - t1.position).magnitude;
//
//			float newfov = Mathf.Clamp(cam.fieldOfView * oldDist / dist, 5.0f, 80.0f);
//			cam.fieldOfView = newfov;
//
//		}
//	}
//
//	Vector3 StartMousePlanePos;
//	bool mousePanning;
//
//	void handlePanning()
//	{
//		var MouseCurrentPlanePos = GetTouchPlanePosition(-1);
//
//
////		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(-1))
////		{
////			StartMousePlanePos = MouseCurrentPlanePos;
////			mousePanning = true;
////		}
//
//
//		Vector3 avgPlanePos = Vector3.zero;
//		Vector3 avgDelta = Vector3.zero;
//		if(Input.multiTouchEnabled && Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(0))
//		{
//			Debug.LogFormat("Pan: tc {0}", Input.touchCount);
//			for(int i = 0; i < Input.touchCount; i++)
//			{
//				var touch = Input.GetTouch(i);
//				var touchPos = GetTouchPlanePosition(i);
//				var delta = touchPos - GetPlanePosition(touch.position - touch.deltaPosition * (Time.deltaTime / touch.deltaTime));
//				avgPlanePos += touchPos;
//				avgDelta += delta;
//				Debug.LogFormat("delta: {0}", delta);
//			}
//			avgPlanePos /= Input.touchCount;
//			avgDelta /= Input.touchCount;
//			Debug.LogFormat("avgdelta: {0}", avgDelta);
//		}
////		if(mousePanning && Input.mousePresent)
////		{
////			Debug.LogFormat("OH NOoo - mouse present?");
////			avgPlanePos = MouseCurrentPlanePos;
////
////			avgDelta = GetTouchPlanePosition(-1) - StartMousePlanePos;
////		}
//
//		if(mousePanning || Input.touchCount > 0)
//		{
//			Debug.LogFormat("avgdelta2: {0}", avgDelta);
//			avgDelta.y = 0;
//			cam.transform.position -= avgDelta;
//		}
//
//		if(Input.GetMouseButtonUp(0))
//		{
//			mousePanning = false;
//		}
//	}
//
//	bool tapBegin;
//	Vector2 tapStartPos;
//	bool clickBegin;
//	Vector2 clickStartPos;
//	void handleClick()
//	{
//		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(0))
//		{
//			tapBegin = true;
//			tapStartPos = Input.GetTouch(0).position;
//		}
//		if(tapBegin && Input.touchCount != 1)
//		{
//			tapBegin = false;
//		}
//		if(tapBegin && Input.touchCount == 1)
//		{
//			if((tapStartPos - Input.GetTouch(0).position).sqrMagnitude > sqrMinMoveMagnitude)
//			{
//				tapBegin = false;
//			}
//			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
//			{
//				tapBegin = false;
//				CursorPlanePosition = GetTouchPlanePosition(0);
//				clicked();
//				observable.notifyObservers();
//			}
//		}
//
//
//		//Debug.LogFormat("t: {0}, clickbegin {1}, md {2}, mu {3}", Time.time, clickBegin, Input.GetMouseButtonDown(0), Input.GetMouseButtonUp(0));
//
//		if(!Input.mousePresent)
//		{
//			return;
//		}
//		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject(-1))
//		{
//			clickBegin = true;
//			clickStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//		}
//		if(clickBegin)
//		{
//			if((clickStartPos - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).sqrMagnitude > sqrMinMoveMagnitude)
//			{
//				clickBegin = false;
//			}
//			else if(Input.GetMouseButtonUp(0))
//			{
//				clickBegin = false;
//				CursorPlanePosition = GetTouchPlanePosition(-1);
//				clicked();
//				observable.notifyObservers();
//			}
//		}
//	}


}
