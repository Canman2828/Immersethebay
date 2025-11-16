using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles card throwing in XR with various input methods.
/// Provides multiple approaches for robust card throwing detection.
/// </summary>
public class CardThrowHandler : MonoBehaviour
{
    [Header("Throw Detection Settings")]
    [SerializeField] private float velocityThreshold = 2f; // Minimum velocity to trigger throw
    [SerializeField] private float maxThrowDistance = 50f; // Maximum distance card can be thrown
    [SerializeField] private float throwCooldown = 0.2f; // Cooldown between throws to prevent double-throws

    [Header("Input Method Selection")]
    [SerializeField] private ThrowInputMethod inputMethod = ThrowInputMethod.VelocityBased;

    [Header("Pinch Detection (for pinch-release throwing)")]
    [SerializeField] private float pinchThreshold = 0.1f; // How close fingers must be to register pinch
    [SerializeField] private float pinchSensitivity = 1f; // How sensitive pinch detection is

    [Header("References")]
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Camera xrCamera;
    [SerializeField] private LineRenderer throwArcPreview; // Optional trajectory preview

    private CardManager cardManager;
    private GameManager gameManager;
    private bool isEnabled = true;
    private float lastThrowTime = 0f;

    private Dictionary<Transform, CardThrowState> handStates = new Dictionary<Transform, CardThrowState>();
    private List<Card> currentHand = new List<Card>();

    public enum ThrowInputMethod
    {
        VelocityBased,    // Current method: hand velocity triggers throw
        PinchRelease,     // Pinch to grab, release to throw
        GestureSwipe,     // Swipe gesture to throw
        GestureThrow,     // Throw-shaped hand gesture
        RaycastPoint      // Point at location and confirm with gesture
    }

    private class CardThrowState
    {
        public Vector3 lastPosition;
        public Vector3 velocity;
        public bool isMoving;
        public float stationaryTime;
        public bool isPinching;
        public Card heldCard;
    }

    private void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        gameManager = FindObjectOfType<GameManager>();
        xrCamera = Camera.main;

        // Initialize hand states
        if (rightHandTransform != null)
        {
            handStates[rightHandTransform] = new CardThrowState();
        }
        if (leftHandTransform != null)
        {
            handStates[leftHandTransform] = new CardThrowState();
        }

        // Initialize hand with random cards
        InitializeHand();
    }

    private void Update()
    {
        if (!isEnabled || !gameManager.IsGameActive()) return;

        switch (inputMethod)
        {
            case ThrowInputMethod.VelocityBased:
                HandleVelocityBasedThrow();
                break;
            case ThrowInputMethod.PinchRelease:
                HandlePinchReleaseThrow();
                break;
            case ThrowInputMethod.GestureSwipe:
                HandleSwipeThrow();
                break;
            case ThrowInputMethod.GestureThrow:
                HandleThrowGestureThrow();
                break;
            case ThrowInputMethod.RaycastPoint:
                HandleRaycastPointThrow();
                break;
        }
    }

    // ========== THROW INPUT METHODS ==========

    /// <summary>
    /// CURRENT METHOD: Velocity-based throwing
    /// Hand velocity is tracked and when it exceeds threshold, card is thrown
    /// PROS: Intuitive, natural feeling
    /// CONS: Can trigger accidental throws, hard to be precise
    /// </summary>
    private void HandleVelocityBasedThrow()
    {
        foreach (var handEntry in handStates)
        {
            Transform handTransform = handEntry.Key;
            CardThrowState state = handEntry.Value;

            // Calculate hand velocity
            Vector3 currentPos = handTransform.position;
            state.velocity = (currentPos - state.lastPosition) / Time.deltaTime;
            state.lastPosition = currentPos;

            float velocity = state.velocity.magnitude;

            // Check if velocity exceeds threshold
            if (velocity > velocityThreshold && Time.time - lastThrowTime > throwCooldown)
            {
                // Throw card in direction of hand movement
                ThrowCard(handTransform.position, state.velocity.normalized);
                lastThrowTime = Time.time;
            }
        }
    }

    /// <summary>
    /// IMPROVED METHOD 1: Pinch-to-grab, release-to-throw
    /// Player pinches fingers to grab a card, then releases pinch to throw
    /// PROS: Very deliberate, prevents accidental throws, clear feedback
    /// CONS: Requires more complex hand tracking
    /// </summary>
    private void HandlePinchReleaseThrow()
    {
        foreach (var handEntry in handStates)
        {
            Transform handTransform = handEntry.Key;
            CardThrowState state = handEntry.Value;

            // Check pinch state (requires hand tracking with finger positions)
            bool currentlyPinching = DetectPinch(handTransform);

            if (currentlyPinching && !state.isPinching)
            {
                // Start pinching - grab a card
                state.heldCard = currentHand[Random.Range(0, currentHand.Count)];
                Debug.Log($"Grabbed card: {state.heldCard.name}");
            }
            else if (!currentlyPinching && state.isPinching && state.heldCard != null)
            {
                // Release pinch - throw the card
                Vector3 throwDirection = handTransform.forward;
                ThrowCard(handTransform.position, throwDirection);
                state.heldCard = null;
                lastThrowTime = Time.time;
            }

            state.isPinching = currentlyPinching;
        }
    }

    /// <summary>
    /// IMPROVED METHOD 2: Swipe gesture
    /// Player swipes their hand to throw a card in that direction
    /// PROS: Clear input pattern, less accidental triggering
    /// CONS: Requires gesture recognition
    /// </summary>
    private void HandleSwipeThrow()
    {
        foreach (var handEntry in handStates)
        {
            Transform handTransform = handEntry.Key;
            CardThrowState state = handEntry.Value;

            Vector3 currentPos = handTransform.position;
            state.velocity = (currentPos - state.lastPosition) / Time.deltaTime;
            state.lastPosition = currentPos;

            // Detect swipe: fast movement in relatively straight line
            float speed = state.velocity.magnitude;

            if (speed > velocityThreshold && Time.time - lastThrowTime > throwCooldown)
            {
                // Check if movement is mostly horizontal (swipe pattern)
                if (Mathf.Abs(state.velocity.y) < Mathf.Abs(state.velocity.x) * 0.5f)
                {
                    ThrowCard(handTransform.position, state.velocity.normalized);
                    lastThrowTime = Time.time;
                    Debug.Log("Swipe throw detected!");
                }
            }
        }
    }

    /// <summary>
    /// IMPROVED METHOD 3: Throw-shaped gesture recognition
    /// Player performs a throwing motion to trigger throw
    /// PROS: Most natural feeling, mimics real throwing
    /// CONS: Complex gesture recognition needed
    /// </summary>
    private void HandleThrowGestureThrow()
    {
        foreach (var handEntry in handStates)
        {
            Transform handTransform = handEntry.Key;
            CardThrowState state = handEntry.Value;

            Vector3 currentPos = handTransform.position;
            state.velocity = (currentPos - state.lastPosition) / Time.deltaTime;
            state.lastPosition = currentPos;

            // Detect throw gesture: rapid forward movement combined with hand rotation
            float velocity = state.velocity.magnitude;
            Vector3 throwDirection = handTransform.forward;

            // Check if hand is moving forward with sufficient velocity
            if (Vector3.Dot(state.velocity.normalized, throwDirection) > 0.7f &&
                velocity > velocityThreshold &&
                Time.time - lastThrowTime > throwCooldown)
            {
                ThrowCard(handTransform.position, throwDirection);
                lastThrowTime = Time.time;
                Debug.Log("Throw gesture detected!");
            }
        }
    }

    /// <summary>
    /// IMPROVED METHOD 4: Point and confirm
    /// Player points at location and confirms with a gesture (pinch or button)
    /// PROS: Most precise placement, least accidental throws
    /// CONS: Slower gameplay, requires confirmation step
    /// </summary>
    private void HandleRaycastPointThrow()
    {
        foreach (var handEntry in handStates)
        {
            Transform handTransform = handEntry.Key;
            CardThrowState state = handEntry.Value;

            // Cast ray from hand forward
            Ray ray = new Ray(handTransform.position, handTransform.forward);

            // Project onto ground plane or battlefield
            if (Physics.Raycast(ray, out RaycastHit hit, maxThrowDistance))
            {
                // Draw preview line
                if (throwArcPreview != null)
                {
                    throwArcPreview.SetPosition(0, handTransform.position);
                    throwArcPreview.SetPosition(1, hit.point);
                    throwArcPreview.enabled = true;
                }

                // Wait for confirmation (pinch or button press)
                if (DetectPinch(handTransform) && Time.time - lastThrowTime > throwCooldown)
                {
                    // Throw to the raycast hit point
                    Vector3 direction = (hit.point - handTransform.position).normalized;
                    ThrowCard(hit.point, direction);
                    lastThrowTime = Time.time;

                    if (throwArcPreview != null)
                        throwArcPreview.enabled = false;
                }
            }
        }
    }

    // ========== UTILITY FUNCTIONS ==========

    /// <summary>
    /// Detect if hand is in pinch gesture
    /// This is a placeholder - implement with your hand tracking system
    /// </summary>
    private bool DetectPinch(Transform hand)
    {
        // TODO: Implement with OVRHand, MediaPipe, or your hand tracking solution
        // For now, this is a placeholder
        return false;
    }

    /// <summary>
    /// Actually throw the card at the specified position
    /// </summary>
    private void ThrowCard(Vector3 position, Vector3 direction)
    {
        if (currentHand.Count == 0) return;
        if (cardManager == null) return;

        // Get random card from hand
        Card cardToThrow = currentHand[Random.Range(0, currentHand.Count)];

        // Activate card at throw position
        if (cardToThrow.TryGetComponent<SummonCard>(out var summon))
        {
            summon.ActivateAtPosition(position);
        }
        else if (cardToThrow.TryGetComponent<SpellCard>(out var spell))
        {
            spell.ActivateAtPosition(position);
        }

        Debug.Log($"Threw card: {cardToThrow.name}");
    }

    /// <summary>
    /// Initialize player's starting hand with random cards
    /// </summary>
    private void InitializeHand()
    {
        if (cardManager == null) return;

        currentHand.Clear();
        List<Card> allCards = cardManager.GetAllCards();

        // Give player 3-5 random cards to start
        int handSize = Random.Range(3, 6);
        for (int i = 0; i < handSize && i < allCards.Count; i++)
        {
            currentHand.Add(allCards[i]);
        }

        Debug.Log($"Hand initialized with {currentHand.Count} cards");
    }

    /// <summary>
    /// Switch the throw input method
    /// </summary>
    public void SetThrowInputMethod(ThrowInputMethod method)
    {
        inputMethod = method;
        Debug.Log($"Switched to throw method: {method}");
    }

    /// <summary>
    /// Enable/disable card throwing
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    /// <summary>
    /// Get current throw input method
    /// </summary>
    public ThrowInputMethod GetCurrentThrowMethod()
    {
        return inputMethod;
    }
}