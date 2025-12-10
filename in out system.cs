using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inoutsystem : MonoBehaviour
{
[Header("Player References")]
    [Tooltip("The Player Character GameObject (must be tagged 'Player')")]
    public Transform player;

    [Tooltip("The Player's movement script (e.g., FirstPersonController)")]
    public MonoBehaviour playerMovementScript; 
    
    [Tooltip("Empty Transform defining where the player appears when exiting")]
    public Transform exitPoint;
    
    [Tooltip("The Camera used for player movement")]
    public GameObject playerCamera;
    
    [Tooltip("The Camera used when driving the vehicle")]
    public GameObject vehicleCamera;
    
    [Header("Vehicle Control")]
    [Tooltip("The MAIN vehicle movement/control script (e.g., CarControl.cs). This will be enabled/disabled.")] 
    // We use MonoBehaviour to accept any script type
    public MonoBehaviour vehicleControllerScript; 
    
    private Rigidbody vehicleRb;
    private bool isInside = false;

    [Header("Settings")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    void Start()
    {
        // Get the Rigidbody once on start
        vehicleRb = GetComponent<Rigidbody>();
        if (vehicleRb == null) Debug.LogError("Vehicle requires a Rigidbody component!");
        
        // Initial state: Player outside, vehicle controls disabled
        if (vehicleControllerScript != null) vehicleControllerScript.enabled = false;
        if (vehicleCamera != null) vehicleCamera.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (!isInside)
            {
                // Player is outside, check distance
                if (player != null && Vector3.Distance(player.position, transform.position) < interactDistance)
                {
                    EnterVehicle();
                }
            }
            else
            {
                // Player is inside, exit the car
                ExitVehicle();
            }
        }
    }

    void EnterVehicle()
    {
        if (vehicleControllerScript == null) return;

        isInside = true;

        // 1. Disable Player
        player.gameObject.SetActive(false);
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
        playerCamera.SetActive(false);

        // 2. Enable Vehicle Controls and Camera
        vehicleCamera.SetActive(true);
        vehicleControllerScript.enabled = true;
        
        Debug.Log("Entered the vehicle.");
    }

    void ExitVehicle()
    {

        if (vehicleControllerScript == null) return;
        
        isInside = false;
        
        // 1. CRITICAL: Stop the Vehicle Immediately
        if (vehicleRb != null)
        {
            vehicleRb.velocity = Vector3.zero;
            vehicleRb.angularVelocity = Vector3.zero;
        }

        // 2. Disable Vehicle Controls and Camera
        vehicleControllerScript.enabled = false;
        vehicleCamera.SetActive(false);
        
        // 3. Enable Player and place them at the exit point
        player.position = exitPoint.position;
        player.gameObject.SetActive(true);
        playerCamera.SetActive(true);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        Debug.Log("Exited the vehicle.");
    }
}
