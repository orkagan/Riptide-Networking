using System;
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

    public Client Client { get; private set; }

    [SerializeField] private string _ip;
    [SerializeField] private ushort _port;

    private void Awake()
    {
        Singleton = this;
    }
    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        //set all the functions to trigger when stuff happens
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }
    private void FixedUpdate()
    {
        Client.Update();
    }
    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }
    public void Connect()
    {
        Client.Connect($"{_ip}:{_port}");
    }
    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.SendName();
    }
    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
        {
            Destroy(player.gameObject);
        }
    }
    private void DidDisconnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
        foreach(Player player in Player.list.Values)
        {
            Destroy(player.gameObject);
        }
    }
}
