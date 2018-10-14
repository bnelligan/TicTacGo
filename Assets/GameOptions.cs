using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameOptions : MonoBehaviour {

    public bool IsBotGame;
    public bool IsOnlineGame;
    public int BoardSize = 3;
    public static readonly byte MaxPlayers = 2;

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        IsBotGame = false;
        IsOnlineGame = false;
    }

    public RoomOptions GetRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "BoardSize" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "BoardSize", BoardSize } };
        roomOptions.MaxPlayers = MaxPlayers;
        return roomOptions;
    }

}
