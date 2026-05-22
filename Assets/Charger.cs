using UnityEngine;
using MoreMountains.Tools;

public class Charger : MonoBehaviour
{
    [Header("Charging Settings:")]
    public float chargeTime = 2f;
    public string gameEventName = "ChargerFull";
    public enum ActivationMode { ResetWhenNotHit, PersistAfterHit }
    public ActivationMode activationMode = ActivationMode.ResetWhenNotHit;

    [Header("State Info:")]
    public bool isFullyCharged = false;
    public float currentCharge = 0f;
    public int hitsThisFrame = 0;

    [Header("Visuals: ")]
    public Material defaultMaterial;
    public Color unchargedColor = Color.grey;
    public Color chargedColor = Color.white;
    [SerializeField] private string colorPropertyName = "_BaseColor";

    [Header("Glow Settings: ")]
    [SerializeField] private float unchargedGlow = 0f;
    [SerializeField] private float chargedGlow = 1f;
    [SerializeField] private string emissionPropertyName = "_EmissionColor";

    [Header("Outline Settings: ")]
    [SerializeField] private Outline outline;
    [SerializeField] private Color chargedOutlineColor = Color.white;
    [SerializeField] private Color unchargedOutlineColor = Color.white;
    [SerializeField] private float chargedOutlineWidth = 2f;
    [SerializeField] private float unchargedOutlineWidth = 0f;
    [SerializeField] private float lerpTime = 0.5f;

    private Material runtimeMaterial;
    private Renderer chargerRenderer;

    private void Start()
    {
        chargerRenderer = GetComponent<Renderer>();

        if (defaultMaterial != null)
        {
            runtimeMaterial = new Material(defaultMaterial);
            chargerRenderer.material = runtimeMaterial;

            Color initialColor = isFullyCharged ? chargedColor : unchargedColor;
            float initialGlow = isFullyCharged ? chargedGlow : unchargedGlow;

            runtimeMaterial.SetColor(colorPropertyName, initialColor);

            if (runtimeMaterial.HasProperty(emissionPropertyName))
            {
                runtimeMaterial.EnableKeyword("_EMISSION");
                runtimeMaterial.SetColor(emissionPropertyName, initialColor * initialGlow);
            }
        }
    }

    private void Update()
    {
        UpdateCharging();
        UpdateVisuals();
    }

    private void UpdateCharging()
    {
        if (hitsThisFrame > 0)
        {
            currentCharge += Time.deltaTime;
            hitsThisFrame = 0;

            if (currentCharge >= chargeTime)
            {
                if (!isFullyCharged)
                {
                    isFullyCharged = true;
                    MMGameEvent.Trigger(gameEventName);
                }
                currentCharge = chargeTime;
            }
        }
        else if (!isFullyCharged && activationMode == ActivationMode.ResetWhenNotHit)
        {
            currentCharge = 0f;
        }
    }

    private void UpdateVisuals()
    {
        float chargeRatio = currentCharge / chargeTime;

        Color targetMaterialColor = Color.Lerp(unchargedColor, chargedColor, chargeRatio);
        float targetGlow = Mathf.Lerp(unchargedGlow, chargedGlow, chargeRatio);
        Color targetOutlineColor = Color.Lerp(unchargedOutlineColor, chargedOutlineColor, chargeRatio);
        float targetOutlineWidth = Mathf.Lerp(unchargedOutlineWidth, chargedOutlineWidth, chargeRatio);

        // Lerp Material
        if (runtimeMaterial != null)
        {
            float t = (lerpTime > 0) ? Time.deltaTime / lerpTime : 1f;

            Color currentColor = runtimeMaterial.GetColor(colorPropertyName);
            Color nextColor = Color.Lerp(currentColor, targetMaterialColor, t);
            runtimeMaterial.SetColor(colorPropertyName, nextColor);

            if (runtimeMaterial.HasProperty(emissionPropertyName))
            {
                Color currentEmission = runtimeMaterial.GetColor(emissionPropertyName);
                Color targetEmissionColor = targetMaterialColor * targetGlow;
                Color nextEmissionColor = Color.Lerp(currentEmission, targetEmissionColor, t);
                runtimeMaterial.SetColor(emissionPropertyName, nextEmissionColor);
            }
        }

        // Lerp Outline
        if (outline != null)
        {
            float t = (lerpTime > 0) ? Time.deltaTime / lerpTime : 1f;
            outline.OutlineColor = Color.Lerp(outline.OutlineColor, targetOutlineColor, t);
            outline.OutlineWidth = Mathf.Lerp(outline.OutlineWidth, targetOutlineWidth, t);
        }
    }
}
