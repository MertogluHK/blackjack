using System.Collections;
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

    BlackjackRound round;
    DealerAI dealerAI = new DealerAI();

    RoundState state = RoundState.Idle;
    bool inputLocked = false;

    void SetButtons(bool hit, bool stand, bool dbl)
    {
        if (!ui) return;
        ui.SetActionButtons(hit, stand, dbl);
    }

    void RefreshButtons()
    {
        if (!ui || round == null || inputLocked || state != RoundState.PlayerTurn)
        {
            SetButtons(false, false, false);
            return;
        }

        if (round.IsPlayerDone())
        {
            SetButtons(false, false, false);
            return;
        }

        int score = round.playerHand.Skor();

        SetButtons(
            hit: score < 21,
            stand: true,
            dbl: round.CanDoubleDown()
        );
    }

    void LockActions()
    {
        inputLocked = true;
        state = RoundState.Idle;
        SetButtons(false, false, false);
    }

    // UI Button -> OnClick bağla
    public void YeniElBaslat()
    {
        StopAllCoroutines();

        round = new BlackjackRound(desteSayisi, karistirEsigi, baseBet);
        state = RoundState.Dealing;
        inputLocked = true;

        if (ui)
        {
            ui.HideResults();
            SetButtons(false, false, false);
        }

        if (presenter) presenter.ClearAll();

        StartCoroutine(BaslangicDagitimi());
    }

    IEnumerator BaslangicDagitimi()
    {
        // Player 1
        var p1 = round.DealToPlayer();
        if (presenter) presenter.ShowPlayerCard(p1);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Dealer 1
        var d1 = round.DealToDealer();
        if (presenter) presenter.ShowDealerCard(d1);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Player 2
        var p2 = round.DealToPlayer();
        if (presenter) presenter.ShowPlayerCard(p2);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Dealer 2
        var d2 = round.DealToDealer();
        if (presenter) presenter.ShowDealerCard(d2);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Blackjack kontrolü (başlangıç)
        if (round.PlayerHasBlackjackAtStart() || round.DealerHasBlackjackAtStart())
        {
            LockActions();

            int p = round.playerHand.Skor();
            int d = round.dealerHand.Skor();

            if (ui)
            {
                if (p == 21 && d == 21) ui.ShowOutcome(RoundOutcome.Push);
                else if (p == 21) ui.ShowOutcome(RoundOutcome.PlayerWin);
                else ui.ShowOutcome(RoundOutcome.DealerWin);
            }

            yield return new WaitForSeconds(elSonuBekleme);
            YeniElBaslat();
            yield break;
        }

        state = RoundState.PlayerTurn;
        inputLocked = false;
        RefreshButtons();
    }

    // HIT
    public void Hit()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        var c = round.Hit();
        if (presenter) presenter.ShowPlayerCard(c);

        int p = round.playerHand.Skor();

        if (p > 21)
        {
            // Bust -> dealer oynatmaya gerek yok, direkt sonuç
            LockActions();
            if (ui) ui.ShowOutcome(RoundOutcome.DealerWin);
            StartCoroutine(NextHandAfterDelay());
            return;
        }

        if (p == 21)
        {
            Stand();
            return;
        }

        RefreshButtons();
    }

    IEnumerator NextHandAfterDelay()
    {
        yield return new WaitForSeconds(elSonuBekleme);
        YeniElBaslat();
    }

    // STAND
    public void Stand()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        round.Stand();
        StartDealerTurn();
    }

    // DOUBLE
    public void DoubleDown()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;
        if (!round.CanDoubleDown()) return;

        var c = round.DoubleDown();
        if (presenter && c != null) presenter.ShowPlayerCard(c);

        round.Stand();
        StartDealerTurn();
    }

    void StartDealerTurn()
    {
        inputLocked = true;
        state = RoundState.DealerTurn;
        RefreshButtons();
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