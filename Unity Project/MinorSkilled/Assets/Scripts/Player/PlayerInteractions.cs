using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInteractions : MonoBehaviour
{
    private NPCInteractable npc;
    private PlayerController player;

    [SerializeField] private Collider flyingEnemyCollider;
    [SerializeField] private GameObject interactMessage;

    private RotatePad rotatePad;

    private Vector3 spawnPos;

    private Enemy2 attachedEnemy = null;
    private Transform oldEnemyParent;

    private void Start()
    {
        if (interactMessage != null)
            interactMessage.SetActive(false);

        player = GetComponent<PlayerController>();

        if (!PlayerPrefs.HasKey("Level") || PlayerPrefs.GetString("Level") != SceneManager.GetActiveScene().name)
        {
            PlayerPrefs.SetString("Level", SceneManager.GetActiveScene().name);

            PlayerPrefs.SetFloat("CheckpointPosX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointPosY", transform.position.y);
            PlayerPrefs.SetFloat("CheckpointPosZ", transform.position.z);
            PlayerPrefs.Save();
        }
        else
        {
            spawnPos.x = PlayerPrefs.GetFloat("CheckpointPosX");
            spawnPos.y = PlayerPrefs.GetFloat("CheckpointPosY");
            spawnPos.z = PlayerPrefs.GetFloat("CheckpointPosZ");
            transform.position = spawnPos;
        }

        flyingEnemyCollider.enabled = false;
    }

    /// <summary>
    /// Set to null to disable
    /// </summary>
    /// <param name="interactable"></param>
    public void SetNPCInRange(NPCInteractable interactable)
    {
        this.npc = interactable;

        if (interactMessage != null)
        {
            if (npc == null)
                interactMessage.SetActive(false);
            else if (npc != null)
                interactMessage.SetActive(true);
        }
    }

    public void SetRotatePad(RotatePad pad)
    {
        rotatePad = pad;
    }

    private void Update()
    {
        bool interactInput = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0);
        if (interactInput)
            Interact();

        bool rotatePadInput = Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Joystick1Button1);
        if (rotatePadInput && rotatePad)
        {
            rotatePad.Apply();
            rotatePad = null;
        }
    }

    private void Interact()
    {
        //Yo we have an npc
        if (npc != null)
        {
            //Disable on start
            player.canMove = false;
            player.allowJump = false;

            //Do our cool thing
            //wot

            //enable after
        }
    }

    public void SetCheckpointToCurrentPos()
    {
        PlayerPrefs.SetFloat("CheckpointPosX", transform.position.x);
        PlayerPrefs.SetFloat("CheckpointPosY", transform.position.y);
        PlayerPrefs.SetFloat("CheckpointPosZ", transform.position.z);
        PlayerPrefs.Save();

		spawnPos = transform.position;
    }

    public void RespawnPlayer()
    {
        transform.position = spawnPos;
    }

    public bool AttachEnemy(Enemy2 enemy)
    {
        if (attachedEnemy != null)
            return false;

        oldEnemyParent = enemy.transform.parent;

        enemy.transform.SetParent(transform);
        enemy.transform.localPosition = new Vector3(0, 0, 0);
        enemy.visuals.transform.localPosition = new Vector3(0, 1.5f, 0);
        enemy.GetComponent<NavMeshAgent>().enabled = false;
        
        attachedEnemy = enemy;

        flyingEnemyCollider.enabled = true;

        //Apply speed changes
        float modSpeed = 1;
        if (enemy.invertControls)
            modSpeed *= -1;
        modSpeed *= enemy.slow;
        player.scriptableSpeedModifier = modSpeed;
        player.forceMovement = enemy.forceMovement;
        return true;
    }

    public void DetachEnemy()
    {
        if (attachedEnemy == null)
            return;

        attachedEnemy.attached = false;
        attachedEnemy.transform.SetParent(oldEnemyParent, true);
        oldEnemyParent = null;

        flyingEnemyCollider.enabled = false;

        Destroy(attachedEnemy.gameObject);
        attachedEnemy = null;

        //Undo speed changes
        player.scriptableSpeedModifier = 1;
        player.forceMovement = false;
    }

    private void OnCollisionEnter(Collision c)
    {
        foreach (ContactPoint cp in c.contacts)
        {
            if (cp.thisCollider == flyingEnemyCollider)
            {
                DetachEnemy();
            }
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("EnemyKillingFog"))
        {
            DetachEnemy();
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(PlayerInteractions))]
public class PlayerInteractionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset checkpoint"))
        {
            PlayerInteractions p = target as PlayerInteractions;
            p.SetCheckpointToCurrentPos(); //Practically sets its spawnpoint to its own position
        }
    }
}


#endif