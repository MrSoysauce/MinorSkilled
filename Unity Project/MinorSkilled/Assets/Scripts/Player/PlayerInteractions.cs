using UnityEngine;
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

    [SerializeField] private MeshRenderer[] fadeRenderers;
    [SerializeField] public Transform[] raycastPoints;

    [SerializeField] public Collider col;

    [Header("Audio trigger ranges")]
    [SerializeField] private float audioLandingRange;
    [SerializeField] private bool drawAudioLandingRange;

    [SerializeField] private float audioSprintRange;
    [SerializeField] private bool drawAudioSprintRange;

    [SerializeField] private float audioCrouchRange;
    [SerializeField] private bool drawAudioCrouchRange;

    [SerializeField] private float audioWalkingRange;
    [SerializeField] private bool drawAudioWalkingRange;

    private bool landing = false;

    private RotatePad rotatePad;

    private Vector3 spawnPos;

    private GrabPlayerEnemy attachedEnemy = null;

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
        SetFadeAlpha(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool CanAttachEnemy()
    {
        return attachedEnemy == null;
    }

    public void AttachEnemy(GrabPlayerEnemy enemy)
    {
        attachedEnemy = enemy;
        flyingEnemyCollider.enabled = true;
        player.onlyWalk = true;
        enemy.ApplyEffects(player);
    }

    private void DetachEnemy()
    {
        if (attachedEnemy == null)
            return;

        //Tell enemy that we're done with him
        attachedEnemy.Detach();

        //Undo speed changes
        player.scriptableSpeedModifier = 1;
        player.forceMovement = false;
        player.onlyWalk = false;

        //Undo visual changes
        SetFadeAlpha(1);

        flyingEnemyCollider.enabled = false;
        attachedEnemy = null;
    }

    public void SetFadeAlpha(float a)
    {
        foreach (MeshRenderer m in fadeRenderers)
        {
            if (m != null &&m.material != null)
                m.material.color = new Color(m.material.color.r, m.material.color.g, m.material.color.b, a);
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        //Kill enemy by running into a low ceiling
        foreach (ContactPoint cp in c.contacts)
            if (cp.thisCollider == flyingEnemyCollider)
                DetachEnemy();
    }

    private void OnTriggerEnter(Collider c)
    {
        //Kill enemy with fog
        if (c.CompareTag("EnemyKillingFog"))
            DetachEnemy();
    }

    public float GetSoundRange()
    {
        if (!player.isMoving)
            return 0;

        if (landing)
            return audioLandingRange;
        if (player.sprinting)
            return audioSprintRange;
        if (player.crouching)
            return audioCrouchRange;

        return audioWalkingRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (drawAudioLandingRange)
            Gizmos.DrawWireSphere(transform.position, audioLandingRange);

        if (drawAudioSprintRange)
            Gizmos.DrawWireSphere(transform.position, audioSprintRange);

        if (drawAudioCrouchRange)
            Gizmos.DrawWireSphere(transform.position, audioCrouchRange);

        if (drawAudioWalkingRange)
            Gizmos.DrawWireSphere(transform.position, audioWalkingRange);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            Gizmos.DrawWireSphere(transform.position, GetSoundRange());
#endif
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