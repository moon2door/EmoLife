using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MaterialChanger : MonoBehaviourPun
{
    public Material[] changeMaterials; // 인스펙터에 넣기
    public Player_Door playerdoor;

    public Button[] materialButtons;
    public GameObject button_total;

    private SkinnedMeshRenderer smr;

    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();

        if (photonView.IsMine)
        {
            for (int i = 0; i < materialButtons.Length; i++)
            {
                int index = i;
                materialButtons[i].onClick.AddListener(() =>
                {
                    photonView.RPC("ChangeMaterialRPC", RpcTarget.AllBuffered, index);
                });
            }
        }
        else
        {
            button_total.SetActive(false);
        }
    }


    void Update()
    {
        if (!photonView.IsMine) return;

        if (button_total != null)
            button_total.SetActive(playerdoor.isHouse);
    }


    [PunRPC]
    void ChangeMaterialRPC(int index)
    {
        if (changeMaterials == null || index >= changeMaterials.Length) return;

        Material[] currentMats = smr.materials;
        if (currentMats.Length == 0) return;

        currentMats[0] = changeMaterials[index]; // 0번 머테리얼 교체
        smr.materials = currentMats;
    }
}
