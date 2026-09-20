using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDodgeSystem : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.5f;
    [SerializeField] private float dodgeCooldown = 0.8f;

    [Header("I-Frame Timing")]
    [SerializeField] private float iFrameStartOffset = 0.05f;
    [SerializeField] private float iFrameDuration = 0.3f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;

    private bool isDodging = false;
    private bool isInvincible = false;
    private float lastDodgeTime = -999f;
    private Vector3 dodgeDirection;

    private static readonly int DodgeTriggerHash = Animator.StringToHash("DodgeRoll");

    public void OnDodge(InputValue value)
    {
        if (value.isPressed && CanDodge())
        {
            StartCoroutine(ExecuteDodgeRoll());
        }
    }

    private bool CanDodge()
    {
        return !isDodging && Time.time >= lastDodgeTime + dodgeCooldown;
    }

    private IEnumerator ExecuteDodgeRoll()
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputDir.sqrMagnitude > 0.1f)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;

            dodgeDirection = (cameraForward.normalized * inputDir.y + cameraRight.normalized * inputDir.x).normalized;
            transform.rotation = Quaternion.LookRotation(dodgeDirection);
        }
        else
        {
            dodgeDirection = transform.forward;
        }

        if (animator != null) animator.SetTrigger(DodgeTriggerHash);

        StartCoroutine(ActivateIFrames());

        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            elapsed += Time.deltaTime;

            if (characterController != null && characterController.enabled)
            {
                characterController.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            }

            yield return null;
        }

        isDodging = false;
    }

    private IEnumerator ActivateIFrames()
    {
        yield return new WaitForSeconds(iFrameStartOffset);
        isInvincible = true;

        yield return new WaitForSeconds(iFrameDuration);
        isInvincible = false;
    }

    public bool IsInvincible() => isInvincible;
    public bool IsDodging() => isDodging;
}
