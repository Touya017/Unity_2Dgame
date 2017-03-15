using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFileManager : MonoBehaviour {

    InputField inputField;

    // InputFieldのコンポーネント取得と初期化
    void Start()
    {
        inputField = GetComponent<InputField>();
        InitInputField("", false);
    }

    // Log出力と確認用、その後初期化
    public void InputLogger()
    {
        string inputValue = inputField.text;
        Debug.Log(inputValue);
        InitInputField("", false);
    }

    // InputFieldの初期化処理
    void InitInputField(string initString, bool Forcus)
    {
        // テキスト内を初期化
        inputField.text = initString;
        // InputFieldをフォーカスする
        if (Forcus == true)
        {
            inputField.ActivateInputField();
        }
        else
        {
            inputField.DeactivateInputField();
        }
    }
}
