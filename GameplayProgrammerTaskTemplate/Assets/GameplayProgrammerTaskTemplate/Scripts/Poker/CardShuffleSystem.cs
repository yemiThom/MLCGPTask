using System;
using System.Collections.Generic;

public static class CardShuffleSystem
{
    public static Stack<int> FisherYatesShuffle(Stack<int> deckStack)
    {
        int[] deck = deckStack.ToArray();
        
        for (int n = deck.Length - 1; n > 0; --n)
        {
            int k = UnityEngine.Random.Range(0,n + 1);
            (deck[n], deck[k]) = (deck[k], deck[n]);
        }
        
        return new Stack<int>(deck);
    }
}
