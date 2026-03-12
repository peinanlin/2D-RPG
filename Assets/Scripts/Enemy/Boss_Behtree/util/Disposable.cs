using UnityEngine;
public class Disposable:MonoBehaviour
{
    public float lifetime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);    
    }
}
