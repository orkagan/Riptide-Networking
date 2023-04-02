using UnityEngine;
using Riptide;
using Riptide.Utils;

public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    playerMovement,
}
public enum ClientToServerId : ushort
{
    name = 1,
    input,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destronying duplicate!");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }

    [SerializeField] private ushort _port;
    [SerializeField] private ushort _maxClientCount;

    private void Awake()
    {
        Singleton = this;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        //Sets where the network outputs logs to
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(_port, _maxClientCount);
        Server.ClientDisconnected += PlayerLeft; //when a client leaves the server run the playerleft func
    }
    private void FixedUpdate()
    {
        Server.Update();
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        //Destroys the game object from the client's ID specified in the server disconnect event arguments
        if (Player.list.TryGetValue(e.Client.Id, out Player player))
        {
            Destroy(player.gameObject);
        }
    }
}
