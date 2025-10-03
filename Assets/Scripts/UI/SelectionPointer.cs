using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPointer : MonoBehaviour
{
    [SerializeField] private RectTransform[] options;
    private RectTransform rect; //transform akorat u UI objektù
    private int currentPosition;

    //Audio
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //meneni pozice pointeru
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            ChangePosition(-1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            ChangePosition(1);

        //interakce s buttony
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            Interact();
    }

    private void ChangePosition(int _change)
    {
        currentPosition += _change;

        //if(_change != 0)
            //SoundManager.instance.PlaySound(changeSound);

        if (currentPosition < 0)
        {
            currentPosition = options.Length - 1;
        }
        else if (currentPosition > options.Length -1)
            currentPosition = 0;


        //priradi honodtu y vybrane moznosti na selection pointer
        rect.position = new Vector3(rect.position.x, options[currentPosition].position.y, 0);
    }
    
    private void Interact()
    {
        //SoundManager.instance.PlaySound(clickSound);

        //Acces the button component
        options[currentPosition].GetComponent<Button>().onClick.Invoke();
    }
}
