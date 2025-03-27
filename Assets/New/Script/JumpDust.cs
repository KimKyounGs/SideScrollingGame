using UnityEngine;

public class JumpDust : MonoBehaviour
{
    public float lifetime = 0.5f;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }
}
