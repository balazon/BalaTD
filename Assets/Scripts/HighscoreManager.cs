using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




[Serializable]
public struct HighscoreRecord
{
	public string name;
	public int score;
	public HighscoreRecord(string n = "", int sc = 0)
	{
		name = n;
		score = sc;
	}
};



public class HighscoreManager : MonoBehaviour {

	public static HighscoreManager Instance {get; private set;}

	public List<HighscoreRecord> initialHighscores = new List<HighscoreRecord>();


	string highscoreNamePreKey = "HighscoreName_";
	string highscoreCountKey = "HighscoreCount";
	string highscoreScorePreKey = "HighscoreValue_";

	//saving this much record max
	public int topScoresCount = 10;



	public List<HighscoreRecord> AllScores {get; private set;}



	public bool reset = false;

	private static int HighscoreRecordCompare(HighscoreRecord hr1, HighscoreRecord hr2)
	{
		return hr2.score - hr1.score;
	}

	void Awake()
	{
		Instance = this;
	}


	// Use this for initialization
	void Start () {

		if(reset || !PlayerPrefs.HasKey(highscoreCountKey) || PlayerPrefs.GetInt(highscoreCountKey) == 0)
		{

			AllScores = new List<HighscoreRecord>();
			PlayerPrefs.SetInt(highscoreCountKey, 0);

			for(int i = 0; i < Math.Min(initialHighscores.Count, topScoresCount); i++)
			{
				SaveHighscore(initialHighscores[i]);
			}

		}
		else
		{
			int highscoreCount = Math.Min (PlayerPrefs.GetInt(highscoreCountKey), topScoresCount);
			AllScores = new List<HighscoreRecord>(highscoreCount);
			
			for(int i = 0; i < highscoreCount; i++)
			{
				string name = PlayerPrefs.GetString(highscoreNamePreKey + i);
				int score = PlayerPrefs.GetInt(highscoreScorePreKey + i);
				AllScores.Add(new HighscoreRecord(name, score));
			}

		}

		AllScores.Sort(HighscoreRecordCompare);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//saves highscore if name does not already exist
	//return true when successful
	public bool NewHighscore(string name, int score)
	{
		var hr = new HighscoreRecord(name, score);
		return SaveHighscore(hr);
	}

	public bool SaveHighscore(HighscoreRecord hr)
	{
		if(hr.name == "")
		{
			hr.name = "-";
		}

		int pos = 0;
		int min = Math.Min (AllScores.Count, topScoresCount);
		for(pos = 0; pos < min; pos++)
		{
			if(HighscoreRecordCompare(AllScores[pos], hr) > 0)
			{
				break;
			}
		}
		if(pos == topScoresCount)
		{
			return true;
		}
		for(int i = pos; i < min; i++)
		{
			HighscoreRecord t = AllScores[i];
			AllScores[i] = hr;
			hr = t;
			PlayerPrefs.SetString(highscoreNamePreKey + i, AllScores[i].name);
			PlayerPrefs.SetInt(highscoreScorePreKey + i, AllScores[i].score);
		}

		if(AllScores.Count >= topScoresCount)
		{
			PlayerPrefs.Save();
			return true;
		}
		AllScores.Add(hr);
		PlayerPrefs.SetString(highscoreNamePreKey + (AllScores.Count - 1), hr.name);
		PlayerPrefs.SetInt(highscoreScorePreKey + (AllScores.Count - 1), hr.score);

		
		AllScores.Sort(HighscoreRecordCompare);

		PlayerPrefs.SetInt(highscoreCountKey, AllScores.Count);

		PlayerPrefs.Save();

		return true;
	}




}
