using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Triggers : MonoBehaviour
{
    private ArcadeDriftController controller;
    private GameManager manager;

    public TextMeshProUGUI textCanvas;
    private Transform tTextCanvas;

    private float elapsed = 0;
    private float speed = 0;

    private void Awake()
    {
        tTextCanvas = textCanvas.transform;
    }

    private void Start()
    {
        controller = ArcadeDriftController._controller;
        manager = GameManager._gameManager;
    }

    private void Update()
    {
        if (manager.Jugando)
        {
            tTextCanvas.localScale = new Vector3(speed, speed, speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turbo"))
        {
            controller.Boost();
            textCanvas.SetText("¡ BOOST !");
            StopAllCoroutines();
            StartCoroutine(ChangeSpeed1(speed, 1, 0.5f));
        }
        if (other.CompareTag("Aceite"))
        {
            controller.Aceite();
            textCanvas.SetText("¡ OIL !");
            StopAllCoroutines();
            StartCoroutine(ChangeSpeed1(speed, 1, 0.5f));
        }
        if (other.CompareTag("Meta"))
        {
            manager.PasoMeta();
        }
        if (other.CompareTag("Tiempo"))
        {
            manager.SubirTiempo();
            other.gameObject.SetActive(false);
            textCanvas.SetText("¡ +10 SEC !");
            StopAllCoroutines();
            StartCoroutine(ChangeSpeed1(speed, 1, 0.5f));
        }
    }

    IEnumerator ChangeSpeed1(float v_start, float v_end, float duration)
    {
        elapsed = 0.0f;
        while (elapsed < duration)
        {
            speed = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        speed = v_end;
        StartCoroutine(ChangeSpeed2(1, 0, 1));
    }
    IEnumerator ChangeSpeed2(float v_start, float v_end, float duration)
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