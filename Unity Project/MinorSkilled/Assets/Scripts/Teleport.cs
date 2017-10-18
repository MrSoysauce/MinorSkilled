using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	public GameObject exit;
	public GameObject player;

	void OnCollisionEnter (Collision collision)
	{
		if(collision.gameObject == player)
		{
			player.transform.position = exit.transform.position;
		}
	}
}
