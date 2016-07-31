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


}
