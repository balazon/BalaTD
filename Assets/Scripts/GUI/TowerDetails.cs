using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerDetails : MonoBehaviour {

	Tower tower;

	GameObject towerDetailsPanel;

	void Awake()
	{
		towerDetailsPanel = transform.Find("TowerDetailsPanel").gameObject;
	}

	// Use this for initialization
	void Start () {
		GuiHandler.Instance.PointerOverObjectChanged.notify += UpdateDetails;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Register(Tower t)
	{
		if(tower != null && tower.observable.notify != null)
		{
			tower.observable.notify -= UpdateDetails;
		}
		tower = t;
		tower.observable.notify += UpdateDetails;
	}

	void Unregister()
	{
		if(tower != null)
		{
			tower.observable.notify -= UpdateDetails;
		}
	}

	public void UpdateDetails()
	{
		Unregister();

		var s = GameManager.ThePlayerController.selected;
		if(s == null)
		{
			//disable tower details
			towerDetailsPanel.SetActive(false);
			return;
		}
		var towerSel = s.GetComponent<TowerPlaceSelection>();
		if(towerSel != null)
		{
			ETower type = ETower.Electro;
			if(Configurations.TowerTypeFromName(GuiHandler.Instance.PointerOverObjectName, ref type))
			{
				towerDetailsPanel.SetActive(true);
				UpdateTowerDetailsContent(type, 0);
				return;
			}
			else
			{
				towerDetailsPanel.SetActive(false);
				return;
			}
		}
		var t = s.GetComponent<Tower>();
		if(t != null)
		{
			towerDetailsPanel.SetActive(true);
			Register (t);
			UpdateTowerDetailsContent(t.type, t.level);
		}
		else
		{
			towerDetailsPanel.SetActive(false);
		}


	}
	
	public void UpdateTowerDetailsContent(ETower type, int level)
	{
		GameObject panel = towerDetailsPanel;
		Image icon = panel.transform.Find("Icon").GetComponent<Image>();
		var currentLevelText = panel.transform.Find("CurrentLevelText").GetComponent<Text>();
		var NextLevelText = panel.transform.Find("NextLevelText").GetComponent<Text>();
		var SellText = panel.transform.Find("SellText").GetComponent<Text>();
		
		
		
		//icon.sprite = iconMap[Configurations.Instance.GetBaseTowerPrefab(type).GetComponent<Tower>().iconName];
		//icon.sprite = iconMap[Configurations.TowerNameFromType(type)];

		icon.sprite = GuiHandler.Instance.GetIcon(type.ToString());
		
		
		
		var sb = new System.Text.StringBuilder();
		
		if(level == 0)
		{
			
			sb.Append(GetStringTowerDetails(type, 1));
			sb.AppendFormat("\nCost: {0}", Configurations.Instance.getCostOfTower(type, 1));
			currentLevelText.text = sb.ToString();
			NextLevelText.text = "";
			SellText.text = "";
			return;
		}
		
		sb.Append(GetStringTowerDetails(type, level));
		
		currentLevelText.text = sb.ToString();
		
		sb = new System.Text.StringBuilder();
		sb.Append("Next level\n");
		if(Configurations.Instance.getMaxLevel(type) == level)
		{
			sb.AppendFormat("Maximum lvl reached");
		}
		else
		{
			sb.Append(GetStringTowerDetails(type, level + 1, true));
			sb.AppendFormat("\nCost: {0}", Configurations.Instance.getCostOfTower(type, level + 1));
		}
		
		NextLevelText.text = sb.ToString();
		
		SellText.text = "Sell value: " + Configurations.Instance.getSellingPriceOfTower(type, level);
	}
	
	string GetStringTowerDetails(ETower type, int level, bool ignoreFirstRow = false)
	{
		TowerLevelAttributes attr = Configurations.Instance.GetTowerLevels(type)[level - 1];
		var bulletAttr = attr.bulletAttributes;
		
		var sb = new System.Text.StringBuilder();
		
		
		if(!ignoreFirstRow)
		{
			sb.AppendFormat("{0} Tower Lvl {1}\n",Configurations.TowerNameFromType(type), level);
		}
		
		
		sb.AppendFormat("AttackRate: {0}\nDmg: {1}", attr.attackRate, bulletAttr.baseDamage);
		if(bulletAttr.dotDps > 0)
		{
			sb.AppendFormat(" + {0} over {1}s", bulletAttr.dotDps, bulletAttr.dotDuration);
		}
		if(bulletAttr.slowRate > 0.0f)
		{
			sb.AppendFormat("\nSlows: {0:0.#}% for {1:0.#}s", 100.0f - 100.0f / (1.0f + bulletAttr.slowRate), bulletAttr.slowDuration);
		}
		
		if(bulletAttr.splash)
		{
			sb.Append("\nSplash");
		}
		
		return sb.ToString();
		
	}
}
