using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections.Generic;

public class manager : MonoBehaviour
{
    public uimanager uimanager;
    [SerializeField] private Text textttlog;
    [SerializeField] private GameObject go_LoginPannel;
    [SerializeField] private GameObject go_Chatting;
    [SerializeField] private InputField input_Id;
    [SerializeField] private InputField input_Pass;

    public FirebaseAuth auth; // Firebase ���� �ν��Ͻ�  
    public void Start()
    {
        //���� üũ 
        FirebaseInit();



        go_LoginPannel.SetActive(false); 
    }

    private void FirebaseInit()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Firebase dependencies are ready.");

                auth = FirebaseAuth.DefaultInstance; //�⺻�� �־����

                auth.StateChanged += OnAuthStateChanged; //���¹ٲ�� ���ϱ�

                DatabaseReference chatdata = FirebaseDatabase.DefaultInstance.GetReference("Chatting");
                chatdata.LimitToLast(1).ValueChanged += ReciveMessage;

            }
            else
            {
                Debug.LogError("Firebase dependencies could not be resolved: " + task.Exception);
            }
        });
    }

    

    // Firebase ���� ���� ���� �̺�Ʈ �ڵ鷯
    private void OnAuthStateChanged(object sender, EventArgs eventArgs)
    {
        FirebaseAuth auth = sender as FirebaseAuth;
        if(auth !=null)
        {
            this.auth = auth;
            if (auth.CurrentUser != null)
            {
                textttlog.text = this.auth.CurrentUser.UserId;
                Debug.Log("User is signed in: " + this.auth.CurrentUser.UserId);
                
            }
            else
            {
                textttlog.text = "Null";
                Debug.Log("No user is signed in.");
            }
        }
        else
        {
            textttlog.text = "Null";
            Debug.Log("auth is null");
        }

    }


    public void GuestLogin()
    {
        // �Խ�Ʈ �α��� ó��
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Guest login successful: " + auth.CurrentUser.UserId);
                
            }
            else
            {
                Debug.LogError("Guest login failed: " + task.Exception);
            }
        });
    }


    public void LogInTry()
    {
        if (auth == null)
        {
            Debug.LogError("Firebase not initialized yet.");
            return;
        }

        if (auth.CurrentUser != null)
        {
            Debug.Log("Already logged in as: " + auth.CurrentUser.UserId);
            return;
        }

        string email = input_Id.text;
        string password = input_Pass.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email or password is empty.");
            return;
        }

        Debug.Log("Trying login: " + email + " / pw length: " + password.Length);

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (var ex in task.Exception.Flatten().InnerExceptions)
                {
                    Debug.LogError("Login failed: " + ex.GetType().Name + " - " + ex.Message);
                }
                return;
            }
            Firebase.Auth.AuthResult authResult = task.Result;
            Firebase.Auth.FirebaseUser newUser = authResult.User;

            Debug.Log("Login successful! UID: " + newUser.UserId);
        });
    }

    public void LogInRegister()
    {
        if (auth == null)
        {
            Debug.LogError("Firebase not initialized yet.");
            return;
        }

        if (auth.CurrentUser != null)
        {
            Debug.Log("Already register in as: " + auth.CurrentUser.UserId);
            return;
        }

        string email = input_Id.text;
        string password = input_Pass.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email or password is empty.");
            return;
        }

        Debug.Log("Trying register: " + email + " / pw length: " + password.Length);

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("register canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (var ex in task.Exception.Flatten().InnerExceptions)
                {
                    Debug.LogError("register failed: " + ex.GetType().Name + " - " + ex.Message);
                }
                return;
            }
   
            Firebase.Auth.AuthResult authResult = task.Result;
            Firebase.Auth.FirebaseUser newUser = authResult.User;

            Debug.Log("register successful! UID: " + newUser.UserId);
        });
    }


    public void OpenChatting()
    {
        if (auth.CurrentUser == null)
        {
            Debug.Log("No user is currently logged in.");
            return;
        }
        go_Chatting.gameObject.SetActive(true);
        LoadMessage();
        

    }

    public void ReciveMessage(object sender, ValueChangedEventArgs arg)
    {
        LoadMessage();
    }

    public void LoadMessage()
    {
        if (uimanager == null)
        {
            Debug.LogError("uimanager is not assigned.");
            return;
        }
        DatabaseReference chatdata = FirebaseDatabase.DefaultInstance.GetReference("Chatting");

        chatdata.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                uimanager.LoadMessage(snapshot); 
            }
            else
            {
                Debug.LogError("Failed to load messages: " + task.Exception);
            }
        });
    }

    public void SendMessage(string Message)
    {
        Debug.Log("SendMessage called with message: " + Message);

        if (auth.CurrentUser == null)
        {
            Debug.LogError("No user is currently logged in. Cannot send message.");
            return;
        }

        DatabaseReference chatdata = FirebaseDatabase.DefaultInstance.GetReference("Chatting");



        string key = chatdata.Push().Key; // ���ο� �޽����� Ű ����
        string userId = auth.CurrentUser.UserId;
        Dictionary<string,string> message = new Dictionary<string,string>();

        message.Add("username", userId); // ���� �α����� ������� ID�� �߰�
        message.Add("message", Message); // �޽��� ���� �߰�

        
        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        updateMsg.Add(key, message); // �޽����� ������Ʈ�� ��ųʸ� ����  
        chatdata.UpdateChildrenAsync(updateMsg).ContinueWithOnMainThread(task=>
        {
            if(task.IsCompleted)
            {
                Debug.Log("Message sent successfully.");
                //LoadMessage(); // �޽��� ���� �� UI ������Ʈ
            }
            else
            {
                Debug.LogError("Failed to send message: " + task.Exception);
            }
        });
    }
    public void LogOutTry()
    {
        if (auth.CurrentUser == null)
        {
            Debug.Log("No user is currently logged in.");
            return;
        }
        // �α׾ƿ� ó��
        auth.SignOut();
        Debug.Log("User logged out successfully.");
        

    }


}
