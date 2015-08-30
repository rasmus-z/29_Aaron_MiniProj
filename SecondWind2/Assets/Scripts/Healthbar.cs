using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour
{
	//Create a script to control all char health

	public PlayerMovements2 player;
	public Transform foregroundSprite;
	public SpriteRenderer foregroundRenderer;
	public Color MaxHealthColor = new Color (255 / 255f, 63 / 255f, 63 / 255f);
	public Color MinHealthColor = new Color (64 / 255f, 127 / 255f, 255 / 255f);
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovements2> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		var healthPercent = player.Health / (float)player.Maxhealth;
		
		foregroundSprite.localScale = new Vector3 (healthPercent, 1, 1);
		foregroundRenderer.color = Color.Lerp (MaxHealthColor, MinHealthColor, healthPercent);
	}
}
