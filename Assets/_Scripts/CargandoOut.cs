using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CargandoOut : MonoBehaviour {

    public Image fondo;
    public TextMeshProUGUI testo;
    public Image sliderFill;
    public GameObject UI;
    public TextMeshProUGUI textoCount;
    public UnityEvent eventoAntes;
    public UnityEvent eventoFinal;
    public UnityEvent eventoFinal2;

    private float speed = 0;

    private void Awake()
    {
        UI.SetActive(true);
        eventoAntes.Invoke();
    }

    private void Start()
    {
        StartCoroutine(ChangeSpeed(1, 0, 1f));
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
        eventoFinal.Invoke();
        UI.SetActive(false);
        //enabled = false;
        StartCoroutine(CountStart());
    }

    IEnumerator CountStart()
    {
        textoCount.transform.localScale = Vector3.one * 2;
        textoCount.SetText("3");
        yield return new WaitForSecondsRealtime(1.1f);
        textoCount.SetText("2");
        yield return new WaitForSecondsRealtime(1.1f);
        textoCount.SetText("1");
        yield return new WaitForSecondsRealtime(1.1f);
        textoCount.transform.localScale = Vector3.zero;
        eventoFinal2.Invoke();
        enabled = false;
        StopAllCoroutines();
    }

    private void Update()
    {
        fondo.color = new Color(0, 0, 0, speed);
        testo.color = new Color(1, 1, 1, speed);
        sliderFill.color = new Color(1, 0, 0, speed);
    }
}