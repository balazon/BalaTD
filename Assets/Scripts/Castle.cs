using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Castle : MonoBehaviour {

	public int maxLives = 10;

	public int Lives { get; private set;}


	public MyObservable observable {get; private set;}

	void Awake()
	{
		observable = new MyObservable();
	}

	// Use this for initialization
	void Start () {
		Reset ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Reset()
	{
		Lives = maxLives;
		observable.notifyObservers();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Critter")
		{
			if(Lives > 0)
			{
				Lives -= other.gameObject.GetComponent<Critter>().castleDamage;
				observable.notifyObservers();
			}

			if(Lives <= 0)
			{
				GameManager.ThePlayer.GameFinished();

			}
			other.gameObject.GetComponent<Critter>().EnteredCastle();
		}
	}
}
