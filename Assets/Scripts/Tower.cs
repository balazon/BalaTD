using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Tower : BaseEntity, IMySelectable{

	public ETower type;

	public TowerLevelAttributes[] levels;

	//reciproc of cooldown time for attacking again
	public float attackRate = 1.0f;

	public GameObject bulletPrefab;

	public Vector3 bulletSpawnPoint = new Vector3(0, 1.5f, 0);

	//for finding closest target
	public int sampleCount = 10;

	protected float cooldownRemaining = -1.0f;

	protected IList<Critter> crittersClose;

	protected Critter target;

	public int level;

	protected BulletAttributes bulletAttributes;

	protected IList<Critter> removables;

	protected MeshRenderer selectedMesh;

	protected override void Awake()
	{
		base.Awake();
		if(crittersClose == null)
		{
			crittersClose = new List<Critter>(100);
		}
		level = 1;

		removables = new List<Critter>();

		selectedMesh = transform.FindChild("TowerCircleSel").GetComponent<MeshRenderer>();
	}

	public void init(TowerLevelAttributes[] levs)
	{
		levels = levs;
		UpdateAttributes();
	}

	// Use this for initialization
	void Start () {
		target = null;
	}
	
	// Update is called once per frame
	void Update () {
		if(cooldownRemaining > 0.0f)
		{
			cooldownRemaining -= Time.deltaTime;
		}
		else
		{

			removables.Clear();
			for(int i = 0; i < crittersClose.Count; i++)
			{
				if(crittersClose[i] == null || crittersClose[i].alreadyKilled)
				{
					removables.Add(crittersClose[i]);
				}
			}
			for(int i = 0; i < removables.Count; i++)
			{
				crittersClose.Remove(removables[i]);
			}


			if(crittersClose.Count > 0)
			{
				fire ();
				cooldownRemaining = 1.0f / attackRate;
			}
		}


	}

	void UpdateAttributes()
	{
		var lev = levels[level-1];
		attackRate = lev.attackRate;
		bulletAttributes = lev.bulletAttributes;
	}

	public void Upgrade()
	{
		if(levels.Length > level)
		{
			level++;
			UpdateAttributes();
			observable.notifyObservers();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Critter")
		{
			if(crittersClose == null)
			{
				Debug.Log("null");
				return;
			}
			crittersClose.Add(other.gameObject.GetComponent<Critter>());
		}

	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Critter")
		{
			crittersClose.Remove(other.gameObject.GetComponent<Critter>());
		}
	}

	protected Critter FindTarget()
	{
		if(bulletAttributes.slowRate > 0.0f)
		{
			return FindPreferablyNotSlowedTarget();
		}

		int count = crittersClose.Count;
		float minDistSq = -1.0f;
		Critter minCritter = null;

		int loopcount = count < sampleCount ? count : sampleCount;
		for(int i = 0; i < loopcount; i++)
		{
			var index = (count < sampleCount) ? i : Random.Range(0, count);
			var critter = crittersClose[index];

			float distSq = (critter.transform.position - transform.position).sqrMagnitude;
			if(minCritter == null || distSq < minDistSq)
			{
				minCritter = critter;
				minDistSq = distSq;
			}

		}
		return minCritter;
	}


	protected Critter FindPreferablyNotSlowedTarget()
	{
		int count = crittersClose.Count;
		float minDistSq = -1.0f;
		Critter minCritter = null;
		float minDistSqNotSlowed = -1.0f;
		Critter minNotSlowedCritter = null;
		
		int loopcount = count < sampleCount ? count : sampleCount;
		for(int i = 0; i < loopcount; i++)
		{
			var index = (count < sampleCount) ? i : Random.Range(0, count);
			var critter = crittersClose[index];
			
			float distSq = (critter.transform.position - transform.position).sqrMagnitude;
			if(minCritter == null || distSq < minDistSq)
			{
				minCritter = critter;
				minDistSq = distSq;
			}
			if(critter.slowedDurationRemaining <= 0.0f && (minNotSlowedCritter == null || distSq < minDistSqNotSlowed))
			{
				minNotSlowedCritter = critter;
				minDistSqNotSlowed = minDistSq;
			}
			
		}

		return minNotSlowedCritter ?? minCritter;
	}

	public virtual void fire()
	{
		if(!target || !crittersClose.Contains(target) || target.WillDieSoon() || bulletAttributes.slowRate > 0.0f)
		{
			target = FindTarget();
		}
		for(int i = 0; i < 5; i++)
		{
			if(!target.WillDieSoon())
			{
				break;
			}
			target = FindTarget();
		}

		var go = Instantiate(bulletPrefab, transform.position + bulletSpawnPoint, Quaternion.identity) as GameObject;
		go.transform.parent = GameManager.DynamicObjectContainer.transform;
		Bullet b = go.GetComponent<Bullet>();
		b.init(target, 20.0f, 2.0f, bulletAttributes);

	}

	public void SetSelected(bool val)
	{
		selectedMesh.enabled = val;
	}

	public bool SameAs(IMySelectable other)
	{
		return this == other;
	}

}
