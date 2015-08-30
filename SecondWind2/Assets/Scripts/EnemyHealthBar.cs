using UnityEngine;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{

	public EnemyTestCol Enemy;
	public Transform foregroundSprite;
	public SpriteRenderer foregroundRenderer;
	public Color MaxHealthColor = new Color (255 / 255f, 63 / 255f, 63 / 255f);
	public Color MinHealthColor = new Color (64 / 255f, 127 / 255f, 255 / 255f);
	// Use this for initialization
	void Start ()
	{
		Enemy = GameObject.Find ("TrainingDummy").GetComponent<EnemyTestCol> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		var healthPercent = Enemy.EHealth / (float)Enemy.EMaxhealth;
		
		foregroundSprite.localScale = new Vector3 (healthPercent, 1, 1);
		foregroundRenderer.color = Color.Lerp (MaxHealthColor, MinHealthColor, healthPercent);
	}
}
