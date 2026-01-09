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

    void SetActionButtons(bool hit, bool stand, bool dbl)
    {
        if (!ui) return;
        ui.SetActionButtons(hit, stand, dbl);
    }

    void RefreshButtons()
    {
        if (!ui || round == null || inputLocked || state != RoundState.PlayerTurn)
        {
            SetActionButtons(false, false, false);
            return;
        }

        if (round.IsHandDone(0))
        {
            SetActionButtons(false, false, false);
            return;
        }

        int score = round.playerHands[0].Skor();

        SetActionButtons(
            hit: score < 21,
            stand: true,
            dbl: round.CanDoubleDown(0)
        );
    }

    public void YeniElBaslat()
    {
        StopAllCoroutines();

        round = new BlackjackRound(desteSayisi, karistirEsigi, baseBet);

        state = RoundState.Dealing;
        inputLocked = true;

        if (ui)
        {
            ui.HideResults();
            SetActionButtons(false, false, false);
        }

        if (presenter)
        {
            presenter.ClearAll();
            presenter.EnsurePlayerHandAreas(1); // tek el
        }

        StartCoroutine(BaslangicDagitimi());
    }

    public void Hit()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        var card = round.Hit(0);
        presenter.ShowPlayerCard(0, card);

        int p = round.playerHands[0].Skor();

        if (p > 21)
        {
            round.Stand(0);
            StartDealerTurn();
            return;
        }

        if (p == 21)
        {
            Stand();
            return;
        }

        RefreshButtons();
    }

    public void Stand()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        round.Stand(0);
        StartDealerTurn();
    }

    public void DoubleDown()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;
        if (!round.CanDoubleDown(0)) return;

        var card = round.DoubleDown(0);
        presenter.ShowPlayerCard(0, card);

        // double biter
        round.Stand(0);
        StartDealerTurn();
    }

    void StartDealerTurn()
    {
        inputLocked = true;
        state = RoundState.DealerTurn;
        RefreshButtons();
        StartCoroutine(DealerOynasinVeSonuc());
    }

    IEnumerator BaslangicDagitimi()
    {
        // Player 1
        presenter.ShowPlayerCard(0, round.DealToPlayer(0));
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Dealer 1
        presenter.ShowDealerCard(round.DealToDealer());
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Player 2
        presenter.ShowPlayerCard(0, round.DealToPlayer(0));
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Dealer 2
        presenter.ShowDealerCard(round.DealToDealer());
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        state = RoundState.PlayerTurn;
        inputLocked = false;
        RefreshButtons();
    }

    IEnumerator DealerOynasinVeSonuc()
    {
        while (dealerAI.ShouldHit(round.dealerHand, dealerHitsSoft17))
        {
            var c = round.DealToDealer();
            presenter.ShowDealerCard(c);
            yield return new WaitForSeconds(kartCekmeGecikmesi);
        }

        var res = round.ResolveHand(0);
        if (ui) ui.ShowOutcome(res.outcome);

        yield return new WaitForSeconds(elSonuBekleme);
        YeniElBaslat();
    }
}
