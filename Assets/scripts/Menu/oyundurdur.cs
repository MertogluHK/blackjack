using UnityEngine;

public class oyundurdur : MonoBehaviour
{
    public GameObject durdurmaEkrani;

    public void OyunuDurdur()
    {
        durdurmaEkrani.SetActive(true);
        Time.timeScale = 0f; // Oyun zamanýný durdur
    }
}
