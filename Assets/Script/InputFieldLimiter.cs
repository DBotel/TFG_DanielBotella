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

        // Asigna el método OnValueChanged al evento del TMP_InputField
       // inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnValueChanged);
    }

    void OnValueChanged(string value)
    {
        // Intenta parsear el valor ingresado
        if (float.TryParse(value, out float number))
        {
            // Si el número está fuera del rango, lo limita
            if (number < minValue || number > maxValue)
            {
                number = Mathf.Clamp(number, minValue, maxValue);
                inputField.text = number.ToString();
            }
        }
        else if (!string.IsNullOrEmpty(value))
        {
            // Si el valor no es un número, lo resetea a un valor dentro del rango
            inputField.text = minValue.ToString();
        }
    }

    void OnEndEdit(string value)
    {
        // Si el campo queda vacío al terminar, resetea al valor mínimo
        if (string.IsNullOrEmpty(value))
        {
            inputField.text = minValue.ToString();
        }
    }
}