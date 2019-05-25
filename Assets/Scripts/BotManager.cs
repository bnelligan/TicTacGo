using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour {
    
    private Bot P1Bot;
    private Bot P2Bot;
    private List<Bot> AvailableBots;
    private List<Bot> AliveBots;
    private GameOptions options;
    
    

	// Use this for initialization
	void Awake () {
        AvailableBots = new List<Bot>();
        AliveBots = new List<Bot>();
        options = FindObjectOfType<GameOptions>();
        if((options.IsBotGame || options.IsTutorialGame) && P2Bot == null)
        {
            GameObject botPrefab = Resources.Load<GameObject>($"Prefabs/Bots/{options.BotPrefabName}");
            P2Bot = GameObject.Instantiate(botPrefab, transform).GetComponent<Bot>();
            P2Bot.BotPlayer = Player.P2;
        }
        
	}
	
    public void StartGame()
    {

    }
    public void MakeMove()
    {
        
    }
    public void GameOver(Player winner)
    {
        if (winner == Player.P1)
        {
            Destroy(P2Bot);
            P2Bot = P1Bot.CloneAndMutate();
            P2Bot.BotPlayer = P1Bot.OpponentPlayer;
        }
        else if (winner == P2Bot.BotPlayer)
        {
            Destroy(P1Bot);
            P1Bot = P2Bot.CloneAndMutate();
            P1Bot.BotPlayer = P2Bot.OpponentPlayer;
        }
    }
    public void NotifyTurn(Player activePlayer)
    {
        if(options.IsLocalGame)
        {
            if(activePlayer == Player.P1)
            {
                P1Bot.MakeMove();
            }
            else if(activePlayer == Player.P2)
            {
                P2Bot.MakeMove();
            }
        }
        else if(options.IsBotGame || options.IsTutorialGame)
        {
            if(activePlayer == Player.P2)
            {
                P2Bot.MakeDelayedMove();
            }
        }
    }
    
}
