using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {

	public static GameManager Instance{ get; private set;}

	public static Player ThePlayer {get; private set;}

	public static PlayerController ThePlayerController {get; private set;}

	public static Castle TheCastle {get; private set;}

	public static GameObject DynamicObjectContainer {get; private set;}

	void Awake()
	{
		Instance = this;

		ThePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		ThePlayerController = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerController>();

		TheCastle = GameObject.FindGameObjectWithTag("Castle").GetComponent<Castle>();

		DynamicObjectContainer = GameObject.Find("Dynamic Objects");

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DestroyDynamic()
	{
		
		Transform[] allTransforms = DynamicObjectContainer.GetComponentsInChildren<Transform>();
		foreach (Transform child in allTransforms)
		{
			if(child == DynamicObjectContainer.transform)
			{
				continue;
			}
			Destroy(child.gameObject);
		}
	}

}
