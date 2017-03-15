using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectFileCopy : MonoBehaviour {

    UpdateFileManager Upfile;
    ChangeFileInputManager Changefile;
    string UpdateFileName;
    string ChangeFileName;

	// Use this for initialization
	void Start () {
        Upfile = GetComponent<UpdateFileManager>();
        Changefile = GetComponent<ChangeFileInputManager>();
    }
	
	public void Update () {

        if (Upfile == null && Changefile == null)
        {
            return;
        }
        else
        {
            // InputFieldに入力されたファイルパスの文字列を格納
            UpdateFileName = Upfile.GetComponentInParent<InputField>().text;
            ChangeFileName = Changefile.GetComponentInParent<InputField>().text;
            Debug.Log(UpdateFileName);
            Debug.Log(ChangeFileName);

            //第3引数にtruを指定すると、コピー先が存在している場合は上書きする
            //上書きするファイルが読み取り専用などで上書きできない場合は、
            //  UnauthorizedAccessExceptionが発生
            System.IO.File.Copy(@"UpdatefileName", @"ChangeFileName", true);
        }
        return;
    }
}
