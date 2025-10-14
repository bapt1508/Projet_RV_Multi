using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class errorManager : MonoBehaviour
{
    public TMP_Text errorText;
    
    public void SetErrorText(string error)
    {
        errorText.text = error;
    }
}
