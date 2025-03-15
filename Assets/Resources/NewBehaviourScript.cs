using UnityEngine;
public class RotateLoader : MonoBehaviour
{
    void Update() => transform.Rotate(0, 0, -360 * Time.deltaTime);
}