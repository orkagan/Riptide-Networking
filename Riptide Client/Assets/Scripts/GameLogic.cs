using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, destronying duplicate!");
                Destroy(value);
            }
        }
    }

    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject LocalPlayerPrefab => _localPlayerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _playerPrefab;
    private void Awake()
    {
        Singleton = this;
    }
}
