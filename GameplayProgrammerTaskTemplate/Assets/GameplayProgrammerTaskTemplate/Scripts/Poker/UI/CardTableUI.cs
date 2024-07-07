using UnityEngine;
using UnityEngine.UI;

public class CardTableUI : MonoBehaviour
{
    [SerializeField]
    private Image[] m_cards;
    
    public void SetCards(CardTable m_cardHand)
    {
        Card[] cards = m_cardHand.GetCards();
        for (int i = 0; i < cards.Length; i++)
        {
            var moveMethod = m_cards[i].GetComponent<MoveAndSpinUI>();
            if (moveMethod != null)
            {
                moveMethod.MoveFromOffscreen();
                moveMethod.SetHasMovedBool(true);
            }

            m_cards[i].sprite = cards[i].Sprite;
            m_cards[i].color = Color.white;
        }
    }

    public void ClearTable()
    {
        for (int i = 0; i < m_cards.Length; i++)
        {
            var moveMethod = m_cards[i].GetComponent<MoveAndSpinUI>();
            if (moveMethod != null)
            {
                moveMethod.SetHasMovedBool(false);
            }

            m_cards[i].sprite = null;
            m_cards[i].color = Color.clear;
        }
    }
}
