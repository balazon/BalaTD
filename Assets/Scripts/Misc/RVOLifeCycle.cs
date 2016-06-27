using UnityEngine;
using System.Collections;

public class RVOLifeCycle : MonoBehaviour {

	AIPath aipath;

	protected bool inited = false;
	void Awake()
	{

	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(!inited)
		{
			init ();
			inited = true;
		}
	}

	void init()
	{
		aipath = GetComponent<AIPath>();
		MyRVOManager.GetInstance().Register(aipath);
	}

	void OnEnable()
	{

	}

	void OnDisable()
	{
		
	}


	void OnDestroy()
	{
		MyRVOManager.GetInstance().UnRegister(aipath);
	}
}
