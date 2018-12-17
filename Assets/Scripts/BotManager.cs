using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour {

    private Bot ActiveBot;
    private Bot P1Bot;
    private Bot P2Bot;
    private List<Bot> AvailableBots;
    private List<Bot> AliveBots;
    private GameOptions options;

    

	// Use this for initialization
	void Start () {
        AvailableBots = new List<Bot>();
        AliveBots = new List<Bot>();
        options = FindObjectOfType<GameOptions>();
        if(options.IsSimulatedGame)
        {
            P1Bot = new Bot();
            P1Bot.BotPlayer = Player.P1;
            P2Bot = new Bot();
            P2Bot.BotPlayer = Player.P2;
        }
        else if(options.IsBotGame)
        {
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

    public void GetBot(Player botPlayer)
    {

    }
}
