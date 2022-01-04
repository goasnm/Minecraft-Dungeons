using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    new Rigidbody rigidbody;

    public float moveSpeed;
    public float JumpPoewr;

    public GameObject[] Weapons;
    public bool[] hasWeapons;

    public Camera followCamera;

    public int Ammo;
    public int Coin;
    public int Heart;

    public int Maxammo;
    public int Maxcoin;
    public int Maxheart;

    float hAxis;
    float vAxis;

    bool Att;
    bool Walk;
    bool Jump;
    bool Dodge;
    bool Item;
    bool Reload;
    bool weaponSwap1;
    bool weaponSwap2;

    bool isAttReady = true;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isBorder;

    int weaponIndex;
    int equipWeaponIndex = -1;
    int reAmmo;

    float attDelay;

    Vector3 inputDir;
    Vector3 inputDodge;

    Animator ani;

    GameObject nearObject;
    Weapon equipWeapon;

    Item item;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        playerMove();
        PlayerJump();
        PlayerAtt();
        PlayerReload();
        PlayerDodge();
        PlayerInteration();
        PlayerWeaponSwap();
    }

    //플레이어 키 받아오기
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        Walk = Input.GetButton("Walk");
        Jump = Input.GetButtonDown("Jump");
        Att = Input.GetButtonDown("Att");
        Reload = Input.GetButtonDown("Reload");
        Dodge = Input.GetButtonDown("Dodge");
        Item = Input.GetButtonDown("Interation");
        weaponSwap1 = Input.GetButtonDown("Swap1");
        weaponSwap2 = Input.GetButtonDown("Swap2");
    }

    //플레이어 이동
    void playerMove()
    {

        inputDir = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            inputDir = inputDodge;
        }

        if (!isAttReady || isReload)
        {
            inputDir = Vector3.zero;
        }

        if (!isBorder)
        {
           transform.position += inputDir * moveSpeed *(Walk ? .5f : 1f) * Time.deltaTime;
        }

        //플레이어가 움직이는 방향으로 바라본다 WASD
        transform.LookAt(transform.position + inputDir);

        //플레이어가 총쏜 방향으로 바라본다 마우스클릭
        if (Att)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

        ani.SetBool("isRun", inputDir != Vector3.zero);
        ani.SetBool("isWalk", Walk);
    }

    //플레이어 점프
    void PlayerJump()
    {
        if (Jump && !isJump && !isSwap)
        {
            rigidbody.AddForce(Vector3.up * JumpPoewr, ForceMode.Impulse);
            ani.SetBool("isJump", true);
            ani.SetTrigger("doJump");
            isJump = true;
        }
    }

    void PlayerAtt()
    {
        if (equipWeapon == null) return;

        attDelay += Time.deltaTime;
        isAttReady = equipWeapon.rate < attDelay;

        if (Att && isAttReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            ani.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            attDelay = 0;
        }
    }

    void PlayerReload()
    {
        if (equipWeapon == null)    return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if (Ammo == 0) return;

        if (Reload && !isJump && !isDodge && !isDodge && !isSwap && isAttReady)
        {
            ani.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        reAmmo = Ammo < equipWeapon.maxAmmo ? Ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        Ammo -= reAmmo;
        isReload = false;
    }

    void PlayerDodge()
    {
        if (Dodge && !isJump && inputDir != Vector3.zero && !isDodge && !isSwap)
        {
            inputDodge = inputDir;
            moveSpeed *= 2;
            ani.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", .5f);
        }
    }

    void DodgeOut()
    {
        moveSpeed *= .5f;
        isDodge = false;
    }

    // 플레이어 무기 스왑
    void PlayerWeaponSwap()
    {
        if (weaponSwap1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;
        if (weaponSwap2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;

        weaponIndex = -1;
        if (weaponSwap1) weaponIndex = 0;
        if (weaponSwap2) weaponIndex = 1;

        if ((weaponSwap1 || weaponSwap2) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = Weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            ani.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", .4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void PlayerInteration()
    {
        if (Item && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }
    void FreezeRotation()
    {
        rigidbody.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 1f, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            ani.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.itemType)
            {
                case global::Item.ItemType.Ammo:
                    Ammo += item.value;
                    if (Ammo > Maxammo)
                    {
                        Ammo = Maxammo;
                    }
                    break;
                case global::Item.ItemType.Coin:
                    Coin += item.value;
                    if (Coin > Maxcoin)
                    {
                        Coin = Maxcoin;
                    }
                    break;
                case global::Item.ItemType.Heart:
                    Heart += item.value;
                    if (Heart > Maxheart)
                    {
                        Heart = Maxheart;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }

        if (other.tag == "Item")
        {
            nearObject = other.gameObject;
        }

        //Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
