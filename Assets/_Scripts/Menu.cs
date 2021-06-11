using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public Cargando _cargando;
    public float velocidad = 0.5f;
    [Space]
    public float valorMax = 15;
    public float valorMin = 5;
    public TextMeshProUGUI btn_start;
    public TextMeshProUGUI textStart;

    [Space]
    public GameObject pantalla1;
    public GameObject pantalla2;

    private float alpha = 0;
    private bool dir = true;
    private Color32 colorT = new Color32(255, 255, 255, 255);

    private void Awake()
    {
        QualitySettings.maxQueuedFrames = 1;

        pantalla1.SetActive(true);
        pantalla2.SetActive(false);

        if (PlayerPrefs.HasKey("record"))
        {
            textStart.SetText(textStart.text + "\n" + "\n" + "Segundos restantes mas alto :" + PlayerPrefs.GetFloat("record").ToString("F2") + "  Partidas: " + PlayerPrefs.GetInt("partidas"));
        }
        else if (PlayerPrefs.HasKey("partidas"))
        {
            textStart.SetText(textStart.text + "\n" + "\n" + "Partidas: " + PlayerPrefs.GetInt("partidas"));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();
        }

        if (dir)
        {
            alpha += velocidad * Time.deltaTime;
        }
        else
        {
            alpha -= velocidad * Time.deltaTime;
        }

        if (alpha >= valorMax)
        {
            dir = false;
        }
        if (alpha <= valorMin)
        {
            dir = true;
        }

        colorT.a = (byte)alpha;
        btn_start.color = colorT;
    }

    public void PasarCarro()
    {
        _cargando.IniciarCarga();
        //Initiate.Fade(_Loading, Color.black, 0.5f);
        //pantalla1.SetActive(false);
        //pantalla2.SetActive(true);
    }

    public void salir()
    {
        Application.Quit();
    }
}