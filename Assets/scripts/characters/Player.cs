using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[System.Flags]
public enum FireMode { Semi, Burst, Auto}


public class Player : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField, Range(1,20)] private float mouseSensX;
    [SerializeField, Range(1,20)] private float mouseSensY;

    [SerializeField, Range(0,180)] private float minViewAngle;
    [SerializeField, Range(0,180)] private float maxViewAngle;

    [SerializeField] private Transform lookAtPoint;
 
    [SerializeField] private Rigidbody bulletPrefab;
    [SerializeField] private float projectileForce;

    public Camera playerCamera;

    AudioSource m_shootingSound; 

    //Burst fire script
    [Header("Burst Fire")]
    [SerializeField] private KeyCode _shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _cycleFireModeKey = KeyCode.V;
    [SerializeField] private float _fireRate = 0.00f;
    private float _fireTimer = 0.0f;
    [SerializeField] private FireMode _fireMode = FireMode.Semi;
    private bool _shootInput = false;
    private bool _bursting = false;

    //
    private Vector2 currentRotation;
    
    private bool isGrounded;
    private Vector3 _moveDir;
    
    //Reload script
    [Header("Reload")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;

    [Header("Player UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI shotsFired;

    [SerializeField] private float maxHealth;
    private int shotsFiredCounter;
    private float _health;

    private float Health
    {
        get => _health;
        set
        {
            _health = value;
            healthBar.fillAmount = _health / maxHealth;
        }
    }

    
    private float depth;
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Init(myPlayer:this);
        InputManager.GameMode();

        Health = maxHealth;

        currentAmmo = maxAmmo;

        shotsFiredCounter = maxAmmo;

        m_shootingSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Time.deltaTime * _moveDir;

        Health -= Time.deltaTime * 5;

        if(isReloading)
        return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;

        }


        //burst fire
        if (_fireTimer < _fireRate + 1.0f)
        _fireTimer += Time.deltaTime;

        switch (_fireMode)
        {
            case FireMode.Auto:
            _shootInput = Input.GetKey(_shootKey);
            break;

            default:
            _shootInput = Input.GetKeyDown(_shootKey);
            break;
        }


        //shoot
        if (_shootInput)
        {
            if (_fireTimer > _fireRate && !_bursting)
            {
                //shoot
                m_shootingSound.Play();
                Shoot();

                //reset fire rate
                _fireTimer = 0.0f;

                if (_fireMode == FireMode.Burst)
                {
                    _bursting = true;
                    StartCoroutine(BurstFire());
                }
            }
        }

        //fire mode
        if (Input.GetKeyDown(_cycleFireModeKey))
        CycleFireMode();

    }
    
    public void SetMovementDirection(Vector3 newDirection)
    {
        _moveDir = newDirection;
        //Debug.Log("Shoot");
    }

  
    private void CheckGround()
   {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, depth, groundLayers);
        Debug.DrawRay(transform.position, Vector3.down * depth,
            Color.green, 0, false);
    }

    public void SetLookDirection(Vector2 ReadValue)
    {
        //Controls rotation angles
        currentRotation.x += ReadValue.x * Time.deltaTime * mouseSensX;
        currentRotation.y += ReadValue.y * Time.deltaTime * mouseSensY;

        //Rotates left and right
        transform.rotation = Quaternion.AngleAxis(currentRotation.x, Vector3.up);

        //Clamp rotation anglw so you can't roll your head
        currentRotation.y = Mathf.Clamp(currentRotation.y, minViewAngle, maxViewAngle);

        //Rotate up and down
        lookAtPoint.localRotation = Quaternion.AngleAxis(currentRotation.y, Vector3.right);
    }

    public IEnumerator Reload ()
    {
        isReloading = true;
        Debug.Log("Reloading..");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        shotsFiredCounter = maxAmmo;
        isReloading = false;
    }

    

    public void Shoot()
    {
        currentAmmo--;

        Rigidbody currentProjectile = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        currentProjectile.AddForce(lookAtPoint.forward * projectileForce, ForceMode.Impulse);

        

        --shotsFiredCounter;

        shotsFired.text = shotsFiredCounter.ToString();

        Destroy(currentProjectile.gameObject, 4);
    }

    //fire mode

    private void CycleFireMode() => _fireMode = ((int)_fireMode < 2) ? _fireMode + 1 : 0;

    //burst fire coroutine
    private IEnumerator BurstFire()
    {
        yield return new WaitForSeconds(_fireRate);
        Shoot();
        yield return new WaitForSeconds(_fireRate);
        Shoot();
        yield return new WaitForSeconds(_fireRate);
        _bursting = false;
    }
    
}