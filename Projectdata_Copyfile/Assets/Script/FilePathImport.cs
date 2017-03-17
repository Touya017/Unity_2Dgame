using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FilePathImport : MonoBehaviour {

    static void Main(string[] args)
    {
        string filePath = @"C:\Users\tester\Desktop\ProjectSettings\ProjectSettings.txt";
        Encoding fileEnc = Encoding.GetEncoding("shift_jis");

        // テキスト内容を全て格納
        string projectFilepath = File.ReadAllText(filePath, fileEnc);

        // テキスト内容を1行ずつ格納
        string[] projecFilepathlines = File.ReadAllLines(filePath, fileEnc);

        // 文字列表示
        foreach(string i in projecFilepathlines)
        {
            Debug.Log("{i}\n");
        }
    }
}
