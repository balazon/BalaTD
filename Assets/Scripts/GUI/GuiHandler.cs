using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GuiHandler : MonoBehaviour {

	public string[] iconKeys;
	public Sprite[] iconVals;



	protected Dictionary<string, Sprite> iconMap;



	public static GuiHandler Instance{ get; private set;}



	protected GameObject mainMenuContext;
	public GameObject buildingTowerContext {get; private set;}
	protected GameObject selectedTowerContext;
	protected GameObject highscoresContext;
	protected GameObject instructionsContext;
	protected GameObject gameOverContext;
	protected GameObject pauseMenuContext;

	//building or a selectedtower
	protected GameObject currentContext;

	protected GameObject detailsPanel;

	public GameObject towerDetailsPanel {get; private set;}

	public GameObject critterDetailsPanel {get; private set;}



	protected Text ScoreText;
	protected Text GoldText;
	protected Text LivesText;
	protected Text GameOverScoreText;

	protected InputField gameOverName;

	protected GameObject hud;




	public string PointerOverObjectName {get; private set;}

	public MyObservable PointerOverObjectChanged {get; private set;}

	void Awake()
	{
		Instance = this;

		PointerOverObjectChanged = new MyObservable();

		



		mainMenuContext = transform.Find("MainMenu").gameObject;

		highscoresContext = transform.Find("Highscores").gameObject;

		instructionsContext = transform.Find("Instructions").gameObject;

		hud = transform.Find("HUD").gameObject;

		buildingTowerContext = hud.transform.Find("FourTowers").gameObject;
		selectedTowerContext = hud.transform.Find ("SelectedTower").gameObject;

		
		gameOverContext = transform.Find("GameOverScreen").gameObject;
		gameOverName = gameOverContext.transform.Find("GameOverNameInputField").GetComponent<InputField>();
		GameOverScoreText = gameOverContext.transform.Find("GameOverScoreText").GetComponent<Text>();
		
		pauseMenuContext = transform.Find("PauseMenu").gameObject;




		ScoreText = hud.transform.Find("ScorePanel").Find("ScoreText").GetComponent<Text>();
		
		GoldText = hud.transform.Find("LivesAndGoldPanel").Find("GoldNum").GetComponent<Text>();

		LivesText = hud.transform.Find("LivesAndGoldPanel").Find("LivesNum").GetComponent<Text>();

		detailsPanel = hud.transform.Find("DetailsPanel").gameObject;
		towerDetailsPanel = detailsPanel.transform.Find("TowerDetailsPanel").gameObject;
		critterDetailsPanel = detailsPanel.transform.Find("CritterDetailsPanel").gameObject;




		iconMap = new Dictionary<string, Sprite>();
		for(int i = 0; i < iconKeys.Length; i++)
		{
			iconMap[iconKeys[i]] = iconVals[i];
		}
	}

	public Sprite GetIcon(string name)
	{
		return iconMap[name];
	}

	// Use this for initialization
	void Start () {
		GameManager.ThePlayer.observable.notify += EventPlayerChanged;
		GameManager.ThePlayerController.observable.notify += EventPlayerControllerClicked;
		GameManager.TheCastle.observable.notify += EventCastleChanged;

		ShowMain();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowMain()
	{
		DisableAllGUI();
		ActivateContext(mainMenuContext);
	}

	public void ShowHighScores()
	{
		DisableAllGUI();
		highscoresContext.SetActive(true);
		var firstChild = highscoresContext.transform.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstChild);
		var hd = transform.Find("HighscoreUpdater").GetComponent<HighscoreDisplay>();
		hd.FillContent();
	}

	public void ShowInstructions()
	{
		DisableAllGUI();
		instructionsContext.SetActive(true);
		var firstChild = instructionsContext.transform.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstChild);
	}

	public void Exit()
	{
		Application.Quit();
	}

	public void NewGame()
	{
		//DisableAllGUI();
		ShowHUD();
		GameManager.ThePlayer.StartGame();
	}

	public void ShowHUD()
	{
		DisableAllGUI();

		hud.SetActive(true);
		detailsPanel.SetActive(true);
	}

	public void UpdateBuildingOptions()
	{
		var s = GameManager.ThePlayerController.selected;
		TowerPlaceSelection tps = null;
		Tower t = null;
		if(s != null)
		{
			tps = s.GetComponent<TowerPlaceSelection>();
			t = s.GetComponent<Tower>();
		}
		if(tps != null && BuildingManager.Instance.CanBuildAt(tps.i, tps.j) && !GameManager.ThePlayer.Paused)
		{
			ActivateContext(buildingTowerContext);
		}
		else if(t != null && !GameManager.ThePlayer.Paused)
		{
			ActivateContext(selectedTowerContext);
		}
		else
		{
			ActivateContext(null);
		}


	}

	void ActivateContext(GameObject context)
	{
		buildingTowerContext.SetActive(false);
		selectedTowerContext.SetActive(false);

		currentContext = context;
		if(currentContext != null)
		{
			currentContext.SetActive(true);


			var firstChild = currentContext.transform.GetChild(0).gameObject;
			EventSystem.current.SetSelectedGameObject(firstChild);
		}

		UpdateInteractable();
	}
	

	public void ShowGameOverScreen()
	{
		
		Debug.Log("Showgameoverscreen");
		DisableAllGUI();
		gameOverContext.SetActive(true);
		var p = GameManager.ThePlayer;
		GameOverScoreText.text = "" + p.FinishedScore;


	}

	public void GameOverOkClicked()
	{
		if(HighscoreManager.Instance.NewHighscore(gameOverName.text, GameManager.ThePlayer.FinishedScore))
		{
			GameManager.ThePlayer.Reset();
			ShowHighScores();

		}
		else
		{
			//failed to save highscore - invalid name?
		}


	}
	
	public void ShowPauseMenu()
	{
		//					UIFX.Instance.PlayAudioClip(2);
		//					state = player.NavigateContextMenu(CurrentMove, state, EStateEnum.PauseMenu);
		//					Time.timeScale = 0.0f;

		pauseMenuContext.SetActive(true);
		var firstChild = pauseMenuContext.transform.GetChild(0).gameObject;
		//EventSystem.current.firstSelectedGameObject = firstChild;


		EventSystem.current.SetSelectedGameObject(firstChild);

	}

	void hidePauseMenu()
	{
		pauseMenuContext.SetActive(false);
	}

	public void ResumeClicked()
	{
		GameManager.ThePlayer.Paused = false;
	}

	public void ToMainClicked()
	{
		GameManager.ThePlayer.Reset();
		ShowMain();
	}



	public void PointerOverTowerType(string type)
	{
		Debug.LogFormat("pointer event: {0}", type);
		PointerOverObjectName = type;
		PointerOverObjectChanged.notifyObservers();

	}

	public void CancelButtonClicked()
	{
		if(GameManager.ThePlayer.Paused)
		{
			return;
		}
		GameManager.ThePlayerController.SetNewSelected(null);
	}

	public void BuildTowerString(string nameOfType)
	{
		if(GameManager.ThePlayer.Paused)
		{
			return;
		}
		PointerOverObjectName = "";
		ETower type = ETower.Venom;
		if(Configurations.TowerTypeFromName(nameOfType, ref type))
		{
			var towerPlace = GameManager.ThePlayerController.selected.GetComponent<TowerPlaceSelection>();
			int i, j;
			i = towerPlace.i;
			j = towerPlace.j;
			int cost = 0;
			if(BuildingManager.Instance.CanBuildAt(i, j) && GameManager.ThePlayer.canAffordOption(nameOfType, ref cost))
			{
				GameManager.ThePlayer.IncreaseMoney(-cost);
				var tower = BuildingManager.Instance.buildTower(type, i, j);
				GameManager.ThePlayerController.SetNewSelected(tower.gameObject);
			}


		}
		EventPlayerControllerClicked();
	}

	public void UpgradeTower()
	{
		if(GameManager.ThePlayer.Paused)
		{
			return;
		}
		Tower t = GameManager.ThePlayerController.GetSelectedTower();
		int cost = 0;
		if(GameManager.ThePlayer.canAffordOption("Upgrade", ref cost))
		{
			t.Upgrade();
			GameManager.ThePlayer.IncreaseMoney(-cost);
		}

		EventPlayerControllerClicked();
	}

	public void SellTower()
	{
		var s = GameManager.ThePlayerController.selected;
		Tower t = null;
		if(s != null)
		{
			t = s.GetComponent<Tower>();
		}

		if(t == null)
		{
			return;
		}

		var towPlace = GameManager.ThePlayerController.TowerPlaceAt(t.gameObject.transform.position);

		GameManager.ThePlayer.IncreaseMoney(BuildingManager.Instance.sellTower(t));

		GameManager.ThePlayerController.SetNewSelected(towPlace);


		EventPlayerControllerClicked();
	}


	public void DisableAllGUI()
	{
		if(hud.activeSelf) hud.SetActive(false);
		if(detailsPanel.activeSelf) detailsPanel.SetActive(false);
		if(mainMenuContext.activeSelf) mainMenuContext.SetActive(false);
		if(buildingTowerContext.activeSelf) buildingTowerContext.SetActive(false);
		if(selectedTowerContext.activeSelf) selectedTowerContext.SetActive(false);
		if(highscoresContext.activeSelf) highscoresContext.SetActive(false);
		if (instructionsContext.activeSelf) instructionsContext.SetActive(false);
        if (gameOverContext.activeSelf) gameOverContext.SetActive(false);
		if(pauseMenuContext.activeSelf) pauseMenuContext.SetActive(false);
		if(towerDetailsPanel.activeSelf) towerDetailsPanel.SetActive(false);
		if(critterDetailsPanel.activeSelf) critterDetailsPanel.SetActive(false);
	}



	



	public void ShowDetails(GameObject panel)
	{
		if(panel == null)
		{
			detailsPanel.SetActive(false);
			return;
		}

		detailsPanel.SetActive(true);
		foreach(Transform child in detailsPanel.transform)
		{
			child.gameObject.SetActive(false);
		}

		UpdateInteractable();
		panel.SetActive(true);
		
	}

	void UpdateInteractable()
	{
		if(currentContext == null)
		{
			return;
		}
		foreach (Transform child in currentContext.transform)
		{
			var go = child.gameObject;
			var btn = go.GetComponent<Button>();
			int cost = 0;
			btn.interactable = GameManager.ThePlayer.canAffordOption(go.name, ref cost);
		}
	}

	public void EventPlayerChanged()
	{
		
		ScoreText.text = "" + GameManager.ThePlayer.CurrentScore;
		GoldText.text = "" + GameManager.ThePlayer.CurrentMoney;
		if(GameManager.ThePlayer.IsFinished)
		{
			ScoreText.text = "" + GameManager.ThePlayer.FinishedScore;
			ShowGameOverScreen();
		}
		else if(GameManager.ThePlayer.Paused)
		{
			ShowPauseMenu();
		}
		else
		{
			hidePauseMenu();
		}

		UpdateInteractable();
		
	}

	public void EventCastleChanged()
	{
		LivesText.text = "" + GameManager.TheCastle.Lives;
	}

	public void EventPlayerControllerClicked()
	{

		var td = detailsPanel.GetComponent<TowerDetails>();
		var cd = detailsPanel.GetComponent<CritterDetails>();



		UpdateBuildingOptions();


		td.UpdateDetails();
		cd.UpdateDetails();
	}



	//


	//public void ShowDetails(ESelectableEnum 


}
