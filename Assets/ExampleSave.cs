using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSave : MonoBehaviour
{
    public List<LineRenderer> letterExample;
    public string testLetterName = "O";
    public HandWrittenFont.LetterData letterDataExample;
    public HandWrittenFont.FontData fontDataExample;
    public Dictionary<string,HandWrittenFont.LetterData> letterDictionaryExample
                    = new Dictionary<string, HandWrittenFont.LetterData>();
    void Update()
    {
        letterDataExample = new HandWrittenFont.LetterData(letterExample);
        fontDataExample = new HandWrittenFont.FontData(letterDictionaryExample);
        letterDictionaryExample[testLetterName] = letterDataExample;
    }
}
