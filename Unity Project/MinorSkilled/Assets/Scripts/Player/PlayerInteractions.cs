using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInteractions : MonoBehaviour
{
    private PlayerController player;

    [SerializeField] private Collider flyingEnemyCollider;
    [SerializeField] private GameObject interactMessage;

    [SerializeField] private MeshRenderer[] fadeRenderers;
    [SerializeField] public Transform[] raycastPoints;

    [SerializeField] public Collider col;

    [SerializeField] private float fallDeathTreshold = 2;

    [Header("Audio trigger ranges")]
    [SerializeField] private float audioSprintRange;
    [SerializeField] private bool drawAudioSprintRange;

    [SerializeField] private float audioCrouchRange;
    [SerializeField] private bool drawAudioCrouchRange;

    [SerializeField] private float audioWalkingRange;
    [SerializeField] private bool drawAudioWalkingRange;

    [Header("Bumping")]
    [SerializeField] private float bumpSoundModDecreaseSpeed = 0.1f;
    [SerializeField] private float bumpVelocityModifier = 1;
    private float bumpingSoundMod = 1;
    [HideInInspector] public SoundModifyingMaterial sndmod;

    [Header("Feedback")]
    [SerializeField] private UnityEngine.UI.Image sprintChargeImage;

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

    public void SetRotatePad(RotatePad pad)
    {
        rotatePad = pad;
    }

    private void Update()
    {
        bool rotatePadInput = Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Joystick1Button1);
        if (rotatePadInput && rotatePad)
        {
            rotatePad.Apply();
            rotatePad = null;
        }

        bumpingSoundMod = Mathf.Lerp(bumpingSoundMod, 1, bumpSoundModDecreaseSpeed * Time.deltaTime);
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

        //Falldeathtreshold at <0 means disabled
        if (fallDeathTreshold > 0 && c.relativeVelocity.magnitude > fallDeathTreshold && !c.gameObject.CompareTag("NoFallDamage"))
            RespawnPlayer();

        if (c.gameObject.CompareTag("MovingPlatform"))
            transform.SetParent(c.transform, true);

        if (c.gameObject.CompareTag("Stairs"))
            player.onStairs = true;

        float mod = c.relativeVelocity.magnitude * bumpVelocityModifier;
        if (mod > bumpingSoundMod)
            bumpingSoundMod = mod;
    }

    private void OnCollisionExit(Collision c)
    {
        if (c.gameObject.CompareTag("MovingPlatform"))
            transform.SetParent(null, true);

        if (c.gameObject.CompareTag("Stairs"))
            player.onStairs = false;
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

        float range;

        if (player.sprinting)
            range = audioSprintRange;
        else if (player.crouching)
            range = audioCrouchRange;
        else
            range = audioWalkingRange;

        range *= bumpingSoundMod;

        if (sndmod != null)
            range *= sndmod.Modifier;

        return range;
    }

    private void OnDrawGizmosSelected()
    {
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

    private void LateUpdate()
    {
        if (sprintChargeImage != null)
            sprintChargeImage.fillAmount = player.sprintCharge / 100;
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