using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Cargando : MonoBehaviour {

    public Image fondo;
    public TextMeshProUGUI testo;
    public Image sliderBackground;
    public Image sliderFill;
    public Slider sliderV;
    public GameObject UI;

    private float speed = 0;
    private readonly Color blackOff = new Color(0, 0, 0, 0);
    private readonly string _GamePlay = "GamePlay";

    private AudioSource musica;

    private void Awake()
    {
        UI.SetActive(false);
        fondo.color = blackOff;
        testo.color = blackOff;
        sliderBackground.color = blackOff;
        sliderFill.color = blackOff;
        sliderV.value = 0;

        musica = GetComponent<AudioSource>();
    }

    public void IniciarCarga()
    {
        UI.SetActive(true);
        StartCoroutine(ChangeSpeed(0, 1, 1f));
    }

    public IEnumerator ChangeSpeed(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            speed = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        speed = v_end;
        StartCoroutine(LoadSceneA());
    }

    IEnumerator LoadSceneA()
    {
        AsyncOperation operacion = SceneManager.LoadSceneAsync(_GamePlay);

        while (!operacion.isDone)
        {
            float progreso = Mathf.Clamp01(operacion.progress / 0.9f);

            sliderV.value = progreso;

            yield return null;
        }
    }

    private void Update()
    {
        fondo.color = new Color(0, 0, 0, speed);
        testo.color = new Color(1, 1, 1, speed);
        sliderBackground.color = new Color(1, 1, 1, speed);
        sliderFill.color = new Color(1, 0, 0, speed);

        musica.volume = speed.Remap(0, 1, 1, 0);
    }
}
