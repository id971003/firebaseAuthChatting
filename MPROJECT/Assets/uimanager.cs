using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
public class uimanager : MonoBehaviour
{
    public Transform trans_SlotParent;
    public CheckSlot slot;
    
    public manager manager;


    public InputField input;
    public void LoadMessage(DataSnapshot snapshot)
    {
        for (int i = 0; i < trans_SlotParent.childCount; i++)
        {
            trans_SlotParent.GetChild(i).gameObject.SetActive(false);
        }
            

        int b = 0;
        foreach (var a in snapshot.Children)
        {
            string name = a.Child("username").Value.ToString();
            string message = a.Child("message").Value.ToString();
            CheckSlot targetSlot = null;
            if (b > trans_SlotParent.childCount - 1)
            {
                Instantiate(slot, trans_SlotParent);
            }

            targetSlot = trans_SlotParent.GetChild(b).GetComponent<CheckSlot>();
            targetSlot.SetUp(name, message);
            targetSlot.gameObject.SetActive(true);
            b++;
        }
        trans_SlotParent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
    }
    public void SendMessage()
    {
        manager.SendMessage(input.text);
    }


}
