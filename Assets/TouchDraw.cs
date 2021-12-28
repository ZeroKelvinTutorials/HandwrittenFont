using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDraw : MonoBehaviour
{
    Coroutine drawing;
    public static List<LineRenderer> drawnLineRenderers = new List<LineRenderer>();

    
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            if(EventSystem.current.currentSelectedGameObject == null){
                StartLine();
            }
        }
        if(Input.GetMouseButtonUp(0)){
            FinishLine();
        }
    }
    public void StartLine(){
        if(drawing!=null){
            StopCoroutine(drawing);
        }
        drawing = StartCoroutine(DrawLine());
    }
    public void FinishLine(){
        if(drawing!=null)
        StopCoroutine(drawing);
    }
    IEnumerator DrawLine(){
        GameObject newGameObject = Instantiate(Resources.Load("Line") as GameObject, new Vector3(0,0,0), Quaternion.identity);
        LineRenderer line =  newGameObject.GetComponent<LineRenderer>();
        drawnLineRenderers.Add(line);
        line.positionCount = 0;
        while(true){
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount-1, position);
            yield return null;
        }
    }
}
