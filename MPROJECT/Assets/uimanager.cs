using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
public class uimanager : MonoBehaviour
{
    public List<CheckSlot> checkSlots = new List<CheckSlot>(); 
    public manager manager;


    public InputField input;
    public void LoadMessage(DataSnapshot snapshot)
    {
        foreach(var a in checkSlots)
        {
            a.gameObject.SetActive(false);
        }

        int b = 0;
        foreach (var a in snapshot.Children)
        {
            string name = a.Child("username").Value.ToString();
            string message = a.Child("message").Value.ToString();

            checkSlots[b].SetUp(name, message);
            checkSlots[b].gameObject.SetActive(true);
            b++;
        }
    }
    public void SendMessage()
    {
        Debug.Log(input.text);
        manager.SendMessage(input.text);
    }


}
