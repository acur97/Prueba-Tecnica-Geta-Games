using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager _gameManager;
    public ArcadeDriftController controller;
    public Transform rootCanvas;
    public Transform puntoCanvas;
    public Camera cam;
    public GameObject canvasPausa;

    [Space]
    public int vueltas = 2;
    public TextMeshProUGUI textVueltas;
    private int contadorVueltas = 0;

    public float tiempoInicial = 90;
    public TextMeshProUGUI textTiempo;
    private float _tiempo = 0;

    public UnityEvent AlFinalizar;
    public TextMeshProUGUI TextoFinal;

    public bool Jugando = false;
    private float speed = 1;
    private float elapsed = 0;
    private AudioSource musica;

    private void Awake()
    {
        QualitySettings.maxQueuedFrames = 1;

        _gameManager = this;
        _tiempo = tiempoInicial;
        contadorVueltas = vueltas + 1;

        puntoCanvas.position = cam.WorldToScreenPoint(rootCanvas.position);

        musica = GetComponent<AudioSource>();
    }

    public void Iniciar()
    {
        controller.PuedoJugar(true);
        Jugando = true;
    }

    public void Finalizar(bool termino)
    {
        if (termino)
        {
            if (_tiempo > PlayerPrefs.GetFloat("record"))
            {
                PlayerPrefs.SetFloat("record", _tiempo);
                TextoFinal.SetText("Ganaste!" + "\n" + "Segundos restantes RECORD: " + _tiempo);
            }
            else
            {
                TextoFinal.SetText("Ganaste!" + "\n" + "Segundos restantes: " + _tiempo);
            }
        }
        else
        {
            TextoFinal.SetText("Perdiste!");
        }
        Jugando = false;
        controller.PuedoJugar(false);
        AlFinalizar.Invoke();
        PlayerPrefs.SetInt("partidas", PlayerPrefs.GetInt("partidas") + 1);
    }

    private void Update()
    {
        musica.volume = speed;

        if (Jugando && Input.GetButtonDown("Cancel"))
        {
            if (canvasPausa.activeSelf)
            {
                canvasPausa.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                canvasPausa.SetActive(true);
                Time.timeScale = 0;
            }
        }

        puntoCanvas.position = cam.WorldToScreenPoint(rootCanvas.position);

        if (Jugando)
        {
            if (_tiempo > 0)
            {
                _tiempo -= Time.deltaTime;
                textTiempo.SetText("Tiempo restante:     " + _tiempo.ToString("00"));
            }
            else
            {
                Finalizar(false);
            }
        }
    }

    public void PasoMeta()
    {
        if (Jugando)
        {
            if (vueltas > 0)
            {
                contadorVueltas -= 1;
            }

            if (contadorVueltas == 0)
            {
                Finalizar(true);
            }
            else if (contadorVueltas != vueltas)
            {
                textVueltas.SetText("Vueltas restantes: " + contadorVueltas);
            }
        }
    }

    public void SubirTiempo()
    {
        if (Jugando)
        _tiempo += 10;
    }

    public void VolverMenu()
    {
        Initiate.Fade("MainMenu", Color.black, 0.5f);
        StartCoroutine(ChangeSpeed(1, 0, 2f));
        Time.timeScale = 1;
    }

    public IEnumerator ChangeSpeed(float v_start, float v_end, float duration)
    {
        elapsed = 0.0f;
        while (elapsed < duration)
        {
            speed = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        speed = v_end;
    }
}