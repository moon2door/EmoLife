using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.EventSystems;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("로그인 UI")]
    public InputField inputLoginID;     // ID 입력 필드
    public InputField inputLoginPW;     // 비밀번호 입력 필드
    public Button buttonLogin;          // 로그인 버튼
    public Text errorText;


    public Image secret_img;

    private DatabaseReference dbRef;    // Firebase Realtime Database 참조
    private bool isFirebaseReady = false;

    void Start()
    {
        // Firebase 의존성 확인 및 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // 정상적으로 초기화되면 DB 참조 설정
                dbRef = FirebaseDatabase.GetInstance("https://my-unity-pj-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
                isFirebaseReady = true;
                Debug.Log("Firebase 초기화 완료");
            }
            else
            {
                // 실패 시 로그 출력
                Debug.LogError("Firebase 초기화 실패: " + task.Result);
            }
        });

        // 로그인 버튼 클릭 시 OnClick_Login 실행
        buttonLogin.onClick.AddListener(OnClick_Login);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

            if (current != null)
            {
                Selectable next = current.FindSelectableOnDown(); // 아래쪽 Selectable 찾기
                if (next != null)
                {
                    next.Select(); // 포커스 넘기기
                }
            }

            // Tab 키의 기본 입력 이벤트 차단 (InputField 내부에서 탭 삽입 방지)
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (Input.GetKey(KeyCode.F11))
        {
            secret_img.gameObject.SetActive(true);
        }
        else
        {
            secret_img.gameObject.SetActive(false);
        }
    }

    // 로그인 버튼 클릭 시 호출되는 함수
    public void OnClick_Login()
    {
        if (!isFirebaseReady)
        {
            Debug.LogWarning("Firebase가 아직 초기화되지 않았습니다.");
            return;
        }

        string id = inputLoginID.text.Trim();  // 입력한 ID 공백 제거
        string pw = inputLoginPW.text.Trim();  // 입력한 PW 공백 제거

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            // ID 또는 비밀번호가 비어있을 경우
            Debug.Log("ID와 비밀번호를 입력하세요.");
            errorText.text = "ID와 비밀번호를 입력하세요.";
            return;
        }

        // Firebase에서 users/{id} 경로의 데이터 가져오기
        dbRef.Child("users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.Result.Exists)
            {
                // 입력한 ID가 데이터베이스에 없을 경우
                Debug.Log("존재하지 않는 ID입니다.");
                errorText.text = "존재하지 않는 ID입니다.";
                return;
            }

            // 저장된 비밀번호 값 가져오기
            string savedPW = task.Result.Child("password").Value.ToString();
            bool isLoggedIn = false;

            if (task.Result.Child("isLoggedIn").Exists)
            {
                isLoggedIn = (bool)task.Result.Child("isLoggedIn").Value;
            }

            if (savedPW != pw)
            {
                Debug.Log("비밀번호가 틀렸습니다.");
                errorText.text = "비밀번호가 틀렸습니다.";
                return;
            }

            if (isLoggedIn)
            {
                Debug.Log("이미 접속 중인 계정입니다.");
                errorText.text = "이미 접속 중인 계정입니다.";
                return;
            }

            // 로그인 성공 → 접속 상태 true로 변경
            dbRef.Child("users").Child(id).Child("isLoggedIn").SetValueAsync(true).ContinueWithOnMainThread(_ =>
            {
                Debug.Log("로그인 성공!");

                LoginSessionManager.Instance.loggedInID = id;
                LoginSessionManager.Instance.dbRef = dbRef;

                SceneManager.LoadScene("SampleScene");
            });
        });
    }
}
