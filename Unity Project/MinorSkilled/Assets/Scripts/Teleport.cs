using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	public float waitTime = 5;
	public GameObject player;
    public GameObject exit;

	Coroutine coroutine = null;

    private void OnTriggerEnter(Collider other)
    {
		if (coroutine == null && other.CompareTag("Player"))
        {
			coroutine = StartCoroutine (TeleportPlayer ());
        }
    }

	IEnumerator TeleportPlayer() {
		yield return new WaitForSeconds(waitTime); 
		player.transform.position = exit.transform.position;

		coroutine = null;
	}
}
