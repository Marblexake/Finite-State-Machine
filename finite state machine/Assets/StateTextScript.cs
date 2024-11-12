using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateTextScript : MonoBehaviour
{

    private string CurrentStateText;
    private TMP_Text textObject;
    // Start is called before the first frame update
    void Start()
    {
        //transform.Translate(Vector3.up * 3);
        textObject = GetComponent<TMP_Text>();
        UpdateStateText("Default Value!!!");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180.0f, 0);
    }

    public void UpdateStateText(string currentState)
    {
        textObject.SetText(currentState);
    }
}
