using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameOptions : MonoBehaviour {

    public GameMode mode;
    public bool IsBotGame {get { return mode == GameMode.BOT; } }
    public bool IsOnlineGame { get { return mode == GameMode.ONLINE; } }
    public bool IsSimulatedGame { get { return mode == GameMode.LOCAL; } }
    public bool Start3D = false;
    public int BoardSize = 3;
    public static readonly byte MaxPlayers = 2;
    public Sprite PlayerToken;
    public Sprite OtherPlayerToken;
    
    private void Awake()
    {
        Reset();
    }
    private void Start()
    {
        Board board = FindObjectOfType<Board>();
        if(board)
        {
            board.SetTokenSprites(PlayerToken, OtherPlayerToken);
        }
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
