using UnityEngine;
using Firebase.Database;

public class LoginSessionManager : MonoBehaviour
{
    public static LoginSessionManager Instance;

    [Header("���� ����")]
    public string loggedInID;
    public DatabaseReference dbRef;

    void Awake()
    {
        // �̱��� ���� & �� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    void OnApplicationQuit()
    {
        // ���� ���� �� �α��� ���� false ó��
        if (!string.IsNullOrEmpty(loggedInID) && dbRef != null)
        {
            Debug.Log($"���� ���� - �α��� ���� �ʱ�ȭ: {loggedInID}");
            dbRef.Child("users").Child(loggedInID).Child("isLoggedIn").SetValueAsync(false);
        }
    }
}
