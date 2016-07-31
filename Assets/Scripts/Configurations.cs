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
