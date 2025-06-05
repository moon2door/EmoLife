using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player_Door : MonoBehaviour
{
    GameObject player;
    PlayerMove playermove;
    CharacterController myCC;
    PhotonView myPV;

    public Image e_Button;
    public Image esc_Button;
    public GameObject skill;
    public Text chat_Text;

    public Transform tp_House;
    public Transform tp_Door;

    public GameObject second_Camera;

    public Image exit_Image;

    public bool isDoor = false;
    public bool isHouse = false;

    public static bool isHouseOccupied = false;


    void Start()
    {
        myPV = GetComponent<PhotonView>();
        playermove = GetComponent<PlayerMove>();

        tp_House = GameObject.Find("TP_House").GetComponent<Transform>();
        tp_Door = GameObject.Find("TP_Door").GetComponent<Transform>();

        myCC = GetComponent<CharacterController>();

        second_Camera = FindObjectEvenIfInactive("Camera__Second");

        if (!myPV.IsMine)
        {
            second_Camera.SetActive(false); // 다른 사람은 꺼놓기
            e_Button.gameObject.SetActive(false);
            esc_Button.gameObject.SetActive(false);
            skill.SetActive(false);
            chat_Text.gameObject.SetActive(false);
            return;
        }

        second_Camera.SetActive(false);

        player = gameObject;
    }

    void Update()
    {
        if (!myPV.IsMine) return;

        E_Click();

        Esc_Exit();

        Esc_Click();
    }

    void E_Click()
    {
        if (!isDoor) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHouseOccupied && !isHouse)
            {
                playermove.Who_InHouse("<color=#FF0000>누가 집에 있는 것 같은데?</color>");
                return;
            }

            myCC.enabled = false;
            player.transform.position = tp_House.position;
            myCC.enabled = true;

            isHouse = true;
            isHouseOccupied = true;

            e_Button.gameObject.SetActive(false);
            skill.SetActive(false);

            esc_Button.gameObject.SetActive(true);
            second_Camera.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Esc_Click()
    {
        if (!isHouse) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            myCC.enabled = false;
            player.transform.position = tp_Door.position;
            myCC.enabled = true;

            isHouse = false;
            isHouseOccupied = false;

            esc_Button.gameObject.SetActive(false);
            second_Camera.SetActive(false);

            e_Button.gameObject.SetActive(true);
            skill.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Esc_Exit()
    {
        if (isHouse) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exit_Image.gameObject.activeSelf)
            {
                exit_Image.gameObject.SetActive(false);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                exit_Image.gameObject.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void _Click_Exit()
    {
        Application.Quit();
    }

    public void _Click_Cancle()
    {
        exit_Image.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!myPV.IsMine) return;

        if (other.CompareTag("Door"))
        {
            e_Button.gameObject.SetActive(true);
            isDoor = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!myPV.IsMine) return;

        if (other.CompareTag("Door"))
        {
            e_Button.gameObject.SetActive(false);
            isDoor = false;
        }
    }

    GameObject FindObjectEvenIfInactive(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }

}
