using UnityEngine;

public class oyundevam : MonoBehaviour
{
    public GameObject durdurmaEkrani;

    public void OyunaDevamEt()
    {
        durdurmaEkrani.SetActive(false);
        Time.timeScale = 1f; // Oyun zamanýný devam ettir
    }   
}
