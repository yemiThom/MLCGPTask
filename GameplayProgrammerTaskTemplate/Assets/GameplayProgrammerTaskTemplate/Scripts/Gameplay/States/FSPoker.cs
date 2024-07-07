using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

/// <summary>
/// State responsible for running the flow of poker
/// </summary>
public class FSPoker : FlowState
{
    private const int k_startBlindValue = 1;
    private const int k_buyIn = 1000;
    private const int k_playerCount = 3;
    
    private TexasHoldemPokerUI m_ui;
    private UIManager m_uiManager;

    private Deck m_deck;
    private TexasHoldemInteractionManager m_texasHoldemInteractionManager;
    private CardBacksDataObject m_cardBacksData;
    
    private PlayerData[] m_players;
    private List<PlayerUIData> m_playerUIData;
    private CardBack m_cardBack;
    private PokerPhase m_currentPhase;
    private Pot m_pot;
    
    private int m_dealChipId = -1;
    private int m_currentBetId = -1;

    public FSPoker(GameContext gameContext)
    {
        m_deck = new Deck();
  
        m_uiManager = gameContext.UIManager;
        m_texasHoldemInteractionManager = new TexasHoldemInteractionManager(m_deck, k_playerCount);
        
        m_cardBacksData = Resources.Load<CardBacksDataObject>("Data/CardBackData");
        m_cardBack = new CardBack();
        m_cardBack.SetCardBack(m_cardBacksData.CardBack[0]);
        
        m_currentPhase = PokerPhase.DealHands;
    }

    public override void OnInitialise()
    {
        m_ui = m_uiManager.LoadUIScreen<TexasHoldemPokerUI>("UI/Screens/TexasHoldemPokerUI", this);

        m_texasHoldemInteractionManager.SetTexasPokerUI(m_ui);

        m_playerUIData = Resources.LoadAll<PlayerUIData>("Data/PlayerUIData").ToList();

        m_players = new PlayerData[k_playerCount];

        for (int i = 0; i < m_players.Length; i++)
        {
            

            if (i == 0)
                m_players[i].Name = $"Player";
            else
            {
                var pUI = m_playerUIData[Random.Range(0, m_playerUIData.Count)];
                m_players[i].Name = pUI.PlayerName;
                m_players[i].Profile = pUI.PlayerSprite;
                m_playerUIData.Remove(pUI);
            }

            m_players[i].ActiveInRound = true;
            m_players[i].Currency = k_buyIn;
            m_players[i].BetState = BetState.In;
            m_players[i].PlayerLogic = new BasicLogic();
        }

        m_dealChipId = Random.Range(0, m_players.Length);
        
        m_ui.InitUI(m_players, m_cardBack);
        m_ui.UserUI.SetTextValue(m_pot.m_currentBetValue);
        for (int i = 0; i < m_players.Length; i++)
        {
            m_ui.SetHandsCurrency(i, m_players[i].Currency);
        }
    }

    public override void OnActive()
    {
        m_currentBetId = m_dealChipId;
        m_currentBetId = m_dealChipId;
        RunRoundPhase();
    }

    /// <summary>
    /// The poker round flow, must be called to progress, not updated.
    /// </summary>
    private void RunRoundPhase()
    {
        switch (m_currentPhase)
        {
            case PokerPhase.DealHands:
                m_pot = new Pot(k_startBlindValue * 2);
                m_texasHoldemInteractionManager.DealHand();
                m_ui.SetCardsInHands(m_texasHoldemInteractionManager.m_cardHand);
                m_currentPhase = PokerPhase.Blinds;
                RunRoundPhase();
                break;
            case PokerPhase.Blinds:
                PayBlinds();
                m_currentPhase = PokerPhase.Preflop;
                RunRoundPhase();
                break;
            case PokerPhase.Preflop:
                EnterBetPhase();
                break;
            case PokerPhase.Flop:
                m_texasHoldemInteractionManager.DealFlop();
                m_ui.SetCardsInTable(m_texasHoldemInteractionManager.m_cardTable);
                EnterBetPhase();
                break;
            case PokerPhase.Turn:
                m_texasHoldemInteractionManager.DealTurn();
                m_ui.SetCardsInTable(m_texasHoldemInteractionManager.m_cardTable);
                EnterBetPhase();
                break;
            case PokerPhase.River:
                m_texasHoldemInteractionManager.DealRiver();
                m_ui.SetCardsInTable(m_texasHoldemInteractionManager.m_cardTable);
                m_currentPhase++;
                RunRoundPhase();
                break;
            case PokerPhase.Reveal:
                Reveal();
                break;
        }
        
    }

    private async void Reveal()
    {
        m_ui.Reveal();
        var bestHand = m_texasHoldemInteractionManager.GetBestHand();
        await Task.Delay(1000);

        string allWinnersText = "";

        for (int i = 0; i < bestHand.Count; i++)
        {
            Debug.Log($"Best Hand: {m_players[bestHand[i]].Name}");

            allWinnersText += $"{m_players[bestHand[i]].Name}!\n";

            m_players[bestHand[i]].Currency += m_pot.m_potValue / bestHand.Count;
            m_ui.SetHandsCurrency(bestHand[i], m_players[bestHand[i]].Currency);
        }
        
        if(bestHand.Count > 1)
        {
            m_ui.SetWinnerResultText($"Joint Winners!\n{allWinnersText}");
        }
        else
        {
            m_ui.SetWinnerResultText($"Winner Winner!\n{allWinnersText}");
        }

        await Task.Delay(3000);

        m_ui.ToggleTableResults(true);

        await Task.Delay(6000);

        m_ui.ToggleTableResults(false);

        m_deck.ResetDeck();
        m_ui.Reset();
        
        for (int i = 0; i < m_players.Length; i++)
        {
            m_players[i].ActiveInRound = true;
            m_players[i].BetState = BetState.In;
        }
        
        m_dealChipId = PokerUtility.GetNextPlayerId(m_dealChipId, m_players);
        
        m_texasHoldemInteractionManager.Reset();
        m_currentPhase = PokerPhase.DealHands;
        RunRoundPhase();
    }
    
    
    private void EnterBetPhase()
    {
        m_currentBetId = m_dealChipId;

        for (int i = 0; i < m_players.Length; i++)
        {
            m_players[i].BetState = m_players[i].ActiveInRound ? m_players[i].BetState : BetState.Out;
        }

        RunBetPhase();
    }
    
    private void RunBetPhase()
    {
        bool stillIn = true;

        while (stillIn)
        {
            stillIn = false;
            bool wentRound = false;
            for (int i = m_currentBetId, j = m_players.Length; i < j; i++)
            {
                if (CheckActiveInBet(m_players[i].BetState, m_pot.m_currentBetValue, m_players[i].BetThisRound))
                {
                    stillIn = true;
                    m_pot.m_consideredBetValue = m_pot.m_currentBetValue;
           
                    if (i == 0)
                    {
                        //Main Player 
                        m_currentBetId = i;
                        m_ui.EnableUserUi();
                        m_ui.UserUI.UpdateCurrencyValue(m_pot.m_consideredBetValue - m_players[i].BetThisRound);
                        return;
                    }
               
                    //AI Players
                    int betAmount = m_players[i].PlayerLogic.RunAIBet(ref m_players[i], m_pot);
                    if (betAmount > 0)
                    {
                        Bet(i, betAmount);
                    }
                }
                
                if (!wentRound && i + 1 >= j)
                {
                    wentRound = true;
                    i = -1;
                    j = m_currentBetId;
                }
            }
        }

        m_ui.DisableUserUI();
        m_currentPhase++;

        for (int i = 0; i < m_players.Length; i++)
        {
            m_players[i].BetState = m_players[i].BetState = m_players[i].ActiveInRound ? BetState.In : BetState.Out;
            m_players[i].BetThisRound = 0;
        }

        m_pot.m_consideredBetValue = 0;
        m_pot.m_currentBetValue = 0;
        RunRoundPhase();
    }
    
    public bool CheckActiveInBet(BetState betState, int currentBet, int betThisRound)
    {
        switch (betState)
        {
            case BetState.In:
                return true;
            case BetState.Checked:
            case BetState.Bet:
            case BetState.Called:
            case BetState.Raised:
                return currentBet > betThisRound;
            case BetState.Folded:
            case BetState.Out:
                return false;
        }

        return false;
    }

    private void PayBlinds()
    {
        int smallBlindId = PokerUtility.GetNextPlayerId(m_dealChipId, m_players);
        int bigBlindId = PokerUtility.GetNextPlayerId(smallBlindId, m_players);
        
        //pay blinds
        Bet(smallBlindId, Mathf.FloorToInt(m_pot.m_currentBlindValue * 0.5f));
        m_players[smallBlindId].BetState = BetState.In;
        Bet(bigBlindId, m_pot.m_currentBlindValue);
        m_players[bigBlindId].BetState = BetState.Checked;

        m_pot.m_currentBetValue = m_pot.m_currentBlindValue;
        m_currentBetId = PokerUtility.GetNextPlayerId(smallBlindId, m_players);;
        m_ui.UserUI.UpdateCurrencyValue(m_pot.m_currentBetValue);
    }

    public override void ReceiveFlowMessages(object message)
    {
        switch (message)
        {
            case "raise":
                m_ui.UserUI.ToggleBetSetterActive(true);
                m_players[m_currentBetId].BetState = BetState.Raised;
                break;
            case "bet":
                m_ui.UserUI.ToggleBetSetterActive(true);
                m_players[m_currentBetId].BetState = BetState.Bet;
                break;
            case "check":
                m_players[m_currentBetId].BetState = BetState.Checked;
                AdvanceBet();
                break;
            case "fold":
                m_players[m_currentBetId].BetState = BetState.Folded;
                m_players[m_currentBetId].ActiveInRound = false;
                AdvanceBet();
                break;
            case "call":
                m_pot.m_consideredBetValue = m_pot.m_currentBetValue - m_players[m_currentBetId].BetThisRound;
                m_players[m_currentBetId].BetState = BetState.Called;
                AdvanceBet();
                break;
            case "increaseBet":
                m_pot.m_consideredBetValue = Mathf.Min(m_players[m_currentBetId].Currency,m_pot.m_consideredBetValue + m_pot.m_currentBlindValue);
                m_ui.UserUI.UpdateCurrencyValue(m_pot.m_consideredBetValue);
                break;
            case "decreaseBet":
                m_pot.m_consideredBetValue = Mathf.Max(m_pot.m_currentBlindValue,m_pot.m_consideredBetValue - m_pot.m_currentBlindValue);
                m_ui.UserUI.UpdateCurrencyValue(m_pot.m_consideredBetValue);
                break;
            case "resetBet":
                m_pot.m_consideredBetValue = m_pot.m_currentBlindValue;
                m_ui.UserUI.UpdateCurrencyValue(m_pot.m_consideredBetValue);
                break;
            case "confirmBet":
                m_ui.UserUI.ToggleBetSetterActive(false);
                AdvanceBet();
                break;
            case "cancelBet":
                m_players[m_currentBetId].BetState = BetState.In;
                m_pot.m_consideredBetValue = m_pot.m_currentBetValue = m_pot.m_consideredBetValue;
                m_ui.UserUI.ToggleBetSetterActive(false);
                break;
            case SliderFlowMessage sliderFlowMessage:
                m_pot.m_consideredBetValue = (int) Mathf.Lerp(m_pot.m_currentBetValue, m_players[m_currentBetId].Currency, sliderFlowMessage.SliderValue);
                m_ui.UserUI.UpdateCurrencyValue(m_pot.m_consideredBetValue);
                break;
        }
    }

    private void Bet(int playerId, int currencyValue)
    {
        m_players[playerId].Currency -= currencyValue;
        m_players[playerId].BetThisRound += currencyValue;
        m_pot.m_potValue += currencyValue;
        
        m_ui.SetHandsCurrency(playerId, m_players[playerId].Currency);
        m_ui.SetPotCurrency(m_pot.m_potValue);
    }

    private void AdvanceBet()
    {
        Bet(m_currentBetId, m_pot.m_consideredBetValue);
        m_pot.m_currentBetValue = m_pot.m_consideredBetValue;
        
        m_currentBetId = PokerUtility.GetNextPlayerId(m_currentBetId, m_players);
        RunBetPhase();
    }
    
    public override void ActiveUpdate()
    {
        m_ui.UpdateUI();
    }
    
    public override void ActiveFixedUpdate()
    {
    }

    public override void OnInactive()
    {
    }

    public override void OnDismiss()
    {
    }

    public override void FinishDismiss()
    {
        m_ui.DestroyUI();
    }
}
