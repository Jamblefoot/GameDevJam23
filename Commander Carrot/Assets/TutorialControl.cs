using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialControl : MonoBehaviour
{
    public static TutorialControl singleton;

    TMP_Text text;

    bool showingText;
    // Start is called before the first frame update
    void Start()
    {
        if(TutorialControl.singleton != null && TutorialControl.singleton != this)
            Destroy(gameObject);
        else TutorialControl.singleton = this;

        text = GetComponent<TMP_Text>();
    }

    public void SetTutorialText(string words, float displayTime)
    {
        text.text = words;
        if(showingText && displayTime > 0)
            StopAllCoroutines();
        StartCoroutine(ShowTextCo(displayTime));
    }

    IEnumerator ShowTextCo(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);

        text.text = "";
    }

    
}
