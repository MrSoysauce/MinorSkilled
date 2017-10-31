using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaver : MonoBehaviour {

	public Animator lever;
	public Animator door;

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Joystick1Button1)) 
		{
			print ("dslfnlvsdvmnd");
			lever.SetTrigger ("leverani");
			door.SetTrigger ("DoorOp");
		}
	}
}
