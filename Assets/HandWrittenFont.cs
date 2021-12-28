using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Serializable
using UnityEngine.UI; //Text
using System.IO; //File


public class HandWrittenFont : MonoBehaviour
{
    #region CustomClasses
    [Serializable]
    public class Line
    {
        public Vector3[] line;
        public Line(Vector3[] newLine)
        {
            line = newLine;
        }
    }
    [Serializable]
    public class LetterData
    {
        public List<Line> lines;
        public LetterData(List<LineRenderer> lineRenderers)
        {
            lines = new List<Line>();
            foreach(LineRenderer lineRenderer in lineRenderers)
            {
                Vector3[] points = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(points);
                lines.Add(new Line(points));
            }
        }
    }
    [Serializable]
    public class FontData
    {		
    	public List <string> letterKeys;
        public List<LetterData> letterDataList;
        
        public FontData(Dictionary <string, LetterData> characterDictionary)
        {
            letterDataList = new List<LetterData>();
            letterKeys = new List<string>();
            foreach(KeyValuePair<string, LetterData> entry in characterDictionary)
            {
            	letterKeys.Add(entry.Key);
                letterDataList.Add(entry.Value);
            }
        }
    }
    #endregion

	#region fontDataVariables
    string[] letters = {"A","B","C","D","E","F","G","H","I","J",
                        "K","L","M","N","O","P","Q","R","S","T",
                        "U","V","W","X","Y","Z","a","b","c","d",
                        "e","f","g","h","i","j","k","l","m","n",
                        "o","p","q","r","s","t","u","v","w","x",
                        "y","z"};
    Dictionary<string,LetterData> letterDictionary = new Dictionary<string,LetterData>();

    public FontData fontData;
    #endregion

    #region CreateFontVariables
    int currentLetterIndex;
    public Text letterText;
    #endregion

    #region WriteTextVariables
    bool drawingLetter;
    #endregion

    #region Initialization
    void Start()
    {
        if(HasFont())
        {
            LoadFont();
        }
        else
        {
            CreateFont();
        }
        
    }
    void Update()
    {
        fontData = new FontData(letterDictionary);
    }
    bool HasFont(){
        if(PlayerPrefs.HasKey("Saved")) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region CreateFont
    void CreateFont()
    {
        EnableChildren(true);
        currentLetterIndex = 0;
        DisplayCurrentLetter(letters[currentLetterIndex]);
    }

    void DisplayCurrentLetter(string letter)
    {
        letterText.text = letter.ToString();
    }

    public void StoreLetter()
    {
        string currentLetter = letters[currentLetterIndex];
        letterDictionary[currentLetter] = new LetterData(TouchDraw.drawnLineRenderers);

        ClearLetter();

        GoToNextOrSaveFont();
    }

    void ClearLetter()
    {
        foreach(LineRenderer lineRenderer in TouchDraw.drawnLineRenderers)
        {
            Destroy(lineRenderer.gameObject);
        }
        TouchDraw.drawnLineRenderers.Clear();
    }

    void GoToNextOrSaveFont()
    {
        currentLetterIndex++;
        if(currentLetterIndex<letters.Length)
        {
            DisplayCurrentLetter(letters[currentLetterIndex]);
        }
        else
        {
            SaveFont();
        }
    }
    #endregion

    #region WriteText
    public void WriteTextWrapper(string stringToWrite)
    {
        StartCoroutine(WriteText(stringToWrite, new Vector3(-5,0,0)));
    }
    public void WriteTextWrapper(string stringToWrite, Vector3 startingPosition)
    {
    	StartCoroutine(WriteText(stringToWrite,startingPosition));
    }
    IEnumerator WriteText(string stringToWrite, Vector3 startingPosition)
    {
        int i = 0;
        while(i<stringToWrite.Length)
        {
            string letter = stringToWrite[i].ToString();

            if(letter != " ")
            {
	            drawingLetter = true;
	            StartCoroutine(AnimateLetter(letterDictionary[letter], startingPosition+ Vector3.right*i));
            }


            while(drawingLetter)
            {
                yield return null;
            }

            i++;

            yield return null;
        }
        yield return null;

    }
    
    IEnumerator AnimateLetter(LetterData letterData, Vector3 position)
    {
        foreach(Line line in letterData.lines)
        {
            GameObject newLineGameObject = Instantiate(Resources.Load("Line") as GameObject, position, Quaternion.identity);
            LineRenderer lineRenderer = newLineGameObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            int pointIndex = 0;
            while(pointIndex<line.line.Length)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(pointIndex, line.line[pointIndex]);
                pointIndex++;
                yield return null;
            }
            yield return null;
        }
        // yield return new WaitForSeconds(.1f);
        drawingLetter = false;
        yield return null;
    }
    #endregion

    #region Save
    void SaveFont()
    {
        string jsonString = JsonUtility.ToJson(new FontData(letterDictionary));
        string fileName =  Application.persistentDataPath + "/HandWrittenFont.txt";
        File.WriteAllText(fileName, jsonString);
        PlayerPrefs.SetInt("Saved", 1);
        //PlayerPrefs.SetFloat("Saved", .3);
        //PlayerPrefs.SetString("Saved", "sure);
    }
    #endregion 

    #region Load
    void LoadFont()
    {
        EnableChildren(false);

        string completeFileName = Application.persistentDataPath + "/HandWrittenFont.txt";
        if(File.Exists(completeFileName))
            JsonToData(File.ReadAllText(completeFileName));

        else
            Debug.Log("Need a valid name");
    }

    void JsonToData(string jsonString)
    {
        FontData loadedFont = JsonUtility.FromJson<FontData>(jsonString);
        int i = 0;
        letters = loadedFont.letterKeys.ToArray();
        foreach(LetterData letter in loadedFont.letterDataList)
        {
            letterDictionary[letters[i]] = letter;
            i++;
        }
    }
    #endregion
    
    #region HelperFunctions
    void EnableChildren(bool isActive)
    {
        foreach(Transform child in this.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }
    #endregion
}
