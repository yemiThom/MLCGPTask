using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHandUI : MonoBehaviour
{
    [SerializeField]
    private Image m_characterProfile;
    [SerializeField]
    private TextMeshProUGUI m_characterName;
    [SerializeField]
    private TextMeshProUGUI m_currencyText;
    [SerializeField]
    private Image[] m_cards;

    private CardHand m_cardHand;
    
    public void SetCharacter(string characterName, Sprite characterProfile = null)
    {
        m_characterName.text = characterName;
        if(characterProfile != null)
            m_characterProfile.sprite = characterProfile;
    }
    
    public void SetCurrencyText(int currency)
    {
        m_currencyText.text = $"�{currency}";
    }
    
    public void SetCards(CardHand cardHand, Sprite cardBack, bool show)
    {
        m_cardHand = cardHand;
        for (int i = 0; i < cardHand.Cards.Length; i++)
        {
            if (show)
            {
                Reveal();
            }
            else
            {
                m_cards[i].sprite = cardBack;
            }
        }
    }

    public void Reveal()
    {
        for (int i = 0; i < m_cardHand.Cards.Length; i++)
        {
            m_cards[i].sprite = m_cardHand.Cards[i].Sprite;
        }
    }
}
