using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class MyRVOManager : MonoBehaviour {

	public float neighborDist = 50.0f;

	public int maxNeighbors = 6;

	public float timeHorizon = 2.0f;

	public float timeHorizonObst = 2.0f;

	public float radius = 0.2f;

	public float maxSpeed = 1.0f;





	protected IList<AIPath> units;

	protected RVO.Simulator sim;

	static MyRVOManager instance;

	void Awake()
	{


	}

	void OnEnable()
	{

	}
	// Use this for initialization
	void Start () {

		units = new List<AIPath>();
		sim = RVO.Simulator.Instance;

		sim.setAgentDefaults(neighborDist, maxNeighbors, timeHorizon, timeHorizonObst, radius, maxSpeed, new RVO.Vector2(0, 0));

		sim.SetNumWorkers(1);
		//TODO add obstacles?
		//process obstacles
		sim.processObstacles();
		
		instance = this;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static MyRVOManager GetInstance()
	{
		return instance;
	}

	public void Register(AIPath unit)
	{
		if(unit == null)
		{
			return;
		}
		units.Add(unit);
		sim.addAgent(new RVO.Vector2(0, 0));
	}

	public void UnRegister(AIPath unit)
	{
		if(unit == null || !units.Contains(unit))
		{
			return;
		}
		units.Remove(unit);
		sim.removeAgent();
	}

	void LateUpdate()
	{

		sim.setTimeStep(Time.deltaTime);

		//desired vels
		for(int i = 0; i < units.Count; i++)
		{
			Vector3 pos = units[i].GetFeetPosition();
			var v = units[i].GetVelocity();
			var vdes = units[i].DesiredVelocity;

			var rvoAgent = sim.agents_[i];
			rvoAgent.position_ = new RVO.Vector2(pos.x, pos.z);
			rvoAgent.velocity_ = new RVO.Vector2(v.x, v.z);
			rvoAgent.prefVelocity_ = new RVO.Vector2(vdes.x, vdes.z);

		}

		//simulate
		sim.doStep();
		//set new vels
		for(int i = 0; i < units.Count; i++)
		{
			var vnew = sim.agents_[i].velocity_;
			units[i].Move(new Vector3(vnew.x_, 0, vnew.y_));
		}
	}
}

