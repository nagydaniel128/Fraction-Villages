using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterControllerScript
{
    public Transform cam;

    static Player Instance;
    public static Player instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<Player>();
            }
            return Instance;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Cursor.visible = false;

        dodge = new Dodge(this);
        stunAttack = new StunAttack(this);
        shieldPiercingAttack = new ShieldPiercingAttack(this);
    }

    void Update()
    {
        SkillsCd();

        StunAttackAbility();
        PierceAttack();
        DodgeAbility();

        MoveWithKeyboard();
        RotateWithMouse();
        Attack();
        Block();
        WeaponTakeInOrOut();
    }

    private void LateUpdate()
    {
        CameraZoom();
    }



    float hz, v;
    void MoveWithKeyboard()
    {
        hz = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (hz != 0 || v != 0) 
        {
            if (weaponIsOut)
            {
                Vector3 sidle = hz == -1 ? -character.transform.right : hz == 1 ? character.transform.right : new Vector3(0, 0, 0);
                character.inputManager.moveDirection = (v == -1 ? -character.transform.forward : v == 1 ? character.transform.forward : new Vector3(0,0,0)) + sidle;
            }
            else
                character.inputManager.moveDirection = character.transform.forward * 4;
        } else
            character.inputManager.moveDirection = new Vector3(0, 0, 0);
    }
    float targetAngle = 0;
    void RotateWithMouse()
    {
        float hz = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hz, 0, v).normalized;
        if (!weaponIsOut)
        {
            if (direction.magnitude > 0.1f)
            {
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

                character.inputManager.lookAngle = targetAngle;
            }
        }
        else
        {
            if(v == -1)
                targetAngle = Mathf.Atan2(0, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y + 180;
            else
                targetAngle = Mathf.Atan2(0, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            character.inputManager.lookAngle = targetAngle;
        }
    }


    void Attack()
    {
        if (weaponIsOut)
            character.inputManager.attackPressed = Input.GetMouseButton(0);
    }
    void Block()
    {
        character.inputManager.blockPressed = Input.GetMouseButton(1);
    }




    //skills
    //dodge
    public Dodge dodge;
    void DodgeAbility()
    {
        Vector3 direction = new Vector3(hz, 0, v);

        dodge.abilityInputPressed = Input.GetKeyDown("space");

        if((direction.z == 1 || direction.z == 0) &&  direction.x == 0)
        {
            dodge.dodgeDirection = Dodge.dodgeDirections.nothing;
            return;
        }

        if (!(direction.x == 0 && direction.z == 0))
        {
            if (direction.z == -1)
            {
                dodge.dodgeDirection = Dodge.dodgeDirections.back;
                return;
            }
            if (direction.z >= 0)
            {
                if (direction.x < 0)
                    dodge.dodgeDirection = Dodge.dodgeDirections.left;
                else
                    dodge.dodgeDirection = Dodge.dodgeDirections.right;
            }
            else
            {
                if (direction.x < 0)
                    dodge.dodgeDirection = Dodge.dodgeDirections.right;
                else
                    dodge.dodgeDirection = Dodge.dodgeDirections.left;
            }
        }
        else
            dodge.dodgeDirection = Dodge.dodgeDirections.nothing;
    }

    public StunAttack stunAttack;
    void StunAttackAbility()
    {
        stunAttack.abilityInputPressed = Input.GetMouseButton(0) && Input.GetMouseButton(1);
    }
    ShieldPiercingAttack shieldPiercingAttack;
    public void PierceAttack()
    {
        shieldPiercingAttack.abilityInputPressed = Input.GetMouseButton(0) && Input.GetKey("space");
    }




    bool weaponIsOut;
    void WeaponTakeInOrOut()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (weaponIsOut)
            {
                weaponIsOut = false;
            }
            else
            {
                weaponIsOut = true;
            }
        }
    }


    void CameraZoom()
    {
        if (weaponIsOut)
        {
            if (Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Radius > 14)
            {
                //float speed = (Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Radius - 14) / 8f + 0.1f;
                Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Radius -= Time.deltaTime * 10;
                Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Height -= Time.deltaTime * 3;
                Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[0].m_Height -= Time.deltaTime * 6;
            }
        }
        else
        {
            if (Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Radius < 22)
            {
                //float speed = 1f - ((22 - Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Radius) / 8f) + 0.1f;
                Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Radius += Time.deltaTime * 10;
                Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[1].m_Height += Time.deltaTime * 3;
                Camera.main.GetComponent<Cinemachine.CinemachineFreeLook>().m_Orbits[0].m_Height += Time.deltaTime * 6;
            }
        }
    }


    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void SkillsCd()
    {
        dodge.Cd();
        stunAttack.Cd();
        shieldPiercingAttack.Cd();
    }
}
