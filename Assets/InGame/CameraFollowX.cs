using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 따라갈 대상 (Player)
    public float xOffset = 5f;    // 플레이어보다 얼마나 앞을 비출 것인가?
    public float fixedY = 0f;     // ★ 카메라가 고정될 높이 (에디터에서 0으로 설정하세요)

    void LateUpdate()
    {
        if (target == null) return;

        // X축은 플레이어를 똑같이 따라가고, Y축은 우리가 정한 fixedY에 딱 고정!
        transform.position = new Vector3(target.position.x + xOffset, fixedY, transform.position.z);
    }
}