using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class FlipMirror : GemInteractions
{
    public bool isHitting;
    //[SerializeField] LightReflection lightSource;
    [SerializeField] GameObject mirror;
    [SerializeField] GameObject shadowPivot;
    Coroutine currentCoroutine;
    [SerializeField] Material unlit, lit;
    //public LightReflection lightReflection;
    [SerializeField] ShadowCaster shadowCaster;

    bool flip;
    [Space(15)]
    [Header("Testing")]
    [Tooltip("Set to true to start level with Gem 2 turned on.")]
    [SerializeField] bool test = false;

    public override void Start()
    {
        GetComponent<Renderer>().material = unlit;
    }

    public override void Update()
    {
        base.Update();

        //LightReflection light = GameManager.Instance.Player.gameObject.GetComponentInChildren<LightReflection>();
        isHitting = LightTool() != null && LightTool().gemHit;

        if ((isHitting && !flip) || (test && !flip))
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(Flip(Quaternion.Euler(0f, 180f, 0f)));
        }
        else if (!isHitting && flip && !test)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(Flip(Quaternion.Euler(0f, 0f, 0f)));
        }

    }

    IEnumerator Flip(Quaternion target)
    {
        flip = !flip;
        shadowCaster.isChecking = false;
        if (flip) if (GetComponent<Renderer>().material != lit) GetComponent<Renderer>().material = lit;
        if (!flip) if (GetComponent<Renderer>().material != unlit) GetComponent<Renderer>().material = unlit;

        Quaternion startRotation = mirror.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1.0f)
        {
            mirror.transform.rotation = Quaternion.Lerp(startRotation, target, elapsedTime);
            shadowPivot.transform.rotation = Quaternion.Lerp(startRotation, target, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mirror.transform.rotation = target;
        shadowPivot.transform.rotation = target;
        shadowCaster.isChecking = flip;
        currentCoroutine = null;

    }
}
