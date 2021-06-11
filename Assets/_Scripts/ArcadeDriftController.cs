using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ArcadeDriftController : MonoBehaviour
{
    public static ArcadeDriftController _controller;

    public Rigidbody rb;
    public Transform llantaDel_root;
    public Transform llantasDel;
    public Transform llantasTras;
    public Animator anim;

    private float animManejo = 0;
    private float animManejo2 = 0;

    public Transform rootCam;
    public Transform model;
    public Vector3 offset = new Vector3(0, 0, 0);
    public Transform modelNormal;

    [Space]
    public float acceleration = 0;
    public float maxSpeed = 10f;
    public float reverse = 0;
    public float stearing = 0;
    public float gravity = 0;
    public LayerMask layerMask = 0;
    public AnimationCurve curbe;
    public AnimationCurve curbe2;

    private RaycastHit hitNear;

    [Space]
    public float currentTorque = 0;

    private float currentRotate = 0;
    private float rotate = 0;
    private float speed = 0;

    public bool drift = false;
    public float driftMin = 0;
    public float driftPower = 0;
    public ParticleSystem drift1;
    public ParticleSystem drift2;
    private ParticleSystem.EmissionModule emitParams1;
    private ParticleSystem.EmissionModule emitParams2;
    private float emitParam;

    [Space]
    public PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    private int driftDirection = 0;
    private float control = 0;
    private float powerControl = 0;

    [Space]
    public float Velocidad = 0;
    public float VelocidadMagnitude = 0;

    private Vector3 LocalVelocidad = new Vector3(0, 0, 0);

    private float axisH = 0;
    private float axisHold = 0;
    private float axisHold2 = 0;
    private float axisV = 0;

    private readonly string _Vertical = "Vertical";
    private readonly string _Horizontal = "Horizontal";
    private readonly string _Jump = "Jump";
    private readonly string _PlayerSteer = "PlayerSteer";

    private readonly Vector3 Vector3Zero = Vector3.zero;
    private readonly Vector3 vecDown = new Vector3(0, -1, 0);
    private readonly ForceMode f_Acceleration = ForceMode.Acceleration;

    private float fixedTime = 0;
    private bool _boost = false;

    private bool Play = false;

    public void PuedoJugar(bool on)
    {
        Play = on;
    }

    private void Awake()
    {
        _controller = this;

        emitParams1 = drift1.emission;
        emitParams2 = drift2.emission;
        emitParams1.rateOverTime = 0;
        emitParams1.rateOverTime = 0;

        postProfile = postVolume.profile;
    }

    private void Start()
    {
        rb.transform.SetParent(null);
        rb.centerOfMass = Vector3Zero;
    }

    private void Update()
    {
        fixedTime = Time.deltaTime;

        if (_boost)
        {
            currentTorque *= 300 * fixedTime;
            _boost = false;
        }

        postProfile.GetSetting<ChromaticAberration>().intensity.value = (Velocidad / 30f) + (currentTorque / 60f) - 1;

        emitParams1.rateOverTime = emitParam;
        emitParams2.rateOverTime = emitParam;
        emitParam = Mathf.Lerp(emitParam, 0, 0.5f);

        if (Play)
        {
            axisH = Input.GetAxis(_Horizontal);
            axisV = Input.GetAxis(_Vertical);
        }
        else
        {
            //axisH = 0;
            //axisV = 0;
        }

        //follow collider
        transform.position = rb.transform.position - offset;

        //acelerar
        if (axisV > 0)
        {
            speed = acceleration;
        }
        else if (axisV < 0)
        {
            speed = -reverse;
        }
        else
        {
            speed = 0;
        }

        //stearing
        if (axisH != 0)
        {
            if (Velocidad >= 0)
            {
                axisHold2 = axisH * curbe2.Evaluate(Velocidad.Remap(0, 15, 0, 1));
            }
            else
            {
                axisHold2 = axisH;
            }
            Steer(axisH > 0 ? 1 : -1, Mathf.Abs(axisHold2));
        }

        //Drift
        if (Input.GetButtonDown(_Jump) && !drift && axisH != 0 && Velocidad > driftMin)
        {
            drift = true;
            driftDirection = axisH > 0 ? 1 : -1;
        }

        if (drift)
        {
            control = (driftDirection == 1) ? ExtensionMethods.Remap(axisH, -1, 1, 0, 2) : ExtensionMethods.Remap(axisH, -1, 1, 2, 0);
            powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(axisH, -1, 1, .2f, 1) : ExtensionMethods.Remap(axisH, -1, 1, 1, 0.2f);
            Steer(driftDirection, control);
            driftPower += powerControl;
        }

        if (drift && Velocidad < driftMin || Input.GetButtonUp(_Jump))
        {
            drift = false;
            driftPower = 0;
            //boost si se quiere
        }

        if (Velocidad > 0.25f)
        {
            axisHold = axisH;
        }
        else if (Velocidad < 0)
        {
            axisHold = 0;
        }

        if (!drift)
        {
            model.localEulerAngles = Vector3.Lerp(model.localEulerAngles, new Vector3(0, 90 + (axisHold * 15), model.localEulerAngles.z), 0.02f);
            model.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(model.parent.localEulerAngles.y, 0, 0.1f), 0);

            rootCam.localPosition = new Vector3(Mathf.Lerp(rootCam.localPosition.x, 0, 0.02f), Mathf.Lerp(rootCam.localPosition.y, 0, 0.02f), Mathf.Lerp(rootCam.localPosition.z, 0, 0.02f));
        }
        else
        {
            control = (driftDirection == 1) ? ExtensionMethods.Remap(axisH, -1, 1, .5f, 2) : ExtensionMethods.Remap(axisH, -1, 1, 2, 0.5f);
            model.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(model.parent.localEulerAngles.y, (control * 15) * driftDirection, 0.01f), 0);

            rootCam.localPosition = new Vector3(Mathf.Lerp(rootCam.localPosition.x, (driftDirection == 1) ? 1 : -1, 0.01f), Mathf.Lerp(rootCam.localPosition.y, -1f, 0.01f), Mathf.Lerp(rootCam.localPosition.z, 3, 0.01f));
            emitParam = 80;
            //Debug.Log("drift");
        }

        //Valores motor
        currentTorque = Mathf.SmoothStep(currentTorque, speed, fixedTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, fixedTime * 4f); rotate = 0f;

        //llantas
        llantaDel_root.localEulerAngles = new Vector3(0, axisH * 15, 0);
        llantasDel.Rotate(Velocidad * 3f, 0, 0);
        llantasTras.Rotate(currentTorque / 3f, 0, 0);

        //drifts secundarios
        if (currentTorque - Velocidad >= 50)
        {
            emitParam = 50;
            //Debug.Log("drift diferencia");
        }
        if (Mathf.Abs(axisV) > 0 && VelocidadMagnitude < ((axisV > 0) ? 5 : 2) && Mathf.Abs(currentTorque) - VelocidadMagnitude <= 50)
        {
            emitParam = 10;
            //Debug.Log("drift arranques");
        }
        if (axisV < 0 && Velocidad > 0)
        {
            emitParam = 20;
            //Debug.Log("drift frenos");
        }

        //anim
        animManejo = Mathf.Lerp(animManejo, axisH, 0.25f);
        animManejo2 = animManejo.Remap(-1, 1, 0, 0.999f);
        anim.Play(_PlayerSteer, 0, animManejo2);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if (!drift)
        {
            rb.AddForce(-model.transform.right * currentTorque, f_Acceleration);
        }
        else
        {
            rb.AddForce(transform.forward * currentTorque, f_Acceleration);
        }

        //gravedad
        rb.AddForce(vecDown * gravity, f_Acceleration);

        //giro
        if (Velocidad > -1)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate * curbe.Evaluate(Mathf.Abs(currentTorque) / 60), 0), Time.deltaTime * 5f);
        }
        else
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + (-currentRotate) * curbe.Evaluate(Mathf.Abs(currentTorque) / 60), 0), Time.deltaTime * 5f);
        }

        Physics.Raycast(transform.position + (transform.up * .1f), vecDown, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        modelNormal.up = Vector3.Lerp(modelNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        modelNormal.Rotate(0, transform.eulerAngles.y, 0);

        //velocidades
        VelocidadMagnitude = rb.velocity.magnitude;
        LocalVelocidad = transform.InverseTransformDirection(rb.velocity);
        Velocidad = LocalVelocidad.z;
    }

    public void Boost()
    {
        //Debug.ClearDeveloperConsole();
        //drift = false;
        //currentTorque *= 200 * (fixedTime);
        //currentTorque *= 300 * Time.deltaTime;
        _boost = true;
        //aberacion cromatica
        //particulas
        driftPower = 0;
    }

    public void Aceite()
    {
        StartCoroutine(danoTemporal());
    }

    IEnumerator danoTemporal()
    {
        Play = false;
        axisH = Random.Range(0, 2);
        yield return new WaitForSecondsRealtime(1);
        Play = true;
    }

    public void Steer(int direction, float amount)
    {
        rotate = (stearing * direction) * amount;
    }
}