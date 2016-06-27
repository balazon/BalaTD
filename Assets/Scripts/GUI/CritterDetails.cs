using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CritterDetails : MonoBehaviour {

	Critter critter;

	protected GameObject critterDetailsPanel;

	protected Text critterNameText;
	protected Text critterText;
	protected Image critterHealthBarImage;
	protected Text critterHealthBarText;
	protected Image critterIcon;

	void Awake()
	{
		critterDetailsPanel = GameObject.Find("GUI").transform.Find("HUD").Find("DetailsPanel").Find("CritterDetailsPanel").gameObject;

		critterNameText = critterDetailsPanel.transform.Find("Name").GetComponent<Text>();
		critterText = critterDetailsPanel.transform.Find("Text").GetComponent<Text>();
		critterHealthBarImage = critterDetailsPanel.transform.Find("HealthBar").Find("HealthProgressBar").GetComponent<Image>();
		critterHealthBarText = critterDetailsPanel.transform.Find("HealthBar").Find("Text").GetComponent<Text>();
		critterIcon = critterDetailsPanel.transform.Find("IconBg").Find("Icon").GetComponent<Image>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Register(Critter c)
	{
		if(critter != null && critter.observable.notify != null)
		{
			critter.observable.notify -= UpdateDetails;
		}
		critter = c;

		if(critter == null)
		{
			return;
		}
		critter.observable.notify += UpdateDetails;
	}

	void Unregister()
	{
		if(critter != null)
		{
			critter.observable.notify -= UpdateDetails;
		}
	}

	public void UpdateDetails()
	{
		Unregister();
		var s = GameManager.ThePlayerController.selected;
		Critter c = null;
		if(s != null)
		{
			c = s.GetComponent<Critter>();
		}
		if(c == null)
		{
			//disable critter details
			critterDetailsPanel.SetActive(false);
			if(critter != null)
			{
				critter.observable.notify -= UpdateDetails;
			}
			return;
		}

		critterDetailsPanel.SetActive(true);
		Register (c);
		UpdateCritterDetailsContent();
		
	}

	void UpdateCritterDetailsContent()
	{
		critterNameText.text = critter.displayName;
		critterText.text = string.Format("Score: {0}\nGold: {1}\nCastle damage: {2}", critter.scoreWorth, critter.goldWorth, critter.castleDamage);
		critterHealthBarImage.fillAmount = critter.maxHealth > 0.0f ? critter.health / critter.maxHealth : 0.0f;
		critterHealthBarText.text = string.Format("{0:0.#}/{1:0.#}", critter.health, critter.maxHealth);

		critterIcon.sprite = GuiHandler.Instance.GetIcon(critter.iconName);
	}
}
