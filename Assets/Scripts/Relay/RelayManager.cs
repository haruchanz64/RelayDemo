using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

namespace Relay
{
    public class RelayManager : MonoBehaviour
    {
        [Header("Relay Settings")]
        [SerializeField] private int maxConnections = 5; // Maximum number of connections allowed for the relay.

        [Header("UI Elements")] 
        [SerializeField] private Button hostButton; // Button to host a new relay session.
        [SerializeField] private Button joinButton; // Button to join an existing relay session.
        [SerializeField] private TMP_InputField joinCodeInputField; // Input field for the user to enter the join code.
        [SerializeField] private TextMeshProUGUI joinCodeText; // Text element to display the generated join code.

        // Start is called before the first frame update
        private async void Start()
        {
            // Initialize Unity services
            await UnityServices.InitializeAsync();

            // Sign in the user anonymously
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            // Add listeners to the buttons for hosting and joining relay sessions
            hostButton.onClick.AddListener(CreateRelay);
            joinButton.onClick.AddListener(() => JoinRelay(joinCodeInputField.text));
        }

        // Method to create a new relay session
        private async void CreateRelay()
        {
            // Create a relay allocation with the specified maximum connections
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            // Retrieve the join code for the created allocation
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            // Update the UI to display the join code
            joinCodeText.SetText($"Join Code: {joinCode}");

            // Prepare relay server data for the network transport
            var relayServerData = new RelayServerData(allocation, "dtls");
            
            // Set the relay server data in the Unity transport component
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            // Start hosting the relay session
            NetworkManager.Singleton.StartHost();
        }

        // Method to join an existing relay session using a join code
        private static async void JoinRelay(string joinCode)
        {
            // Join the relay allocation using the provided join code
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            // Prepare relay server data for the network transport
            var relayServerData = new RelayServerData(joinAllocation, "dtls");
            // Set the relay server data in the Unity transport component
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            // Start the client to connect to the relay session
            NetworkManager.Singleton.StartClient();
        }
    }
}