public class DealerAI
{
    public bool ShouldHit(El dealerHand, bool hitSoft17)
    {
        int score = dealerHand.Skor();

        if (score < 17) return true;
        if (score > 17) return false;

        if (!hitSoft17) return false;
        return IsSoft17(dealerHand);
    }

    bool IsSoft17(El hand)
    {
        int raw = 0;
        int ace = 0;

        foreach (var k in hand.kartlar)
        {
            if (k.Rank == CardRank.Ace) { raw += 11; ace++; }
            else if (k.Deger == 10) raw += 10;
            else raw += (int)k.Rank + 2;
        }

        return ace > 0 && raw == 17 && hand.Skor() == 17;
    }
}
