using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;




[Serializable]
public struct BulletAttributes
{
	public float baseDamage;
	
	public float dotDps;
	
	public float dotDuration;

	//new speed will be : startingSpeed / (1.0f + slowedRate)
	// for 0.5f -> 0.667 * original
	// for 1.0f -> 0.5 * original
	public float slowRate;
	
	public float slowDuration;
	
	public bool splash;

	public float speed;// 5.0f;
	
	public float acc;// 10.0f;
};

[Serializable]
public struct TowerLevelAttributes
{
	public int cost;
	public float attackRate;
	public BulletAttributes bulletAttributes;
};

public enum ETower {Fire, Venom, Electro, Ice};

public class Configurations : MonoBehaviour {
	
	public static Configurations Instance{ get; private set;}




	public float refundRate = 0.75f;

	public GameObject FireTower;
	public GameObject VenomTower;
	public GameObject ElectroTower;
	public GameObject IceTower;


	public TowerLevelAttributes[] FireLevels;
	public TowerLevelAttributes[] VenomLevels;
	public TowerLevelAttributes[] ElectroLevels;
	public TowerLevelAttributes[] IceLevels;

	Dictionary<ETower, TowerLevelAttributes[]> towerLevels;

	Dictionary<ETower, GameObject> towerPrefabs;

	public Sprite[] towerIcons;

	protected Dictionary<ETower, Sprite> towerIconMap;

	protected GameObject towerDetailsPanel;

	void Awake()
	{
		Instance = this;

		//towerDetailsPanel = GameObject.FindGameObjectWithTag("Player").transform.Find("IngamePanel").Find("TowerDetailsPanel").gameObject;
		 

		towerIconMap = new Dictionary<ETower, Sprite>();
		towerIconMap[ETower.Fire] = towerIcons[0];
		towerIconMap[ETower.Venom] = towerIcons[1];
		towerIconMap[ETower.Electro] = towerIcons[2];
		towerIconMap[ETower.Ice] = towerIcons[3];


		towerLevels = new Dictionary<ETower, TowerLevelAttributes[]>();
		towerLevels[ETower.Fire] = FireLevels;
		towerLevels[ETower.Venom] = VenomLevels;
		towerLevels[ETower.Electro] = ElectroLevels;
		towerLevels[ETower.Ice] = IceLevels;

		towerPrefabs = new Dictionary<ETower, GameObject>();

		towerPrefabs[ETower.Fire] = FireTower;
		towerPrefabs[ETower.Venom] = VenomTower;
		towerPrefabs[ETower.Electro] = ElectroTower;
		towerPrefabs[ETower.Ice] = IceTower;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject GetBaseTowerPrefab(ETower towertype)
	{
		return towerPrefabs[towertype];
	}

	public TowerLevelAttributes[] GetTowerLevels(ETower type)
	{
		return towerLevels[type];
	}
	
	public int getCostOfTower(ETower towertype, int level)
	{

		return towerLevels[towertype][level - 1].cost;
	}

	public int getSellingPriceOfTower(ETower type, int level)
	{
		int sum = 0;
		for(int i = 1; i <= level; i++)
		{
			sum += getCostOfTower(type, i);
		}

		return (int)(sum * refundRate);
	}

	public int getMaxLevel(ETower type)
	{
		return towerLevels[type].Length;
	}


	string GetStringTowerDetails(ETower type, int level, bool ignoreFirstRow = false)
	{
		TowerLevelAttributes attr = GetTowerLevels(type)[level - 1];
		var bulletAttr = attr.bulletAttributes;
		
		var sb = new System.Text.StringBuilder();

		if(!ignoreFirstRow)
		{
			switch(type)
			{
			case ETower.Fire:
				sb.AppendFormat("Fire");
				break;
			case ETower.Venom:
				sb.AppendFormat("Venom");
				break;
			case ETower.Electro:
				sb.AppendFormat("Electro");
				break;
			case ETower.Ice:
				sb.AppendFormat("Ice");
				break;
			}
			sb.AppendFormat(" Tower Lvl {0}\n", level);
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

	public void UpdateTowerDetailsContent(ETower type, int level)
	{
		GameObject panel = towerDetailsPanel;
		Image icon = panel.transform.Find("Icon").GetComponent<Image>();
		var currentLevelText = panel.transform.Find("CurrentLevelText").GetComponent<Text>();
		var NextLevelText = panel.transform.Find("NextLevelText").GetComponent<Text>();
		var SellText = panel.transform.Find("SellText").GetComponent<Text>();



		icon.sprite = towerIconMap[type];



		var sb = new System.Text.StringBuilder();

		if(level == 0)
		{
			sb.Append(GetStringTowerDetails(type, 1));
			sb.AppendFormat("\nCost: {0}", getCostOfTower(type, 1));
			currentLevelText.text = sb.ToString();
			NextLevelText.text = "";
			SellText.text = "";
			return;
		}

		sb.Append(GetStringTowerDetails(type, level));

		currentLevelText.text = sb.ToString();

		sb = new System.Text.StringBuilder();
		sb.Append("Next level\n");
		if(getMaxLevel(type) == level)
		{
			sb.AppendFormat("Maximum lvl reached");
		}
		else
		{
			sb.Append(GetStringTowerDetails(type, level + 1, true));
			sb.AppendFormat("\nCost: {0}", getCostOfTower(type, level + 1));
		}

		NextLevelText.text = sb.ToString();

		SellText.text = "Sell value: " + getSellingPriceOfTower(type, level);
	}

	public void UpdateCritterDetailsContent(Critter critter)
	{
		//fill panel with  health maxhealth .. gold rewarded for kill, etc.
	}

	public static bool TowerTypeFromName(string name, ref ETower type)
	{
		switch(name)
		{
		case "Fire":
			type = ETower.Fire;
			return true;
		case "Venom":
			type = ETower.Venom;
			return true;
		case "Electro":
			type = ETower.Electro;
			return true;
		case "Ice":
			type = ETower.Ice;
			return true;
		default:
			return false;
		}
	}

	public static string TowerNameFromType(ETower type)
	{
		switch(type)
		{
		case ETower.Fire:
			return "Fire";
		case ETower.Venom:
			return "Venom";
		case ETower.Electro:
			return "Electro";
		case ETower.Ice:
			return "Ice";
		default:
			return "Error: tower type not recognized";
		}
	}
}
