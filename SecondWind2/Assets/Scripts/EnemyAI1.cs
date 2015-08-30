using UnityEngine;
using System.Collections;
using SpriteFactory;

/*Imrovements List

	collisions : When collision detected, change state
	when no collision, chase for a period of time, then retreat back to idle


 * Find a way to find waypoints transforms based on the enemy's child


*/
public class EnemyAI1 : MonoBehaviour
{
	
	#region States
	private enum State
	{
		Idle,
		Init,
		Setup,
		Actions
	}
	private enum Actions
	{
		NonCombat,
		InCombat
	}
	private enum NonCombat
	{
		Stay,
		Patrol,
	}
	private enum InCombat
	{
		MeleeAtt,
		RangeAtt,
		Chase,
		Retreat,
	}
	
	private State _state;
	private Actions _actions;
	private NonCombat _nonCombat;
	private InCombat _inCombat;
	#endregion
	#region Variables
	SpriteFactory.Sprite dragonSpr;
	SpriteFactory.SpriteCollider dragonCol;
	
	//Waypoints-----------------------
	
	public Transform[] Waypoints;
	int curWaypoint;
	Vector2 Target;
	Vector2 Direction;
	Vector2 Velocity;
	Transform playerPos;
	Rigidbody2D dragonRb;
	float speed;
	// Timers------------------------
	float idleTimer = 0f;
	float idleMaxTimer;
	float patrolTimer = 0f;
	float patrolMaxTimer;
	float meleeAttTimer = 0f;
	float meleeAttMaxTimer;
	
	//Bools-------------------------
	bool _isAlive = true;
	bool _meleeAttCd = false;
	#endregion
	
	
	void Start ()
	{
		_state = State.Init;
		StartCoroutine ("FSM");
		
	}
	private IEnumerator FSM ()
	{
		while (_isAlive) {
			switch (_state) {
			case State.Init:
				Init ();
				break;
			case State.Setup:
				Setup ();
				break;
			case State.Actions:
				ActionsMethod ();
				break;
			}
			yield return null;
		}
	}
	
	private void Init ()
	{
		Debug.Log ("----Init----");
		dragonSpr = GetComponent<SpriteFactory.Sprite> ();
		dragonRb = GetComponent<Rigidbody2D> ();
		dragonCol = GetComponent<SpriteCollider> ();
		//Timer--------------------------------------
		idleMaxTimer = Random.Range (1.0f, 5.0f);
		patrolMaxTimer = Random.Range (1.0f, 5.0f);
		meleeAttMaxTimer = 5f;
		
		//Waypoints Init----
		playerPos = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		speed = 1f;	
		//ChangeState------------------------------------
		_state = State.Setup;
		
		
	}
	private void Setup ()
	{
		Debug.Log ("---Setup---");
		_nonCombat = NonCombat.Stay;
		_state = State.Actions;
	}
	private void ActionsMethod ()
	{
		Debug.Log ("---Actions---");
		switch (_actions) {
		case Actions.NonCombat:
			//Non-Combat Actions------------------
			switch (_nonCombat) {
			case NonCombat.Stay:
				Stay ();
				break;
			case NonCombat.Patrol:
				Patrol ();
				break;
			}
			//---------------------
			
			break;
		case Actions.InCombat:
			//Combat Actions-----------------
			switch (_inCombat) {
			case InCombat.Chase:
				Chase ();
				break;
			case InCombat.MeleeAtt:
				MeleeAtt ();
				break;
			case InCombat.RangeAtt:
				RangeAtt ();
				break;
			case InCombat.Retreat:
				Retreat ();
				break;
				
			}
			//-----------------
			break;
		}
		
	}
	
	private void Stay ()
	{
		Debug.Log ("---Stay---");
		idleTimer += Time.deltaTime;
		dragonSpr.Play ("Idle");
		dragonRb.velocity = Vector2.zero;
		if (idleTimer >= idleMaxTimer) {
			idleTimer = 0;
			idleMaxTimer = Random.Range (1.0f, 4.0f);
			_nonCombat = NonCombat.Patrol;
		}
	}
	
	private void Patrol ()
	{
		Debug.Log ("---Patrol---");
		patrolTimer += Time.deltaTime;
		
		//patrolling---------------------------
		if (curWaypoint < Waypoints.Length) {
			Target = Waypoints [curWaypoint].transform.position;
			Direction = Target - new Vector2 (transform.position.x, transform.position.y);
			Velocity = dragonRb.velocity;
			
			if (Direction.magnitude < 1) {
				curWaypoint ++;
				dragonSpr.FlipX ();
				//Add Flipping if neccessary
			} else {
				Velocity = Direction.normalized * speed;
			}
		} else {
			curWaypoint = 0;
		}
		
		dragonRb.velocity = Velocity;
		dragonSpr.Play ("Walk");
		if (patrolTimer >= patrolMaxTimer) {
			patrolTimer = 0f;
			patrolMaxTimer = Random.Range (1.0f, 4.0f);
			_nonCombat = NonCombat.Stay;
		}
		
	}
	
	private void MeleeAtt ()
	{
		Debug.Log ("---MeleeAtt---");
		
		dragonSpr.Play ("Attack_1");
		dragonSpr.SetAnimationFinishedCallback ("Attack_1", startCouroutineCd);
		
	}
	void startCouroutineCd ()
	{
		StartCoroutine ("CoolDown");
	}
	IEnumerator CoolDown ()
	{
		_meleeAttCd = true;
		yield return new WaitForSeconds (3f);
		_meleeAttCd = false;
	}
	private void RangeAtt ()
	{
		
	}
	private void Chase ()
	{
		Debug.Log ("---Chase---");
		
		if (Target.x < transform.position.x) {
			transform.localScale = new Vector2 (1, 1);
		} else if (Target.x > transform.position.x) {
			transform.localScale = new Vector2 (-1, 1);
		}//Enemy flip the wrong way when intial chase start backwards. Use raycasting, or try putting flip somewhere else
		
		Target = playerPos.position;
		Direction = Target - new Vector2 (transform.position.x, 0f);
		Velocity = dragonRb.velocity;
		
		
		if (Target.y > transform.position.y) {
			Velocity = Vector2.zero;
		} else {
			Velocity = Direction.normalized * speed;
		}
		
		dragonRb.velocity = Velocity;
		dragonSpr.Play ("Walk");
	}
	private void Retreat ()
	{
		
	}
	
	
	void OnTriggerStaySprite (SpriteFactory.SpriteCollider.CollisionData col)
	{
		if (col.otherColliderGameObject.name == "PlayerCol") {
			_actions = Actions.InCombat;
			
			if (col.spriteColliderName == "MeleeAttCol") {
				if (!_meleeAttCd) {
					_inCombat = InCombat.MeleeAtt;
				} else {
					_inCombat = InCombat.Chase;
				}
			} 
		}
	}
}








