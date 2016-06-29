using UnityEngine;
using System.Collections;

public class Critter : BaseEntity, IMySelectable{

	public Transform goal;


	public float maxHealth = 2.0f;

	public float health = 0.0f;

	public int scoreWorth = 1;

	public int goldWorth = 1;

	public int castleDamage = 1;



	protected float startingSpeed;

	protected float slowedRate = 0.0f;

	public float slowedDurationRemaining { get; private set;}

	protected float dotDps = 0.0f;

	protected float dotDurationRemaining = 0.0f;

	protected NavMeshAgent agent;

	protected float IncomingDamage = 0.0f;

	protected Player player;

	public Wave CritterWave {get; set;}

	protected Spawner spawner;

	public bool alreadyKilled { get; private set;}


	protected MeshRenderer selectedMesh;


	protected override void Awake()
	{
		base.Awake();
		alreadyKilled = false;
		slowedDurationRemaining = 0.0f;

		goal = GameObject.FindGameObjectWithTag("Castle").transform;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		agent = GetComponent<NavMeshAgent>();

		spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();

		selectedMesh = transform.FindChild("CritterCircleSel").GetComponent<MeshRenderer>();

		health = maxHealth;
	}

	// Use this for initialization
	void Start () {
		agent.destination = goal.position;
		startingSpeed = agent.speed;
	}
	
	// Update is called once per frame
	void Update () {
		if(slowedDurationRemaining > 0.0f)
		{
			slowedDurationRemaining -= Time.deltaTime;
		}
		else if(agent)
		{

			agent.speed = startingSpeed;

		}

		if(dotDurationRemaining > 0.0f)
		{
			dotDurationRemaining -= Time.deltaTime;
			TakeDamage(dotDps * Time.deltaTime, true);
		}
		else
		{
			dotDps = 0.0f;
		}


	}

	public void TakeDamage(float dmg, bool isDotDamage = false)
	{
		if(!isDotDamage)
		{
			IncomingDamage -= dmg;
		}
		health -= dmg;
		if(health <= 0.0f)
		{
			health = 0.0f;
			killed();
		}

		observable.notifyObservers();

	}

	//killed is when a tower's bullet kills this critter, not being destroyed bc of entering castle
	// so being killed this way gives the player benefits: score and gold
	void killed()
	{
		if(alreadyKilled || KillPending)
		{
			return;
		}
		alreadyKilled = true;
		agent.Stop();
		agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		Destroy(agent);
		player.IncreaseScore(scoreWorth);
		player.IncreaseMoney(goldWorth);
		spawner.critterDied(CritterWave);
	}

	public void EnteredCastle()
	{
		spawner.critterDied(CritterWave);
		Finished();
		//Destroy(gameObject);
	}

	public void AddDot(float dps, float duration)
	{
		dotDps = dps;
		dotDurationRemaining = duration;
	}

	public void SetSlowed(float slowRate, float slowDuration)
	{
		slowedRate = slowRate;
		agent.speed = startingSpeed  / (1.0f + slowedRate);

		slowedDurationRemaining = slowDuration;
	}


	public void BulletIncoming(Bullet b)
	{
		IncomingDamage += b.baseDamage;
	}
	public bool WillDieSoon()
	{
		return IncomingDamage + dotDps * dotDurationRemaining >= health;
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
