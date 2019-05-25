using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameOptions : MonoBehaviour {

    public GameMode mode;
    public bool IsBotGame {get { return mode == GameMode.BOT; } }
    public bool IsOnlineGame { get { return mode == GameMode.ONLINE; } }
    public bool IsLocalGame { get { return mode == GameMode.LOCAL; } }
    public bool IsTutorialGame { get { return mode == GameMode.TUTORIAL; } }
    public bool Start3D = false;
    public int BoardSize = 3;
    public static readonly byte MaxPlayers = 2;
    public Sprite P1Token;
    public Sprite P2Token;
    public string BotPrefabName;
    
    private void Awake()
    {
        Reset();
    }
    public void Reset()
    {
        mode = GameMode.LOCAL;
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
