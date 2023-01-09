using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float omega;

    // Start is called before the first frame update
    // void Start()
    // {
    // }

    // Update is called once per frame
    void Update()
    {
        transform.position = Rotate_Yaxis(transform.position, omega * Time.deltaTime);
    }

    Vector3 Rotate_Yaxis(Vector3 from, float angle) {
        System.Numerics.Complex a = new System.Numerics.Complex(from.x, from.z);
        float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
        System.Numerics.Complex b = new System.Numerics.Complex(cos, sin);
        System.Numerics.Complex result = a * b;
        return new Vector3(
                (float) result.Real, 
                from.y,
                (float) result.Imaginary
            );
    }

    public void setRandomOmega() {
        this.omega = (Random.value - (float)0.5) * 720;
    }

    public void setRandomPosition() {
        transform.position = new Vector3(
            Random.value * 8 - 4, 
            transform.position.y, 
            Random.value * 8 - 4
        );
    }
}
