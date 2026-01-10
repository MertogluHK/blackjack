public class DealerAI
{
    public bool ShouldHit(El dealerHand, bool hitSoft17)
    {
        if (dealerHand == null) return false;

        int score = dealerHand.Skor();

        if (score < 17) return true;
        if (score > 17) return false;

        // score == 17
        if (!hitSoft17) return false;
        return IsSoft17(dealerHand);
    }

    // Kart.cs enum düzenine BAÐIMLI OLMAYAN soft-17 kontrolü.
    // Soft 17: toplam 17 ve en az bir as "yumuþak" (11 gibi davranýyor) olmalý.
    bool IsSoft17(El hand)
    {
        if (hand == null) return false;
        if (hand.Skor() != 17) return false;

        // "Min total": aslarý 1 sayarak toplam. Sonra bir asý 11'e çekebilmek için +10 eklenir.
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
                // Kart.cs deneme modunda bile Deger tutarlý bir "baz deðer" saðlýyor (10'luklar 10).
                minTotal += k.Deger;
            }
        }

        // En az bir as olmalý ve bir asý 11'e çekince 17 olmalý.
        return aceCount > 0 && (minTotal + 10) == 17;
    }
}