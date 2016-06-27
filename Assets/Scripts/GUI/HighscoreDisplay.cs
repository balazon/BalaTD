using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class HighscoreDisplay : MonoBehaviour {


	public GameObject highscoreItemPrefab;




	Transform itemContainer;




	void Awake()
	{
		itemContainer = GameObject.Find("GUI").transform.Find("Highscores").Find("Scroll View").Find("Viewport").Find("Content");

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void FillContent()
	{
		var AllScores = HighscoreManager.Instance.AllScores;
		var topScoresCount = HighscoreManager.Instance.topScoresCount;



		var children = new List<GameObject>();
		foreach (Transform child in itemContainer.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));


		int min = Math.Min (AllScores.Count, topScoresCount);

		for(int i = 0; i < min; i++)
		{
			var go = Instantiate(highscoreItemPrefab) as GameObject;

			go.transform.SetParent(itemContainer, false);


			var name = go.transform.FindChild("Name").GetComponent<Text>();
			var score = go.transform.FindChild("Score").GetComponent<Text>();

			name.text = (i + 1) + ". " + AllScores[i].name;
			score.text = "" + AllScores[i].score;

			go.SetActive(true);
		}


	}
}
