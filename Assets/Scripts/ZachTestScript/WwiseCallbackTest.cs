using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseCallbackTest : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event scribbleEvent;

    bool isScribbling = true;
    // Start is called before the first frame update
    void Start()
    {
        scribbleEvent.Post(gameObject, (uint)AkCallbackType.AK_EndOfEvent, ScribbleCallback);
    }

    void ScribbleCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if(isScribbling == true)
        {
            scribbleEvent.Post(gameObject, (uint)AkCallbackType.AK_EndOfEvent, ScribbleCallback);

        }
    }



/*    private void Update()
    {
        float x = 100.0f;
        while (x >= 0.0f)
        {
            x -= 1.0f * Time.deltaTime;
            isScribbling = false;
        }
    }*/
    
}
