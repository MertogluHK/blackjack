using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Action Buttons")]
    public GameObject hitbtn;
    public GameObject standbtn;

    public GameObject splitbtn;
    public GameObject doublebtn;

    [Header("Result Buttons/Panels")]
    public GameObject pwinbtn;
    public GameObject dwinbtn;
    public GameObject drawbtn;

    public void HideResults()
    {
        if (pwinbtn) pwinbtn.SetActive(false);
        if (dwinbtn) dwinbtn.SetActive(false);
        if (drawbtn) drawbtn.SetActive(false);
    }

    public void SetActionsVisible(bool visible)
    {
        if (hitbtn) hitbtn.SetActive(visible);
        if (standbtn) standbtn.SetActive(visible);

        // split/double görünürlüğü ayrıca RefreshActionAvailability ile kontrol edilir
        if (!visible)
        {
            if (splitbtn) splitbtn.SetActive(false);
            if (doublebtn) doublebtn.SetActive(false);
        }
    }

    public void RefreshActionAvailability(BlackjackRound round, RoundState state)
    {
        if (state != RoundState.PlayerTurn)
        {
            if (splitbtn) splitbtn.SetActive(false);
            if (doublebtn) doublebtn.SetActive(false);
            return;
        }

        int i = round.activeHandIndex;

        if (splitbtn) splitbtn.SetActive(round.CanSplit(i));
        if (doublebtn) doublebtn.SetActive(round.CanDoubleDown(i));
    }

    public void ShowOutcome(RoundOutcome outcome)
    {
        HideResults();

        if (outcome == RoundOutcome.PlayerWin && pwinbtn) pwinbtn.SetActive(true);
        else if (outcome == RoundOutcome.DealerWin && dwinbtn) dwinbtn.SetActive(true);
        else if (outcome == RoundOutcome.Push && drawbtn) drawbtn.SetActive(true);
    }

    public void SetActionButtons(bool hit, bool stand, bool split, bool dbl)
    {
        if (hitbtn) hitbtn.SetActive(hit);
        if (standbtn) standbtn.SetActive(stand);

        if (splitbtn) splitbtn.SetActive(split);
        if (doublebtn) doublebtn.SetActive(dbl);
    }

}
