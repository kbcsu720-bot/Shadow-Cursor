using UnityEngine;

public class CharacterInteraction : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 마우스로 이 오브젝트(Collider가 있는 곳)를 클릭했을 때 실행됨
    private void OnMouseDown()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log(gameObject.name + "의 음성 출력!");
        }
    }
}