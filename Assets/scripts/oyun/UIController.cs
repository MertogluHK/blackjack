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

    public void SetActionButtons(bool hit, bool stand, bool dbl)
    {
        if (hitbtn) hitbtn.SetActive(hit);
        if (standbtn) standbtn.SetActive(stand);
        if (doublebtn) doublebtn.SetActive(dbl);

        // Split şu an sistemde yok/yarım: test sırasında açık kalıp bozmasın diye kapalı tut.
        if (splitbtn) splitbtn.SetActive(false);
    }

    public void ShowOutcome(RoundOutcome outcome)
    {
        HideResults();

        if (outcome == RoundOutcome.PlayerWin && pwinbtn) pwinbtn.SetActive(true);
        else if (outcome == RoundOutcome.DealerWin && dwinbtn) dwinbtn.SetActive(true);
        else if (outcome == RoundOutcome.Push && drawbtn) drawbtn.SetActive(true);
    }
}
