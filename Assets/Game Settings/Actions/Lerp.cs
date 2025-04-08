using UnityEngine;

public class Lerp : MonoBehaviour
{
    public float value;
    public float maxValue;

    [Range(0, 1)]
    public float step;

    // Update is called once per frame
    void Update()
    {
        value = Mathf.Lerp(value, maxValue, step * Time.deltaTime);
    }
}
