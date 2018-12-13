using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour {

    private Animator _animator;

    private CharacterController _characterController;

    private float Gravity = 10.0f;

    private Vector3 _moveDirection = Vector3.zero;

    public float Speed = 5.0f;

    public float RotationSpeed = 240.0f;

    public float JumpSpeed = 7.0f;

    public Transform firstTranform;

    float moveSpeed = 10.0f;
    Vector3 curPos, lstPos;
    // Use this for initialization

    public void Awake()
    {
        transform.position = firstTranform.transform.position;
    }
    void Start () {

        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        if (GameManager.instance.nextSpawnPoint != "")
        {
            GameObject spawnPoint = GameObject.Find(GameManager.instance.nextSpawnPoint);
            transform.position = spawnPoint.transform.position;

            GameManager.instance.nextSpawnPoint = "";
        }
        else if (GameManager.instance.lastHeroPosition != Vector3.zero)
        {
            transform.position = GameManager.instance.lastHeroPosition;
            GameManager.instance.lastHeroPosition = Vector3.zero;
        }

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        float h = Input.GetAxis("Horizontal") * Speed;
        float v = Input.GetAxis("Vertical");



        Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

        if (move.magnitude > 1f) move.Normalize();

        //Vector3 movement = new Vector3(h, 0.0f, v);
        //GetComponent<Rigidbody>().velocity = movement * moveSpeed; // * Time.deltaTime;

        move = transform.InverseTransformDirection(move);

        float turnAmount = Mathf.Atan2(move.x, move.z);

        transform.Rotate(0, turnAmount * RotationSpeed * Time.deltaTime, 0);




        if (_characterController.isGrounded)
        {
            _moveDirection = transform.forward * move.magnitude;

            _moveDirection *= Speed;

            if (Input.GetButton("Jump"))
            {
                _animator.SetBool("is_in_air", true);
                _moveDirection.y = JumpSpeed;

            }
            else
            {
                _animator.SetBool("is_in_air", false);

                if (Input.GetButton("Run"))
                {
                    _animator.SetBool("walk", false);
                    _animator.SetBool("run", true);

                }
                else
                {
                    _animator.SetBool("walk", move.magnitude > 0);
                    _animator.SetBool("run", false);
                }             
            }
        }

        _moveDirection.y -= Gravity * Time.deltaTime;

        _characterController.Move(_moveDirection * Time.deltaTime);

        //old code
        curPos = transform.position;

        if (curPos == lstPos)
        {
            GameManager.instance.isWalking = false;
        }
        else
        {
            GameManager.instance.isWalking = true;
        }
        lstPos = curPos;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Teleport") {
            
            CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();

            GameManager.instance.nextSpawnPoint = col.spawnPointName;
            GameManager.instance.sceneToLoad = col.sceneToLoad;

            GameManager.instance.LoadNextScene();
            Debug.Log("Colider");
        }

        if (other.tag == "region1")
        {
            GameManager.instance.curRegions = 0;
        }
        if (other.tag == "region2")
        {
            GameManager.instance.curRegions = 1;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "region1" || other.tag == "region2")
        {
            GameManager.instance.canGetEncouter = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "region1" || other.tag == "region2")
        {
            GameManager.instance.canGetEncouter = false;
        }
    }
}
