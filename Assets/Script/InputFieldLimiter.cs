using UnityEngine;
using TMPro;

public class InputFieldLimiter : MonoBehaviour
{
    TMP_InputField inputField;
    [SerializeField] float minValue = 0;
    [SerializeField] float maxValue = 100;

    void Start()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();

       // inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnValueChanged);
    }

    void OnValueChanged(string value)
    {
        if (float.TryParse(value, out float number))
        {
            if (number < minValue || number > maxValue)
            {
                number = Mathf.Clamp(number, minValue, maxValue);
                inputField.text = number.ToString();
            }
        }
        else if (!string.IsNullOrEmpty(value))
        {
            inputField.text = minValue.ToString();
        }
    }

    void OnEndEdit(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            inputField.text = minValue.ToString();
        }
    }
}