using UnityEngine;
using System.Collections;



[System.Serializable]
public class TriangleLayout {

	[System.Serializable]
	public struct triData
	{
		public bool[] e;
		public int n;

		//public string[] labels;

		public triData(int n)
		{
			this.n = n;
			e = new bool[n *  (n + 1) /2];
			//labels = new string[n];
		}



	}

	public triData tri = new triData(System.Enum.GetValues(typeof(ETeamEnum)).Length);

}
