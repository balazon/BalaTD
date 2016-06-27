using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum ETeamEnum{ T0, T1, T2, T3, Ne, Un};


//Not used currently - it could be used in a possible upgrade of the game where there are multiple teams
// and this class could provide the data whether or not 2 teams may harm each other
// (or if 2 entities are against each other based on their teams)
public class Enemies : MonoBehaviour {


	public static Enemies Instance{ get; private set;}

	public TriangleLayout canAttack;
	

	void Awake()
	{
		Instance = this;
	}

	void Update()
	{

	}


}
