using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// pulses with a blue-green color
public class Glow : MonoBehaviour
{
    Material myMaterial;

    [SerializeField] bool glowIncreasing = true;
    [SerializeField] float glowFrequencyInSeconds = 2;
    [SerializeField] float startingGlow = 0.1f; // must be >= minGlow
    [SerializeField] float minGlow = 0.1f;  //must be > 0
    [SerializeField] float maxGlow = 0.5f;  // must be < 1
    [SerializeField] float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        glowIncreasing = true;
        myMaterial.SetColor(
            "_EmissionColor",
            new Color(0, startingGlow, startingGlow, 1)
        );
        StartCoroutine(UpdateGlow());
    }

    // glow brightens, then softens, then pauses (optional) and repeats
    private IEnumerator UpdateGlow()
    {
        while (true)
        {
            var emissionColor = myMaterial.GetColor("_EmissionColor");
            var glowDelta = Time.deltaTime / (glowFrequencyInSeconds / (maxGlow - minGlow));

            // try to increase glow
            if (glowIncreasing)
            {
                if (emissionColor.g > maxGlow)
                {
                    glowIncreasing = false;
                }
                else
                {
                    myMaterial.SetColor(
                        "_EmissionColor",
                        new Color(0, emissionColor.g + glowDelta, emissionColor.b + glowDelta, emissionColor.a)
                    );
                }
            }
            // try to decrease glow
            else
            {
                if (emissionColor.g < minGlow)
                {
                    if (delay > 0)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                    glowIncreasing = true;
                }
                else
                {
                    myMaterial.SetColor(
                        "_EmissionColor",
                        new Color(0, emissionColor.g - glowDelta, emissionColor.b - glowDelta, emissionColor.a)
                    );
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
