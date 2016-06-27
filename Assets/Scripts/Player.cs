using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	protected Castle castle;

	public int StartingMoney = 5;

	public int CurrentScore = 0;
	
	public int CurrentMoney = 0;

	protected float smoothTime = 0.2f;


	protected Vector3 velocity = Vector3.zero;

	protected GameObject cam;



	protected Spawner spawner;

	public Selectable selected {get; private set;}


	public int FinishedScore {get; private set;}

	public bool IsFinished {get; private set;}

	private bool paused;
	public bool Paused
	{
		get {return paused;}
		set
		{
			paused = value;
			if(paused)
			{
				Time.timeScale = 0.0f;
				AudioListener.pause = true;
			}
			else
			{
				Time.timeScale = 1.0f;
				AudioListener.pause = false;
			}
			observable.notifyObservers();
		}
	}

	public MyObservable observable {get; private set;}

	void Awake()
	{
		observable = new MyObservable();

		Paused = false;

		castle = GameObject.FindGameObjectWithTag("Castle").GetComponent<Castle>();

		spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();

		cam = GameObject.FindGameObjectWithTag("MainCamera");


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
		


	public void IncreaseScore(int score)
	{
		if(IsFinished)
		{
			return;
		}
		CurrentScore += score;
		observable.notifyObservers();
	}

	public void IncreaseMoney(int money)
	{
		if(IsFinished)
		{
			return;
		}
		CurrentMoney += money;
		observable.notifyObservers();
	}


	public void StartGame()
	{
		Reset();

		GameManager.ThePlayerController.State = EControllerState.IN_GAME;

		spawner.StartWaves();

		observable.notifyObservers();
	}

	public void Reset()
	{ 
		Paused = false;

		IsFinished = false;
		FinishedScore = 0;
		BuildingManager.Instance.Reset();
		spawner.Reset();
		castle.Reset();

		GameManager.ThePlayerController.State = EControllerState.IN_MENU;
		GameManager.ThePlayerController.SetNewSelected(null);

		CurrentMoney = StartingMoney;
		CurrentScore = 0;
	}
	
	public void GameFinished()
	{
		if(IsFinished)
		{
			return;
		}
		IsFinished = true;
		Debug.Log ("gamefinished");
		int bonusScore = castle.Lives * 100;
		FinishedScore = CurrentScore + bonusScore;

		GameManager.ThePlayerController.State = EControllerState.IN_MENU;

		observable.notifyObservers();
	}

	public bool canAffordOption(string name, ref int cost)
	{
		ETower type = ETower.Electro;
		if(Configurations.TowerTypeFromName(name, ref type))
		{
			cost = Configurations.Instance.getCostOfTower(type, 1);
			return cost <= CurrentMoney;
		}
		else if(name == "Upgrade")
		{
			Tower t = GameManager.ThePlayerController.GetSelectedTower();
			if(t != null && Configurations.Instance.getMaxLevel(t.type) > t.level)
			{
				cost = Configurations.Instance.getCostOfTower(t.type, t.level + 1);
				return cost <= CurrentMoney;
			}
			return false;
		}
		return true;
	}

}
