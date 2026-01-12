public class DealerAI
{
    public bool ShouldHit(El dealerHand, bool hitSoft17)
    {
        if (dealerHand == null) return false;

        int score = dealerHand.Skor();

        if (score < 17) return true;
        if (score > 17) return false;

        if (!hitSoft17) return false;
        return IsSoft17(dealerHand);
    }

    // Kart.cs deneme düzenine raðmen enum deðerine baðýmlý olmadan soft-17 tespiti
    bool IsSoft17(El hand)
    {
        if (hand == null) return false;
        if (hand.Skor() != 17) return false;

        int minTotal = 0;
        int aceCount = 0;

        foreach (var k in hand.kartlar)
        {
            if (k == null) continue;

            if (k.Rank == CardRank.Ace)
            {
                aceCount++;
                minTotal += 1;
            }
            else
            {
                minTotal += k.Deger;
            }
        }

        return aceCount > 0 && (minTotal + 10) == 17;
    }
}