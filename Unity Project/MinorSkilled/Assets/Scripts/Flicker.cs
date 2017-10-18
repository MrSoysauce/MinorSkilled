using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour {

	private Light flickering;
	public bool active;
	public float interval;
	public Vector2 randomInterval;

	// Use this for initialization
	void Start () {
		flickering = this.GetComponent<Light> (); 
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (active) 
		{
			// Goes back and forth between the two forever
			if (flickering.enabled == true) 
			{
				flickering.enabled = false;
				StartCoroutine ("FlickerIntervalRandom");
			}
		} 
		else 
		{
			if (flickering.enabled == true)
			{
				flickering.enabled = false;
				StartCoroutine ("FlickerInterval", interval);
			}
		}
	}

	IEnumerator FlickerInterval()
	{
		yield return new WaitForSeconds(interval * Time.deltaTime * 10f);
		flickering.enabled = true;
	}

	IEnumerator FlickerIntervalRandom()
	{
		yield return new WaitForSeconds(Random.Range(randomInterval.x, randomInterval.y) * Time.deltaTime * 10f);
		flickering.enabled = true;
	}

}
