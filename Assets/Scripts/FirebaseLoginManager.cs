using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.EventSystems;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("�α��� UI")]
    public InputField inputLoginID;     // ID �Է� �ʵ�
    public InputField inputLoginPW;     // ��й�ȣ �Է� �ʵ�
    public Button buttonLogin;          // �α��� ��ư
    public Text errorText;


    public Image secret_img;

    private DatabaseReference dbRef;    // Firebase Realtime Database ����
    private bool isFirebaseReady = false;

    void Start()
    {
        // Firebase ������ Ȯ�� �� �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // ���������� �ʱ�ȭ�Ǹ� DB ���� ����
                dbRef = FirebaseDatabase.GetInstance("https://my-unity-pj-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;
                isFirebaseReady = true;
                Debug.Log("Firebase �ʱ�ȭ �Ϸ�");
            }
            else
            {
                // ���� �� �α� ���
                Debug.LogError("Firebase �ʱ�ȭ ����: " + task.Result);
            }
        });

        // �α��� ��ư Ŭ�� �� OnClick_Login ����
        buttonLogin.onClick.AddListener(OnClick_Login);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

            if (current != null)
            {
                Selectable next = current.FindSelectableOnDown(); // �Ʒ��� Selectable ã��
                if (next != null)
                {
                    next.Select(); // ��Ŀ�� �ѱ��
                }
            }

            // Tab Ű�� �⺻ �Է� �̺�Ʈ ���� (InputField ���ο��� �� ���� ����)
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

    // �α��� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnClick_Login()
    {
        if (!isFirebaseReady)
        {
            Debug.LogWarning("Firebase�� ���� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        string id = inputLoginID.text.Trim();  // �Է��� ID ���� ����
        string pw = inputLoginPW.text.Trim();  // �Է��� PW ���� ����

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            // ID �Ǵ� ��й�ȣ�� ������� ���
            Debug.Log("ID�� ��й�ȣ�� �Է��ϼ���.");
            errorText.text = "ID�� ��й�ȣ�� �Է��ϼ���.";
            return;
        }

        // Firebase���� users/{id} ����� ������ ��������
        dbRef.Child("users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.Result.Exists)
            {
                // �Է��� ID�� �����ͺ��̽��� ���� ���
                Debug.Log("�������� �ʴ� ID�Դϴ�.");
                errorText.text = "�������� �ʴ� ID�Դϴ�.";
                return;
            }

            // ����� ��й�ȣ �� ��������
            string savedPW = task.Result.Child("password").Value.ToString();
            bool isLoggedIn = false;

            if (task.Result.Child("isLoggedIn").Exists)
            {
                isLoggedIn = (bool)task.Result.Child("isLoggedIn").Value;
            }

            if (savedPW != pw)
            {
                Debug.Log("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
                errorText.text = "��й�ȣ�� Ʋ�Ƚ��ϴ�.";
                return;
            }

            if (isLoggedIn)
            {
                Debug.Log("�̹� ���� ���� �����Դϴ�.");
                errorText.text = "�̹� ���� ���� �����Դϴ�.";
                return;
            }

            // �α��� ���� �� ���� ���� true�� ����
            dbRef.Child("users").Child(id).Child("isLoggedIn").SetValueAsync(true).ContinueWithOnMainThread(_ =>
            {
                Debug.Log("�α��� ����!");

                LoginSessionManager.Instance.loggedInID = id;
                LoginSessionManager.Instance.dbRef = dbRef;

                SceneManager.LoadScene("SampleScene");
            });
        });
    }
}
