using UnityEngine;
using Firebase.Database;

public class LoginSessionManager : MonoBehaviour
{
    public static LoginSessionManager Instance;

    [Header("유저 정보")]
    public string loggedInID;
    public DatabaseReference dbRef;

    void Awake()
    {
        // 싱글톤 생성 & 씬 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    void OnApplicationQuit()
    {
        // 게임 종료 시 로그인 상태 false 처리
        if (!string.IsNullOrEmpty(loggedInID) && dbRef != null)
        {
            Debug.Log($"게임 종료 - 로그인 상태 초기화: {loggedInID}");
            dbRef.Child("users").Child(loggedInID).Child("isLoggedIn").SetValueAsync(false);
        }
    }
}
