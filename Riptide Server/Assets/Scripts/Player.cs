using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }

    public PlayerMovement Movement => _movement;

    [SerializeField] private PlayerMovement _movement;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    /// <summary>
    /// Spawns player as a game object and adds them to a list with their ID and username
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="username">Player Username</param>
    public static void Spawn(ushort id, string username)
    {
        //sends the newly spawned player's ID to other existing players on the list
        foreach (Player otherPlayer in list.Values)
            otherPlayer.SendSpawned(id);
        
        //Instantiates player prefab and grabs the script reference to assign data
        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(0f,1f,0f), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username; //if username is null or empty use Guest {id}

        player.SendSpawned();
        list.Add(id, player);
    }

    #region Messages
    private void SendSpawned() //sends message with player data to all connected clients
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)));
    }
    private void SendSpawned(ushort toClientId) //sends message with player data to specified ClientId
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)), toClientId);
    }

    /// <summary>
    /// Adds a player's ID, Username, and Position to a message and returns it
    /// </summary>
    /// <param name="message">Input message</param>
    /// <returns>Message with added data</returns>
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);

        return message;
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.Movement.SetInput(message.GetBools(6), message.GetVector3());
        }
    }
    #endregion
}
