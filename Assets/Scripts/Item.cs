using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType 
    {
        Ammo,
        Coin,
        Heart,
        Weapon
    }

    public ItemType itemType;
    public int value;

    new Rigidbody rigidbody;
    SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rigidbody.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
