using System;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserUI : MonoBehaviour
{
    [SerializeField]
    private Button m_checkButton;
    [SerializeField]
    private Button m_foldButton;
    [SerializeField]
    private Button m_raiseButton;
    [SerializeField]
    private Button m_betButton;
    [SerializeField]
    private Button m_callButton;

    [SerializeField] 
    private TextMeshProUGUI m_currencyText;
    [SerializeField]
    private GameObject m_BetUIGO;
    [SerializeField] 
    private BetSetterUI m_betSetter;

    private void Awake()
    {
        ToggleBetSetterActive(false);
    }

    public void UpdateCurrencyValue(int newValue)
    {
        m_currencyText.text = $"£{newValue}";


        bool checkable = newValue == 0;
        m_callButton.gameObject.SetActive(!checkable);
        m_betButton.gameObject.SetActive(checkable);
        m_checkButton.gameObject.SetActive(checkable);
    }

    public void SetTextValue(int value)
    {
        m_currencyText.text = $"£{value}";
    }
    
    public void ToggleBetSetterActive(bool active)
    {
        m_BetUIGO.SetActive(!active);
        m_betSetter.gameObject.SetActive(active);
    }
}
