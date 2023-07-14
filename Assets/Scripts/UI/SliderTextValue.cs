using TMPro;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderTextValue : MonoBehaviour
{
    [Tooltip("The text component to be contain the sliders formatted value.")]
    [SerializeField] private TextMeshProUGUI text;

    [Tooltip("Format to be used on the value passed by the slider. Leave empty for no formatting.")]
    [SerializeField] private string format = "P0";

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(SetText);
    }

    private void Start()
    {
        SetText(slider.value);
    }

    public void SetText(float value)
    {
        if (text == null) { return; }

        text.SetText(value.ToString(format));
    }
}
