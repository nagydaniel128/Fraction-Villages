using System.Collections;
using UnityEngine;
using TMPro;

public class FpsIndicator : MonoBehaviour
{
    TextMeshProUGUI text;
    float current = 0;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        StartCoroutine(ShowFps());

        Application.targetFrameRate = 9999;
    }
    IEnumerator ShowFps()
    {
        text.text = current.ToString();

        yield return new WaitForSeconds(1);

        StartCoroutine(ShowFps());
    }

    private void Update()
    {
        current = (int)(1f / Time.unscaledDeltaTime);
    }
}
