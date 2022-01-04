using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    new Rigidbody rigidbody;
    BoxCollider boxCollider;
    Material material;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
    }

    // 몬스터 피격시 체력 감소
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.Damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

            Debug.Log("Melee : " + curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.Damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));

            Debug.Log("Range : " + curHealth);
        }
    }

    // 몬스터 피격
    IEnumerator OnDamage(Vector3 reactVec)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(.1f);

        if (curHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.gray;
            gameObject.layer = 12;
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigidbody.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject, 3);
        }
    }
}
