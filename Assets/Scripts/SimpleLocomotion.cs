using UnityEngine;
using System.Collections;

public class SimpleLocomotion : MonoBehaviour {

	Animator anim;
	NavMeshAgent agent;



	Critter c;



	void Start ()
	{
		anim = GetComponentInParent<Animator> ();
		agent = transform.GetComponentInParent<NavMeshAgent> ();
		c = GetComponentInParent<Critter>();
	}
	
	void Update ()
	{

		if(!anim.GetBool("Dead") && (c == null || c.alreadyKilled))
		{
			anim.SetBool("Dead", true);
			//anim.SetTrigger(
		}

		if(c != null && !c.alreadyKilled)
		{
			anim.SetFloat("Speed", agent.velocity.magnitude);
		}
	}

	//Important - To destroy a unit:
	//At animcontroller's dead state, select the death animation, and at the events, set this function to be called when you wish to destroy the unit
	void DeathAnimationFinished()
	{
		//Destroy(transform.parent.gameObject);
		c.Finished();

	}
}
