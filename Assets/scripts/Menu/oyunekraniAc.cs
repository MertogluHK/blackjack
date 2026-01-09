using UnityEngine;

public class oyunekraniAc : MonoBehaviour
{
    public GameObject oyunEkrani;
    public GameObject anaMenuEkrani;

    public void OyunEkraniAc()
    {
        oyunEkrani.SetActive(true);
        anaMenuEkrani.SetActive(false);
    }

}
