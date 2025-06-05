using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("움직임 관련")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    public float gravity = -9f;
    private float verticalVelocity = 0f;

    [Header("참조")]
    public Transform cameraPivot;
    public Animator myAnim;
    public Player_Door player_Door;

    PhotonView myPV;

    [Header("UI")]
    public InputField chating;
    public Text chating_text;
    public Image dance_Img;
    public Image sing_Image;
    public Image hi_Image;

    public Image angry_Image;
    public Image clap_Image;
    public Image surprisec_Image;
    public Image kick_Image;
    public Image swim_Image;

    [Header("Skill")]
    public Image chatList;

    private CharacterController controller;
    private float yaw = 0f;
    private Coroutine chatCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        myPV = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;

        if (!myPV.IsMine)
        {
            // 내 것이 아니면 카메라를 비활성화
            Transform cam = transform.Find("Main Camera");
            if (cam != null)
                cam.gameObject.SetActive(false);
        }

        chating.characterLimit = 30; // 최대 30글자 제한
        chating.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!myPV.IsMine)
            return;

        if (chating.gameObject.activeSelf && chating.isFocused)
            return;

        if (player_Door.isHouse)
        {
            Vector3 lookDir = Vector3.left;
            lookDir.y = 0; // 수직 회전 제거
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        if (player_Door.isHouse)
            return;

        if (myAnim.GetBool("Kick"))
        {
            transform.Rotate(0f, 360f * Time.deltaTime, 0f);
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        // 카메라 피벗 회전
        if (cameraPivot != null)
            cameraPivot.rotation = Quaternion.Euler(0, yaw, 0);

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        move.y = verticalVelocity;

        Run();

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!chating.gameObject.activeSelf)
            {
                chating.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(chating.gameObject); // 포커스 부여
                chating.ActivateInputField(); // 입력 시작
            }
            else
            {
                chating.DeactivateInputField(); // 입력 종료
                chating.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null); // 포커스 해제

                if (string.IsNullOrWhiteSpace(chating.text))
                    return;

                string tempText = chating.text; // 미리 저장

                chating.text = "";

                if (tempText == "/춤")
                {
                    myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
                    myPV.RPC("RPC_Dance", RpcTarget.All, myPV.ViewID);
                }
                else if (tempText == "/노래")
                {
                    myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
                    myPV.RPC("RPC_Sing", RpcTarget.All, myPV.ViewID);
                }
                else if (tempText == "ㅎㅇ" || tempText == "하이" || tempText == "안녕하세요" || tempText == "gd")
                {
                    myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
                    myPV.RPC("RPC_Greeting", RpcTarget.All, myPV.ViewID);
                }
                else
                {
                    myPV.RPC("RPC_ChatText", RpcTarget.All, tempText, myPV.ViewID);
                }
            }
        }

        Click_Keyboard();
    }

    void Run()
    {
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            myAnim.SetBool("D Run", false);
            myAnim.SetBool("A Run", false);
            myAnim.SetBool("W Run", false);
            myAnim.SetBool("S Run", false);
            return;
        }

        myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);

        sing_Image.gameObject.SetActive(false);
        dance_Img.gameObject.SetActive(false);
        hi_Image.gameObject.SetActive(false);


        angry_Image.gameObject.SetActive(false);
        clap_Image.gameObject.SetActive(false);
        surprisec_Image.gameObject.SetActive(false);
        kick_Image.gameObject.SetActive(false);
        swim_Image.gameObject.SetActive(false);

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && Input.GetKey(KeyCode.A))
        {
            myAnim.SetBool("A Run", true);
            myAnim.SetBool("D Run", false);
            myAnim.SetBool("W Run", false);
            myAnim.SetBool("S Run", false);
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && Input.GetKey(KeyCode.D))
        {
            myAnim.SetBool("D Run", true);
            myAnim.SetBool("A Run", false);
            myAnim.SetBool("W Run", false);
            myAnim.SetBool("S Run", false);
        }
        else
        {
            myAnim.SetBool("W Run", Input.GetKey(KeyCode.W));
            myAnim.SetBool("S Run", Input.GetKey(KeyCode.S));
            myAnim.SetBool("A Run", Input.GetKey(KeyCode.A));
            myAnim.SetBool("D Run", Input.GetKey(KeyCode.D));
        }
    }

    void Click_Keyboard()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            chatList.gameObject.SetActive(true);
        }
        else
        {
            chatList.gameObject.SetActive(false);
        }

        if (!Input.anyKeyDown) return;

        

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Dance", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Sing", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Greeting", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Angry", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Clap", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Surprised", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Kick", RpcTarget.All, myPV.ViewID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);
            myPV.RPC("RPC_Swim", RpcTarget.All, myPV.ViewID);
        }
    }

    IEnumerator Chating_Del(string savedText)
    {
        chating_text.color = new Color(0f, 0f, 0f, 1f);
        chating_text.text = savedText;

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(Chating_Fade());

        chatCoroutine = null; // 완료 후 null 처리
    }

    IEnumerator Chating_Fade()
    {
        float duration = 1f;
        float elapsed = 0f;

        Text txt = chating_text;
        Color originalColor = txt.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            txt.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        txt.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        chating_text.text = "";
    }

    public void Who_InHouse(string text)
    {
        myPV.RPC("RPC_ChatText", RpcTarget.All, text, myPV.ViewID);
    }

    [PunRPC]
    void RPC_ChatText(string text, int senderViewID)
    {
        // 내가 보낸 게 아니라면 무시
        if (myPV.ViewID != senderViewID)
            return;

        myPV.RPC("RPC_ResetEmotion", RpcTarget.All, myPV.ViewID);

        Debug.Log("채팅 수신됨: " + text);

        // 이미 실행 중이면 중단
        if (chatCoroutine != null)
            StopCoroutine(chatCoroutine);

        chatCoroutine = StartCoroutine(Chating_Del(text));
    }

    [PunRPC]
    void RPC_Dance(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        myAnim.SetBool("Dance", true); // ✅ 애니메이션 실행
        dance_Img.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_Sing(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        myAnim.SetBool("Sing", true); // ✅ 애니메이션 실행
        sing_Image.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_Greeting(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        StartCoroutine(Hi_Trigger());
    }

    IEnumerator Hi_Trigger()
    {
        myAnim.SetBool("Hi", true);
        hi_Image.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (hi_Image.gameObject.activeSelf)
        {
            hi_Image.gameObject.SetActive(false);
        }

        if (myAnim.GetBool("Hi"))
        {
            myAnim.SetBool("Hi", false);
        }
    }

    [PunRPC]
    void RPC_Angry(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        StartCoroutine(Angry_Trigger());
    }

    IEnumerator Angry_Trigger()
    {
        myAnim.SetBool("Angry", true);
        angry_Image.gameObject.SetActive(true);

        yield return new WaitForSeconds(19f);

        if (angry_Image.gameObject.activeSelf)
        {
            angry_Image.gameObject.SetActive(false);
        }

        if (myAnim.GetBool("Angry"))
        {
            myAnim.SetBool("Angry", false);
        }
    }

    [PunRPC]
    void RPC_Clap(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        StartCoroutine(Clap_Trigger());
    }

    IEnumerator Clap_Trigger()
    {
        myAnim.SetBool("Clap", true);
        clap_Image.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (clap_Image.gameObject.activeSelf)
        {
            clap_Image.gameObject.SetActive(false);
        }

        if (myAnim.GetBool("Clap"))
        {
            myAnim.SetBool("Clap", false);
        }
    }

    [PunRPC]
    void RPC_Surprised(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        StartCoroutine(Surprised_Trigger());
    }

    IEnumerator Surprised_Trigger()
    {
        myAnim.SetBool("Surprised", true);
        surprisec_Image.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);

        if (surprisec_Image.gameObject.activeSelf)
        {
            surprisec_Image.gameObject.SetActive(false);
        }

        if (myAnim.GetBool("Surprised"))
        {
            myAnim.SetBool("Surprised", false);
        }
    }

    [PunRPC]
    void RPC_Kick(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        myAnim.SetBool("Kick", true); // ✅ 애니메이션 실행
        kick_Image.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_Swim(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        myAnim.SetBool("Swim", true); // ✅ 애니메이션 실행
        swim_Image.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_ResetEmotion(int senderViewID)
    {
        if (myPV.ViewID != senderViewID)
            return;

        myAnim.SetBool("Dance", false);
        myAnim.SetBool("Sing", false);
        myAnim.SetBool("Hi", false);
        myAnim.SetBool("Angry", false);
        myAnim.SetBool("Clap", false);
        myAnim.SetBool("Surprised", false);
        myAnim.SetBool("Kick", false);
        myAnim.SetBool("Swim", false);

        dance_Img.gameObject.SetActive(false);
        sing_Image.gameObject.SetActive(false);
        hi_Image.gameObject.SetActive(false);
        angry_Image.gameObject.SetActive(false);
        clap_Image.gameObject.SetActive(false);
        surprisec_Image.gameObject.SetActive(false);
        kick_Image.gameObject.SetActive(false);
        swim_Image.gameObject.SetActive(false);
    }

}
