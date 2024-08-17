using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Create a dialog with yes or no options.
/// </summary>
public class YesNoDialog : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI noButtonText;
    public TextMeshProUGUI yesButtonText;

    Action yesCallback;
    Action noCallback;

    static YesNoDialog _prefab;
    static YesNoDialog Prefab
    {
        get
        {
            if(_prefab == null)
                _prefab = Resources.Load<YesNoDialog>("UI/YesNoDialog");

            return _prefab;
        }
    }

    public static void CreateDialog(string dialogText, Action yesCallback, Action noCallback)
    {
        CreateDialog(dialogText, "Yes", "No", yesCallback, noCallback);
    }

    public static void CreateDialog(string dialogText, string yesButtonText, string noButtonText, Action yesCallback, Action noCallback)
    {
        YesNoDialog newDialog = Instantiate(Prefab, null);

        newDialog.dialogText.text = dialogText;
        newDialog.yesButtonText.text = yesButtonText;
        newDialog.noButtonText.text = noButtonText;

        newDialog.yesCallback = yesCallback;
        newDialog.noCallback = noCallback;
    }

    public void YesClicked()
    {
        if (yesCallback != null)
            yesCallback();

        Destroy(gameObject);
    }

    public void NoClicked()
    {
        if (noCallback != null)
            noCallback();

        Destroy(gameObject);
    }
}
