using UnityEngine;

public class Burnable : MonoBehaviour
{
    [Header("Burn Settings: ")]
    public bool isMultipleLensesEffected = false;
    public float burnTime;
    [SerializeField] private float currentBurnTime = 0f;


    [Header("Burn Color Settings: ")]
    public Color initialColor;
    public float initalColorBreach;
    [Space]
    public Color middleColor;
    public float middleColorBreach;
    [Space]
    public Color finalColor;

    private Renderer objectRenderer;
    private Material materialInstance;


    private void Awake()
    {
        //Get Reference To Current Object Render:
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            materialInstance = objectRenderer.material;
            materialInstance.color = initialColor;
        }
    }

    public void ApplyBurn(float deltaTime, int hitCount = 1)
    {
        float burnIncrement = deltaTime / burnTime;
        if (isMultipleLensesEffected) burnIncrement *= hitCount;

        currentBurnTime += burnIncrement;
        currentBurnTime = Mathf.Clamp01(currentBurnTime);

        UpdateMaterial();

        //Destroy After Threshold Is Met:
        if (currentBurnTime >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateMaterial()
    {
        if (objectRenderer == null) return;

        if (currentBurnTime < initalColorBreach)
        {
            float time = currentBurnTime / initalColorBreach;
            materialInstance.color = Color.Lerp(initialColor, middleColor, time);
        }
        else if (currentBurnTime < middleColorBreach)
        {
            float time = (currentBurnTime - initalColorBreach) / (middleColorBreach - initalColorBreach);
            materialInstance.color = Color.Lerp(middleColor, finalColor, time);
        }
        else
        {
            materialInstance.color = finalColor;
        }
    }
}
