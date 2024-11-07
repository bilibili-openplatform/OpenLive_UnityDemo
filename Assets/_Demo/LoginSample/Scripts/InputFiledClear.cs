using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFiledClear : MonoBehaviour
{
    public Button ClearButton;
    private InputField inputField => GetComponent<InputField>();
    protected virtual void Start()
    {

        ClearButton.onClick.AddListener(ClearText);
    }

    protected virtual void ClearText()
    {
        inputField.text = string.Empty;
    }
}
