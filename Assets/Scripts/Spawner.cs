using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Wave
{
	public GameObject critterPrefab;
	public int count;
	public float deltaBetweenUnits;
	public int deadCount;
};

public class Spawner : MonoBehaviour {
	public Transform spawningPoint;

	public List<Wave> waves = new List<Wave>();

	public int currentWaveIndex;

	protected Wave currentWave;

	//how many waves are killed entirely
	public int waveDeadCount;

	protected Player player;

	protected float spawningTimer;

	protected int currentCritterIndex;

	protected bool nextWave;

	protected int spawnDislocationIndex;

	protected Vector3 dislocationVector = new Vector3(0, 0, 1.1f);

	protected Vector3 randomDisloc = new Vector3(1.0f, 0, 0.0f);

	protected RaycastHit rayHit;

	public LayerMask layerMask;

	protected bool paused;
	// Use this for initialization
	void Start () {


		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		Reset();
	}

	public void Reset()
	{
		foreach (var c in GameObject.FindGameObjectsWithTag("Critter"))
		{
			Destroy(c);
		}

		spawnDislocationIndex = 0;
		currentWaveIndex = 0;
		waveDeadCount = 0;
		spawningTimer = 1.0f;
		currentCritterIndex = 0;
		nextWave = true;
		paused = true;
	}

	public void StartWaves()
	{
		paused = false;
	}


	
	// Update is called once per frame
	void Update () {
		if(paused)
		{
			return;
		}

		if(nextWave && currentWaveIndex < waves.Count)
		{
			currentWave = waves[currentWaveIndex++];
			currentWave.deadCount = 0;
			currentCritterIndex = 0;
			nextWave = false;
		}

		if(!nextWave)
		{
			spawningTimer -= Time.deltaTime;

			if(spawningTimer <= 0.0f)
			{
				Vector3 spawnPosition = Vector3.zero;
				for(int i = 0; i < 20; i++)
				{
					var randomPoint2D = UnityEngine.Random.insideUnitCircle * 3.0f;
					spawnPosition = spawningPoint.position + new Vector3(randomPoint2D.x, 0.0f, randomPoint2D.y);

					
					//if(!Physics.Raycast(spawningPoint.position + new Vector3(randomPoint2D.x, 4.0f, randomPoint2D.y), Vector3.down, 5.0f, layerMask))
					if(!Physics.CheckSphere(spawnPosition, 0.6f, layerMask))
					{

						break;
					}
				}

				//UnityEngine.Random.insideUnitCircle
				//var spawnPosition = spawningPoint.position + dislocationVector * spawnDislocationIndex + randomDisloc * UnityEngine.Random.Range(0, 5);

				var go = Instantiate(currentWave.critterPrefab, spawnPosition, Quaternion.identity) as GameObject;
				go.transform.parent = GameManager.DynamicObjectContainer.transform;
				Critter critter = go.GetComponent<Critter>();
				critter.CritterWave = currentWave;
				currentCritterIndex++;
				if(currentCritterIndex == currentWave.count)
				{
					nextWave = true;
				}

				
				spawningTimer = currentWave.deltaBetweenUnits;

				spawnDislocationIndex++;
				if(spawnDislocationIndex == 5)
				{
					spawnDislocationIndex = 0;
				}
			}
		}

	}

	public void critterDied(Wave w)
	{
		w.deadCount = w.deadCount + 1;

		if(w.deadCount == w.count)
		{
			waveDeadCount++;
		}
		if(waveDeadCount == waves.Count)
		{
			player.GameFinished();
		}
	}


}
