using UnityEngine;

public class ObjectInspectable : MonoBehaviour
{
    public float InspectDistance = 0.1f;
    public float InspectScale = 0.3f;

    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();        
    }
}
