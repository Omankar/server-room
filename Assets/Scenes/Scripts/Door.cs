using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Door Reference")]
    public Transform doorHinge;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float doorSpeed = 2f;

    [Header("AC Audio Sources")]
    public AudioSource[] acAudios;
    public AudioLowPassFilter[] acLowPasses;

    [Header("Audio Values")]
    public float openVolume = 0.5f;
    public float closedVolume = 0.15f;

    public float openCutoff = 22000f;
    public float closedCutoff = 800f;

    public float audioLerpSpeed = 4f;

    private bool isOpen = false;
    private bool playerInsideRoom = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorHinge == null)
        {
            Debug.LogError("DoorHinge is NOT assigned.");
            return;
        }

        closedRotation = doorHinge.rotation;
        openRotation = Quaternion.Euler(
            doorHinge.eulerAngles + Vector3.up * openAngle
        );

        // Initialize audio as unoccluded
        for (int i = 0; i < acAudios.Length; i++)
        {
            if (acAudios[i] != null)
                acAudios[i].volume = openVolume;

            if (i < acLowPasses.Length && acLowPasses[i] != null)
                acLowPasses[i].cutoffFrequency = openCutoff;
        }
    }

    void Update()
    {
        // INPUT (New Input System)
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleDoor();
        }

        // DOOR ROTATION
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        doorHinge.rotation = Quaternion.Slerp(
            doorHinge.rotation,
            targetRotation,
            Time.deltaTime * doorSpeed
        );

        // AUDIO LOGIC
        // Muffle ONLY if door is closed AND player is outside
        bool shouldMuffle = !isOpen && !playerInsideRoom;

        float targetVolume = shouldMuffle ? closedVolume : openVolume;
        float targetCutoff = shouldMuffle ? closedCutoff : openCutoff;

        for (int i = 0; i < acAudios.Length; i++)
        {
            if (acAudios[i] != null)
            {
                acAudios[i].volume = Mathf.Lerp(
                    acAudios[i].volume,
                    targetVolume,
                    Time.deltaTime * audioLerpSpeed
                );
            }

            if (i < acLowPasses.Length && acLowPasses[i] != null)
            {
                acLowPasses[i].cutoffFrequency = Mathf.Lerp(
                    acLowPasses[i].cutoffFrequency,
                    targetCutoff,
                    Time.deltaTime * audioLerpSpeed
                );
            }
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    // PLAYER INSIDE / OUTSIDE DETECTION
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideRoom = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideRoom = false;
        }
    }
}
