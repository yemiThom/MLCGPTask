public class BasicLogic : IPlayerLogic
{
    public int RunAIBet(ref PlayerData playerData, Pot pot)
    {
        if (playerData.Currency > 0)
        {
            if (pot.m_currentBetValue == 0)
            {
                playerData.BetState = BetState.Checked;
            }
            else
            {
                playerData.BetState = BetState.Called;
                return pot.m_consideredBetValue;
            }
        }
        else
        {
            playerData.BetState = BetState.Folded;
        }

        return 0;
    }

}
