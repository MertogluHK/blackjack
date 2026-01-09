using UnityEngine;

public class anamenü : MonoBehaviour
{
    public GameObject anaMenuEkrani;
    public GameObject oyunEkrani;
    public GameObject durdurmaEkrani;

    public void AnaMenuEkraniAc()
    {
        anaMenuEkrani.SetActive(true);
        oyunEkrani.SetActive(false);
        durdurmaEkrani.SetActive(false);
    }
}
