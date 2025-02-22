using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class Movement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f; // Speed of the movement
        private Rigidbody rb; // Reference to the Rigidbody component

        private void Start()
        {
            // Get the Rigidbody component attached to this GameObject
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Only allow movement for the local player
            if (!IsLocalPlayer) return;

            // Get input for movement
            var moveHorizontal = Input.GetAxis("Horizontal");
            var moveVertical = Input.GetAxis("Vertical");

            // Create a movement vector
            var movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * moveSpeed;

            // Move the Rigidbody
            rb.MovePosition(rb.position + movement * Time.deltaTime);
        }
    }
}   