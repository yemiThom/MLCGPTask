using System;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TexasHoldemPokerUI : FlowScreenUI
{
    [SerializeField]
    private Transform[] m_handZones;
    
    [SerializeField]
    private TextMeshProUGUI m_potValue;
    
    [SerializeField]
    private CardTableUI m_cardTable;
    
    [SerializeField]
    private CardHandUI m_handPrefab;
    
    public UserUI UserUI;

    private CardBack m_cardBack;
    private List<CardHandUI> m_cardHands = new List<CardHandUI>();
    
    public void InitUI(PlayerData[] participants, CardBack cardBack)
    {
        UserUI.gameObject.SetActive(false);
        m_cardBack = cardBack;
        
        for (int i = 0; i < participants.Length; i++)
        {
            SpawnHand(i, participants[i].Name);
        }
        
        SetPotCurrency(0);
    }

    public void SpawnHand(int spawnZone, string name)
    {
        CardHandUI handUI = Instantiate(m_handPrefab, m_handZones[spawnZone]);
        handUI.SetCharacter(name);
        
        m_cardHands.Add(handUI);
    }
    
    public void SetCardsInHands(CardHand[] cardHand)
    {
        for (int i = 0; i < cardHand.Length; i++)
        {
            m_cardHands[i].SetCards(cardHand[i], m_cardBack.CardBackSprite, i == 0);
        }
    }
    
    public void SetHandsCurrency(int id, int value)
    {
        m_cardHands[id].SetCurrencyText(value);
    }
    
    public void SetPotCurrency(int value)
    {
        m_potValue.text = $"£{value.ToString()}";
    }
    
    public void SetCardsInTable(CardTable cardTable)
    {
        m_cardTable.SetCards(cardTable);
    }

    public void EnableUserUi()
    {
        UserUI.gameObject.SetActive(true);
    }
    
    public void DisableUserUI()
    {
        UserUI.gameObject.SetActive(false);
    }

    public void Reveal()
    {
        for (int i = 0; i < m_cardHands.Count; i++)
        {
            m_cardHands[i].Reveal();
        }
    }
    
    public void Reset()
    {
        m_cardTable.ClearTable();
    }

    public override void UpdateUI()
    {
    }

    public override void DestroyUI()
    {
        Destroy(gameObject);
    }
}
