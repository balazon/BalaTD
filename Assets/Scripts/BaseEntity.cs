using UnityEngine;
using System.Collections;






public class BaseEntity : MonoBehaviour {

	public ETeamEnum team;

	public string iconName;

	public string displayName;

	public bool KillPending {get; private set;}

	public MyObservable observable {get; private set;}

	protected virtual void Awake()
	{
		KillPending = false;
		observable = new MyObservable();

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	protected void notifyObservers()
//	{
//		if(notify != null)
//		{
//			notify();
//		}
//	}

	//Destroys gameobject, but notifies observers first
	public void Finished()
	{
		KillPending = true;
		observable.notifyObservers();
		Destroy(gameObject);
	}
}
