using UnityEngine;
using SpriteFactory;

public class EnemyTestCol : MonoBehaviour
{
	SpriteFactory.Sprite dummySpr;
	Rigidbody2D dummyRb;
	Transform playerTrans;
	public float EMaxhealth = 100;
	public float EHealth{ get; private set; }

	// Use this for initialization
	void Start ()
	{
		dummySpr = GetComponent<SpriteFactory.Sprite> ();
		dummyRb = GetComponent<Rigidbody2D> ();
		playerTrans = GameObject.Find ("Player").GetComponent<Transform> ();
		EHealth = EMaxhealth;
	}

	void OnTriggerStaySprite (SpriteFactory.SpriteCollider.CollisionData col)
	{
		if (col.otherColliderGameObject.name == "AttackCollider") {
			if (col.spriteColliderName == "DummyCol") {
				dummyRb.AddForce (new Vector2 (playerTrans.localScale.x * 4, 0), ForceMode2D.Impulse);
			}
		}
	}
	void OnTriggerEnterSprite (SpriteFactory.SpriteCollider.CollisionData col)
	{
		if (col.otherColliderGameObject.name == "AttackCollider") {
			if (col.spriteColliderName == "DummyCol") {
				dummySpr.Play ("Hit");
				TakeDamage (30);
				dummyRb.AddForce (new Vector2 (playerTrans.localScale.x * 4, 0), ForceMode2D.Impulse);
			}
		}
	}
	void Reset ()
	{

	}
	void TakeDamage (int damage)
	{
		EHealth -= damage;
		if (EHealth <= 0) {
			EHealth = 0;
			print (EHealth);
			Destroy (this.gameObject);
		}
	}


}
