using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;     // ������ �Ǵ� ������Ʈ (A ������Ʈ)
    public float orbitSpeed = 10f; // �ʴ� ���� �ӵ� (�� ����)

    public Vector3 orbitAxis = Vector3.up; // �����ϴ� �� (Y�� �������� ȸ��)

    void Update()
    {
        if (target == null) return;

        // target.position�� �߽����� orbitAxis �������� ȸ��
        transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);

        // �׻� target�� �ٶ󺸰� ����
        transform.LookAt(target);
    }
}
