using UnityEngine;
using System.Collections;
using SpriteFactory;

public class PlayerMovements2 : MonoBehaviour
{
	//TODO LIST: Helthbar


	#region Variables
	//Bools----------------------------
	bool _isAlive;
	bool _isAttacking;
	bool onGround;//TBC
	bool _gotHit;
	bool _invul;

	//Timers---------------------
	float impactTimer;
	float impactTimerMax;

	float hitTimer;
	float hitTimerMax;

	float invulTimer;
	float invulTimerMax;

	//Int/Floats------------------------------
	int attackCounter;
	int comboCount;
	public float Maxhealth = 100;
	public float Health{ get; private set; }
	//Others---------------------------
	SpriteFactory.Sprite playerSpr;
	Rigidbody2D playerRb;
	Transform groundPos;
	#endregion
	
	
	#region States
	private enum State
	{
		Init,
		Movements,
		Attack,
		Hit
	}
	private State _state;
	#endregion
	
	
	// Use this for initialization
	
	void Awake ()
	{
		_isAlive = true;
	}
	
	void Start ()
	{
		_state = State.Init;
		StartCoroutine ("FSM");
	}
	#region FSM
	private IEnumerator FSM ()
	{
		while (_isAlive) {
			print ("---FSM is running---");
			switch (_state) {
			case State.Init:
				Init ();
				break;
			case State.Movements:
				Movements ();
				break;
			case State.Attack:
				AttackMode ();
				break;
			case State.Hit:
				//Hit ();
				break;
			}
			yield return null;
		}
	}
	#endregion
	#region Initialize
	void Init ()
	{
		Health = Maxhealth;

		print ("---Initializing---");
		playerSpr = GetComponent<SpriteFactory.Sprite> ();
		playerRb = GetComponent<Rigidbody2D> ();
		_state = State.Movements;
		groundPos = transform.FindChild ("groundPos").GetComponent<Transform> ();
		
		_gotHit = false;
		
		impactTimer = 0f;
		impactTimerMax = .1f;
		invulTimer = 0f;
		invulTimerMax = 1.5f;
		hitTimer = 0f;
		hitTimerMax = .8f;
	}
	#endregion
	#region Movements
	void Movements ()
	{
		print ("---Movements Running---");
		if (!Input.anyKey && onGround) {
			playerSpr.Play ("Idle");
		} else if (Input.GetAxisRaw ("Horizontal") != 0 && onGround) {
			playerSpr.Play ("Run");
		} else if (Input.GetKeyDown (KeyCode.Space)) {
			playerSpr.Play ("Jump");
		}
	}
	#endregion

	#region AttackSystem
	void AttackMode ()
	{
		print ("---Attack Running---");
		if (!_isAttacking)
			Attack (attackCounter);
		
	}
	

	void Attack (int AttCount)
	{
		_isAttacking = true;
		
		playerRb.velocity = new Vector2 (0.0f, playerRb.velocity.y);//Reset Vel
		//Inching
		if (transform.localScale.x > 0) {
			playerRb.AddForce ((Vector2.right * .7f), ForceMode2D.Impulse);
		} else if (transform.localScale.x < 0) {
			playerRb.AddForce ((Vector2.left * .7f), ForceMode2D.Impulse);
		}
		
		playerSpr.Play ("Attack1");
		
		playerSpr.SetAnimationFinishedCallback ("Attack1", CheckAttCount);
		
	}
	
	void Attack2 ()
	{
		if (_state == State.Attack) {
			_isAttacking = true;
			playerRb.velocity = new Vector2 (0.0f, playerRb.velocity.y);//Stops Initial Movements
			
			//Inching
			if (transform.localScale.x > 0) {
				playerRb.AddForce ((Vector2.right * .7f), ForceMode2D.Impulse);
			} else if (transform.localScale.x < 0) {
				playerRb.AddForce ((Vector2.left * .7f), ForceMode2D.Impulse);
			}
			
			playerSpr.Play ("Attack2");
			playerSpr.SetAnimationFinishedCallback ("Attack2", CheckAttCount);
		} else {
			endAttack ();
		}
	}
	void Attack3 ()
	{
		if (_state == State.Attack) {
			_isAttacking = true;
			
			playerRb.velocity = new Vector2 (0.0f, playerRb.velocity.y);
			
			//Inching
			if (transform.localScale.x > 0) {
				playerRb.AddForce ((Vector2.right * .7f), ForceMode2D.Impulse);
			} else if (transform.localScale.x < 0) {
				playerRb.AddForce ((Vector2.left * .7f), ForceMode2D.Impulse);
			}
			
			playerSpr.Play ("Attack3");
			playerSpr.SetAnimationFinishedCallback ("Attack3", endAttack);
		} else {
			endAttack ();
		}
	}
	
	void CheckAttCount ()
	{
		print ("CheckingAttCount");
		if (attackCounter > 1) {
			print ("Attack Counter" + attackCounter);
			comboCount++;
			attackCounter = 0;
			if (comboCount == 1) {
				Attack2 ();
			} else if (comboCount == 2) {
				Attack3 ();
				comboCount = 0;
			}
		} else {
			endAttack ();
		}
	}
	void endAttack ()
	{
		_isAttacking = false;
		attackCounter = 0;
		comboCount = 0;
		_state = State.Movements;
		
	}
	#endregion

	#region CheckCol
	void OnTriggerStaySprite (SpriteFactory.SpriteCollider.CollisionData col)
	{
		if (col.otherColliderGameObject.tag == "Enemy") {
			if (col.spriteColliderName == "PlayerCol") {
				if (!_invul)
					StartCoroutine (Knockback (0.02f));
			}
		}
	}
	#endregion
	
	#region KnockBack
	public IEnumerator Knockback (float knockDur)
	{
		//Knocback
		TakeDamage (30);
		print (Health);
		print ("Colided");
		_invul = true;
		float timer = 0;
		
		while (knockDur > timer) {
			
			timer += Time.deltaTime;
			playerRb.velocity = Vector2.zero;
			playerRb.AddForce (new Vector2 (this.transform.localScale.x * -5, -playerRb.velocity.y + 2.0f), ForceMode2D.Impulse);
		}
		yield return 0;
		
	}
	#endregion 


	public void TakeDamage (int damage)
	{
		Health -= damage;
		if (Health <= 0) {
			Health = 0;
			print ("Lost");
			Destroy (this.gameObject);
		}
	}
		
		
		
	// Update is called once per frame
	void Update ()//One-Shot force, and inputs
	{
		#region Invul Timer
		if (_invul)
			invulTimer += Time.deltaTime;
		if (invulTimer > invulTimerMax) {
			_invul = false;
			invulTimer = 0;
		}
		#endregion

		#region Flipping & PlayerInput
		if (!_gotHit) {

			if (!_isAttacking) {
				if (Input.GetAxisRaw ("Horizontal") > 0) {
					transform.localScale = new Vector2 (1, 1);
				} else if (Input.GetAxisRaw ("Horizontal") < 0) {
					transform.localScale = new Vector2 (-1, 1);
				}
				if (Input.GetKeyDown (KeyCode.Space) && onGround) {
					Jump ();
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				attackCounter ++;
				if (!_isAttacking) {
					_state = State.Attack;
				}
			}
		}
		#endregion

	}
	#region Jump
	void Jump ()
	{
		float jumpForce = -playerRb.velocity.y + 4.0f;
		
		playerRb.AddForce ((new Vector2 (0.0f, jumpForce)), ForceMode2D.Impulse);
	}
	#endregion
	void FixedUpdate ()//Constant Rigidbody Forces/Movements
	{
		onGround = Physics2D.Linecast (this.transform.position, groundPos.position, 1 << LayerMask.NameToLayer ("Ground"));
		

		if (_state != State.Attack) {
			if (Input.GetAxisRaw ("Horizontal") > 0) {
				playerRb.AddForce ((Vector2.right), ForceMode2D.Impulse);
			} else if (Input.GetAxisRaw ("Horizontal") < 0) {
				playerRb.AddForce ((Vector2.left), ForceMode2D.Impulse);
			} else if (Input.GetAxisRaw ("Horizontal") == 0) {
				playerRb.velocity = new Vector2 (0.0f, playerRb.velocity.y);
			}
		}
			
			
		playerRb.velocity = new Vector2 (Mathf.Clamp (playerRb.velocity.x, -3.0f, 3.0f), Mathf.Clamp (playerRb.velocity.y, -4.0f, 4.0f));

	}
}
