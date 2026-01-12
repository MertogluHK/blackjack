using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public UIController ui;
    public CardPresenter presenter;

    [Header("Round Settings")]
    public int desteSayisi = 4;
    public int karistirEsigi = 100;
    public int baseBet = 10;

    [Header("Timings")]
    public float kartCekmeGecikmesi = 0.25f;
    public float elSonuBekleme = 1.5f;

    [Header("Dealer Rules")]
    public bool dealerHitsSoft17 = false;

    [Header("Hand Arrows")]
    public GameObject Hand1;
    public GameObject Hand2;
    public GameObject Hand3;

    BlackjackRound round;
    DealerAI dealerAI = new DealerAI();

    RoundState state = RoundState.Idle;
    bool inputLocked = false;
    int standCount = 0;

    // ✅ Dealer BJ olsa bile hemen bitirmemek için
    bool dealerBlackjackPending = false;

    int PlayerSlotForHand(int handIndex)
    {
        if (round != null && round.IsSplit)
            return handIndex == 0 ? 1 : 2;

        return 0;
    }

    void SetButtons(bool hit, bool stand, bool split, bool dbl)
    {
        if (!ui) return;
        ui.SetActionButtons(hit, stand, split, dbl);
    }

    void RefreshButtons()
    {
        if (!ui || round == null || inputLocked || state != RoundState.PlayerTurn)
        {
            SetButtons(false, false, false, false);
            return;
        }

        if (round.IsActiveHandDone())
        {
            SetButtons(false, false, false, false);
            return;
        }

        int score = round.ActiveHand.Skor();

        // ✅ Dealer BJ pending ise split/double kapat (istersen açabilirsin ama blackjack oyunlarında mantıklı değil)
        bool allowSplit = !dealerBlackjackPending && round.CanSplit();
        bool allowDbl = !dealerBlackjackPending && round.CanDoubleDown();

        SetButtons(
            hit: score < 21,
            stand: true,
            split: allowSplit,
            dbl: allowDbl
        );
    }

    void LockActions()
    {
        inputLocked = true;
        state = RoundState.Idle;
        SetButtons(false, false, false, false);
    }

    public void YeniElBaslat()
    {
        StopAllCoroutines();
        standCount = 0;

        dealerBlackjackPending = false;

        Hand2.SetActive(false);
        Hand3.SetActive(false);
        Hand1.SetActive(false);

        round = new BlackjackRound(desteSayisi, karistirEsigi, baseBet);
        state = RoundState.Dealing;
        inputLocked = true;

        if (ui)
        {
            ui.HideResults();
            ui.RestorePreSplitButtonPositions();
            SetButtons(false, false, false, false);
        }

        if (presenter) presenter.ClearAll();

        Hand1.SetActive(true);
        StartCoroutine(YeniElVeDagit());
    }

    IEnumerator YeniElVeDagit()
    {
        yield return null;
        yield return BaslangicDagitimi();
    }

    IEnumerator BaslangicDagitimi()
    {
        var p1 = round.DealToPlayer(0);
        if (presenter) presenter.ShowPlayerCard(0, p1);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // ✅ Dealer ilk kart kapalı
        var d1 = round.DealToDealer();
        if (presenter) presenter.ShowDealerCardHidden(d1);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        var p2 = round.DealToPlayer(0);
        if (presenter) presenter.ShowPlayerCard(0, p2);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        var d2 = round.DealToDealer();
        if (presenter) presenter.ShowDealerCard(d2);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        bool pBJ = round.PlayerHasBlackjackAtStart();
        bool dBJ = round.DealerHasBlackjackAtStart();

        // ✅ Player BJ, dealer BJ değilse: istersen anında bitir
        if (pBJ && !dBJ)
        {
            // dealer kartını açmak istersen burada açabilirsin (opsiyonel)
            if (presenter) presenter.RevealDealerFirstCard(round.dealerHand.kartlar[0]);

            LockActions();
            if (ui) ui.ShowOutcome(RoundOutcome.PlayerBlackjackWin);

            yield return new WaitForSeconds(elSonuBekleme);
            YeniElBaslat();
            yield break;
        }

        // ✅ Dealer BJ varsa: HEMEN BİTİRME
        if (dBJ)
        {
            dealerBlackjackPending = true;

            state = RoundState.PlayerTurn;
            inputLocked = false;

            // player da BJ ise player turu gereksiz -> direkt dealer turu (kartı açıp resolve edecek)
            if (pBJ)
            {
                StartDealerTurn();
                yield break;
            }

            RefreshButtons();
            yield break;
        }

        // normal akış
        state = RoundState.PlayerTurn;
        inputLocked = false;
        RefreshButtons();
    }

    // SPLIT
    public void Split()
    {
        Hand1.SetActive(false);
        Hand2.SetActive(true);

        if (inputLocked || state != RoundState.PlayerTurn) return;
        if (dealerBlackjackPending) return; // ✅ dealer BJ pending iken split yok
        if (!round.CanSplit()) return;

        round.Split();

        if (ui) ui.ApplySplitButtonPositions();

        if (presenter)
        {
            presenter.ClearAll();

            // ✅ dealer kartları tekrar basılırken ilk kart yine kapalı olmalı
            // dealerhand[0] kapalı, [1] açık:
            if (round.dealerHand.kartlar.Count > 0) presenter.ShowDealerCardHidden(round.dealerHand.kartlar[0]);
            if (round.dealerHand.kartlar.Count > 1) presenter.ShowDealerCard(round.dealerHand.kartlar[1]);

            foreach (var k in round.playerHand1.kartlar)
                presenter.ShowPlayerCard(1, k);

            foreach (var k in round.playerHand2.kartlar)
                presenter.ShowPlayerCard(2, k);
        }

        var c1 = round.DealToPlayer(0);
        if (presenter) presenter.ShowPlayerCard(1, c1);

        var c2 = round.DealToPlayer(1);
        if (presenter) presenter.ShowPlayerCard(2, c2);

        RefreshButtons();
    }

    public void Hit()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        var c = round.Hit();
        if (presenter)
            presenter.ShowPlayerCard(PlayerSlotForHand(round.activeHandIndex), c);

        if (round.IsActiveHandDone())
        {
            OnActiveHandFinished();
            return;
        }

        RefreshButtons();
    }

    public void Stand()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        if (standCount == 0 && round.IsSplit)
        {
            Hand1.SetActive(false);
            Hand2.SetActive(false);
            Hand3.SetActive(true);
        }
        else
        {
            Hand1.SetActive(false);
            Hand2.SetActive(false);
            Hand3.SetActive(false);
        }

        standCount++;
        round.Stand();
        OnActiveHandFinished();
    }

    public void DoubleDown()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;
        if (dealerBlackjackPending) return; // ✅ dealer BJ pending iken double yok
        if (!round.CanDoubleDown()) return;

        var c = round.DoubleDown();
        if (presenter && c != null)
            presenter.ShowPlayerCard(PlayerSlotForHand(round.activeHandIndex), c);

        round.Stand();
        OnActiveHandFinished();
    }

    void OnActiveHandFinished()
    {
        if (round.AdvanceToNextHandIfAny())
        {
            RefreshButtons();
            return;
        }

        StartDealerTurn();
    }

    void StartDealerTurn()
    {
        inputLocked = true;
        state = RoundState.DealerTurn;
        RefreshButtons();

        // ✅ Dealer kapalı kartı burada aç
        if (presenter && round.dealerHand.kartlar.Count > 0)
            presenter.RevealDealerFirstCard(round.dealerHand.kartlar[0]);

        StartCoroutine(DealerOynasinVeSonuc());
    }

    IEnumerator DealerOynasinVeSonuc()
    {
        while (dealerAI.ShouldHit(round.dealerHand, dealerHitsSoft17))
        {
            var c = round.DealToDealer();
            if (presenter) presenter.ShowDealerCard(c);
            yield return new WaitForSeconds(kartCekmeGecikmesi);
        }

        state = RoundState.Resolving;

        var res = round.Resolve();
        if (ui) ui.ShowOutcome(res.outcome);

        yield return new WaitForSeconds(elSonuBekleme);
        YeniElBaslat();
    }
}
