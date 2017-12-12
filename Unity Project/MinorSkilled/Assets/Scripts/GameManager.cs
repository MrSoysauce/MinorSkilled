using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject player;

    protected override void Awake()
    {
        if (player == null) player = GameObject.FindWithTag("Player");
    }

	public void PlayAnimation(Transform trans)
	{
		print ("Play animation");
		Animation anim = trans.GetComponent<Animation> ();
		if (anim)
			anim.Play ();
		else
			Debug.LogError ("U idiot, why u try to play an animation with an object that doesnt even have an animation component. Baka, tehee~", this);
	}

	public void LoadScene(int sceneIndex) {
		SceneManager.LoadScene (sceneIndex);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
