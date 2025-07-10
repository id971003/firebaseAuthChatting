using UnityEngine;
using UnityEngine.UI;

public class CheckSlot : MonoBehaviour
{
    [SerializeField] private Text text_name;
    [SerializeField] private Text text_Mes;
    public void SetUp(string a, string b)
    {
        text_name.text = a;
        text_Mes.text = b;

    }
}
