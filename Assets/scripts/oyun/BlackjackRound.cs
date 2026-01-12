using UnityEngine;

public class BlackjackRound
{
    public Deste deck;
    public GameObject el1;
    public GameObject el2;
    public GameObject el3;

    // Split destekli: 2 el
    public El playerHand1 = new El();
    public El playerHand2 = null; // split yoksa null
    public El dealerHand = new El();

    public int baseBet { get; private set; }
    public int bet1 { get; private set; }   // hand1 bet
    public int bet2 { get; private set; }   // hand2 bet (split varsa)

    public bool doubled1 { get; private set; }
    public bool doubled2 { get; private set; }

    public bool stood1 { get; private set; }
    public bool stood2 { get; private set; }

    public int activeHandIndex { get; private set; } // 0 veya 1

    public bool IsSplit => playerHand2 != null;

    public BlackjackRound(int deckCount, int shuffleThreshold, int baseBet)
    {
        deck = new Deste(deckCount, shuffleThreshold);

        playerHand1 = new El();
        playerHand2 = null;
        dealerHand = new El();

        this.baseBet = baseBet;
        bet1 = baseBet;
        bet2 = 0;

        doubled1 = false;
        doubled2 = false;
        stood1 = false;
        stood2 = false;

        activeHandIndex = 0;
    }

    public El ActiveHand => (activeHandIndex == 0) ? playerHand1 : playerHand2;

    public Kart DealToPlayer(int handIndex)
    {
        Kart k = deck.Draw();
        if (handIndex == 0) playerHand1.Ekle(k);
        else playerHand2.Ekle(k);
        return k;
    }

    public Kart DealToDealer()
    {
        Kart k = deck.Draw();
        dealerHand.Ekle(k);
        return k;
    }

    public Kart Hit()
    {
        return DealToPlayer(activeHandIndex);
    }

    public void Stand()
    {
        if (activeHandIndex == 0) stood1 = true;
        else stood2 = true;
    }

    public bool CanDoubleDown()
    {
        if (activeHandIndex == 0)
            return playerHand1.kartlar.Count == 2 && !doubled1 && !stood1;
        else
            return playerHand2 != null && playerHand2.kartlar.Count == 2 && !doubled2 && !stood2;
    }

    public Kart DoubleDown()
    {
        if (!CanDoubleDown()) return null;

        if (activeHandIndex == 0)
        {
            doubled1 = true;
            bet1 *= 2;
        }
        else
        {
            doubled2 = true;
            bet2 *= 2;
        }

        return Hit();
    }

    public bool CanSplit()
    {
        if (IsSplit) return false;
        if (playerHand1.kartlar.Count != 2) return false;
        if (stood1 || doubled1) return false;

        var a = playerHand1.kartlar[0];
        var b = playerHand1.kartlar[1];
        if (a == null || b == null) return false;

        // Basit kural: rank aynıysa split
        return a.Deger == b.Deger;
    }

    public void Split()
    {
        if (!CanSplit()) return;

        playerHand2 = new El();

        // hand1'deki 2. kartı hand2'ye taşı
        var second = playerHand1.kartlar[1];
        playerHand1.kartlar.RemoveAt(1);
        playerHand2.Ekle(second);

        bet2 = baseBet;

        // Split sonrası ilk elden devam
        activeHandIndex = 0;
        stood2 = false;
        doubled2 = false;
    }

    public bool IsActiveHandDone()
    {
        var hand = ActiveHand;
        int s = hand.Skor();

        if (s >= 21) return true;

        if (activeHandIndex == 0)
        {
            if (stood1) return true;
            if (doubled1) return true;
        }
        else if (activeHandIndex == 1)
        {
            if (stood2) return true;
            if (doubled2) return true;
        }

        return false;
    }

    public bool AdvanceToNextHandIfAny()
    {
        if (!IsSplit) return false;

        if (activeHandIndex == 0)
        {
            activeHandIndex = 1;
            return true;
        }

        return false;
    }

    public bool AllPlayerHandsDone()
    {
        bool done1 = HandDone(playerHand1, stood1, doubled1);

        if (!IsSplit) return done1;

        bool done2 = HandDone(playerHand2, stood2, doubled2);
        return done1 && done2;
    }

    bool HandDone(El hand, bool stood, bool doubled)
    {
        if (hand == null) return true;
        int s = hand.Skor();
        if (s >= 21) return true;
        if (stood) return true;
        if (doubled) return true;
        return false;
    }

    public bool PlayerHasBlackjackAtStart()
    {
        return !IsSplit && playerHand1.kartlar.Count == 2 && playerHand1.Skor() == 21;
    }

    public bool DealerHasBlackjackAtStart()
    {
        return dealerHand.kartlar.Count == 2 && dealerHand.Skor() == 21;
    }

    public HandResult Resolve()
    {
        int d = dealerHand.Skor();
        bool dBust = d > 21;

        bool dealerBJ = (dealerHand.kartlar.Count == 2 && d == 21);

        var o1 = ResolveOne(playerHand1, d, dBust, dealerBJ);

        if (!IsSplit)
        {
            bool playerBJ = (playerHand1.kartlar.Count == 2 && playerHand1.Skor() == 21);

            // ✅ Player BJ (dealer BJ değilse) ayrı outcome
            var finalOutcome = o1;
            if (playerBJ && !dealerBJ) finalOutcome = RoundOutcome.PlayerBlackjackWin;

            return new HandResult
            {
                playerScore = playerHand1.Skor(),
                dealerScore = d,
                playerBust = playerHand1.Skor() > 21,
                dealerBust = dBust,
                playerBlackjack = playerBJ,
                dealerBlackjack = dealerBJ,
                outcome = finalOutcome,
                message = "RESOLVED"
            };
        }

        var o2 = ResolveOne(playerHand2, d, dBust, dealerBJ);

        RoundOutcome combined;
        if (o1 == RoundOutcome.DealerWin && o2 == RoundOutcome.DealerWin) combined = RoundOutcome.DealerWin;
        else if (o1 == RoundOutcome.Push && o2 == RoundOutcome.Push) combined = RoundOutcome.Push;
        else combined = RoundOutcome.PlayerWin;

        return new HandResult
        {
            playerScore = playerHand1.Skor(),
            dealerScore = d,
            playerBust = playerHand1.Skor() > 21,
            dealerBust = dBust,
            playerBlackjack = false,
            dealerBlackjack = dealerBJ,
            outcome = combined,
            message = $"H1:{o1} H2:{o2}"
        };
    }

    // ✅ Dealer blackjack kuralı eklendi:
    // Dealer BJ varsa player yalnızca BJ ise push, değilse dealer kazanır (player 21 olsa bile).
    RoundOutcome ResolveOne(El hand, int dealerScore, bool dealerBust, bool dealerBlackjack)
    {
        int p = hand.Skor();
        bool pBust = p > 21;

        bool playerBlackjack = (hand.kartlar.Count == 2 && p == 21);

        if (dealerBlackjack)
            return playerBlackjack ? RoundOutcome.Push : RoundOutcome.DealerWin;

        if (pBust) return RoundOutcome.DealerWin;
        if (dealerBust) return RoundOutcome.PlayerWin;

        if (p > dealerScore) return RoundOutcome.PlayerWin;
        if (p < dealerScore) return RoundOutcome.DealerWin;
        return RoundOutcome.Push;
    }
}
