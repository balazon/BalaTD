using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {

	public float baseDamage = 1.0f;

	public float dotDps = 0.0f;

	public float dotDuration = 0.0f;

	public float slowRate = 0.0f;

	public float slowDuration = 0.0f;

	public bool splash = false;

	public float speed = 5.0f;
	
	public float acc = 10.0f;

	public GameObject targetGO;

	protected Critter target;

	protected NavMeshAgent targetAgent;

	protected IList<Critter> crittersClose;

	protected Vector3 vel;



	public Vector3 lastTargetPosition;

	public void init(Critter target, float angle, float magn, BulletAttributes ba)
	{
		this.baseDamage = ba.baseDamage;
		this.dotDps = ba.dotDps;
		this.dotDuration = ba.dotDuration;
		this.slowRate = ba.slowRate;
		this.slowDuration = ba.slowDuration;
		this.splash = ba.splash;

		setTarget(target);
		setInitialVelocity(angle, magn);
	}

	public void setTarget(Critter target)
	{
		this.target = target;
		targetGO = target.gameObject;
		lastTargetPosition = target.transform.position;
		targetAgent = target.GetComponent<NavMeshAgent>();
		target.BulletIncoming(this);
	}

	public void setInitialVelocity(float angle, float magn)
	{
		if(target == null)
		{
			return;
		}
		Vector3 toTarget = (target.transform.position - transform.position);
		toTarget.y = 0;
		toTarget.Normalize();

		Vector3 rotAxis = new Vector3(-toTarget.z, 0, toTarget.x);
		toTarget = Quaternion.AngleAxis(angle, rotAxis) * toTarget;
		vel = toTarget * magn;
	}

	// Use this for initialization
	void Start () {
		crittersClose = new List<Critter>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPos = target ? target.transform.position : lastTargetPosition;
		lastTargetPosition = targetPos;
		Vector3 toTarget = targetPos - transform.position;


		if(toTarget.sqrMagnitude < 1.0f)
		{
			if(splash)
			{
				foreach(Critter c in crittersClose)
				{
					if(!c || c.alreadyKilled)
					{
						continue;
					}
					c.AddDot(dotDps, dotDuration);
					if(slowRate > 0.0f)
					{
						c.SetSlowed(slowRate, slowDuration);
					}

					c.TakeDamage(baseDamage);
				}
			}
			else if(target && !target.alreadyKilled)
			{
				target.AddDot(dotDps, dotDuration);
				if(slowRate > 0.0f)
				{
					target.SetSlowed(slowRate, slowDuration);
				}
				target.TakeDamage(baseDamage);
			}

			Destroy(gameObject);
		}
		else
		{
			if(target && targetAgent)
			{
				toTarget += targetAgent.desiredVelocity * 0.5f;
			}

			Vector3 dir = toTarget.normalized;
			Vector3 force = dir * speed - vel;
			if(force.sqrMagnitude > acc * acc)
			{
				force = force.normalized * acc;
			}
			vel += force * Time.deltaTime;
			transform.position += vel * Time.deltaTime;
		}

	}

	void OnTriggerEnter(Collider other)
	{

		if(splash && other.gameObject.tag == "Critter")
		{
			crittersClose.Add(other.gameObject.GetComponent<Critter>());
		}
		
	}
	
	void OnTriggerExit(Collider other)
	{
		if(splash && other.gameObject.tag == "Critter")
		{
			crittersClose.Remove(other.gameObject.GetComponent<Critter>());
		}
	}
}
