using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnityChanSan : MonoBehaviour
{
    public Animator animator;
    public Avatar unityChanAvatar;
    public Transform hand;
    public Ball ball;
    public bool canPlayerControl = false;
    public bool canPickUpBall = false;
    public bool canJump = false;
    public bool IsGrounded = true;
    public bool HasBall;
    public bool isAimingJump = false;
    public float JumpStrength;
    public GameObject guide;
    public GameObject jumpMeter;
    public SpriteRenderer meterGlow;
    public GameObject chanHolder;
    private GameData _GameData;
    public Transform modelRoot;
    public Transform floorEffect;

    private enum Direction
    {
        Left,
        Right,
        None
    }
    private Direction direction = Direction.None;
    private void Awake()
    {
        _GameData = new GameData();
        _GameData.Strength = 20;
        _GameData.Agility = 80;

        // _GroundCheck.OnFloorContact += FloorContact;
        //_GroundCheck.OnFloorExit += FloorExit;
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    public void StartBeginningAnimation()
    {
        StartCoroutine(BeginningAnimation());
    }
    IEnumerator BeginningAnimation()
    {
        yield return new WaitForSeconds(15.5f);
        PlayAnimation("Fall", true);
        yield return new WaitForSeconds(4.5f);
        canPlayerControl = true;
        ball.gameObject.SetActive(true);

    }
    // Update is called once per frame
    void Update()
    {
        if(canPlayerControl)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                direction = Direction.Left;
                if(IsGrounded)
                {
                    PlayAnimation("Walk", true);
                }
                //modelRoot.eulerAngles = new Vector3(0f, 270f, 0f);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                direction = Direction.Right;
                if (IsGrounded)
                {
                    PlayAnimation("Walk", true);
                }
                //modelRoot.eulerAngles = new Vector3(0f, 90f, 0f);
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!HasBall && canPickUpBall)
                {
                    StartCoroutine(PickUpBall());
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (HasBall)
                {
                    //StartCoroutine(ThrowBall());
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpaceDown();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                SpaceHold();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SpaceUp();
            }
            if (Input.GetKey(KeyCode.A) && direction == Direction.Left)
            {
                if(!IsGrounded)
                {
                    //transform.rotation = new Quaternion(0f, 0f, transform.rotation.z - .01f, 1f);
                    //GetComponent<Rigidbody>().AddForce(Vector3.left * 5f, ForceMode.Impulse);
                }
                transform.position = new Vector3(transform.position.x - (5f * Time.deltaTime), transform.position.y, transform.position.z);
                
            }
            if (Input.GetKey(KeyCode.D) && direction == Direction.Right)
            {
                if (!IsGrounded)
                {
                    //transform.rotation = new Quaternion(0f, 0f, transform.rotation.z + .01f, 1f);
                    //GetComponent<Rigidbody>().AddForce(Vector3.right * 5f, ForceMode.Impulse);
                }
                transform.position = new Vector3(transform.position.x + (5f * Time.deltaTime), transform.position.y, transform.position.z);
                
            }
/*
            if ((Input.GetKeyUp(KeyCode.A) && !Input.GetKeyUp(KeyCode.D)) || (Input.GetKeyUp(KeyCode.D) && !Input.GetKeyUp(KeyCode.A)))
            {
                PlayAnimation("Idle", true);
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }*/
            if(transform.position.y < 100f)
            {
                floorEffect.position = new Vector3(transform.position.x, floorEffect.position.y, floorEffect.position.z);
                floorEffect.GetComponent<ParticleSystem>().startColor = new Color(255, 255, 255, 255 - (int)((75f / transform.position.y)*255));
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = 100f;
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            Time.timeScale = 1f;
        }
    }

    private void SpaceDown()
    {
        JumpStrength = 0f;
        if(canJump)
        {
            jumpMeter.transform.parent.gameObject.SetActive(true);
            jumpMeter.transform.parent.eulerAngles = Vector3.zero;
            jumpMeter.transform.parent.localScale = new Vector3(0.1631047f, 0.1631047f, 0.1631047f);
            jumpMeter.transform.localScale = Vector3.one;
        }
    }
    private void SpaceHold()
    {
        if (canJump)
        {
            if (JumpStrength < 1f)
            {
                meterGlow.color = new Color(0f, JumpStrength / 1f, 0f, JumpStrength / 1f);
                JumpStrength += .002f;
                jumpMeter.transform.parent.gameObject.SetActive(true);
                jumpMeter.transform.localScale = new Vector3(1f, JumpStrength / 1f, 1f);
            }
            else
            {
                StartCoroutine(MissedJump());
            }
        }

    }
    private void SpaceUp()
    {
        if (canJump && JumpStrength > .99f && JumpStrength < 1f)
        {
            StartCoroutine(Jump());
        }
        else if(canJump)
        {
            StartCoroutine(MissedJump());
        }
        Time.timeScale = 1f;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    IEnumerator Jump()
    {
        canJump = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if(JumpStrength > .99f && JumpStrength < 1f)
        {
            GetComponent<Rigidbody>().AddForce(transform.up * (JumpStrength * 99800f), ForceMode.Impulse);
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(transform.up * (JumpStrength * 10000f), ForceMode.Impulse);
        }
        jumpMeter.transform.DOScaleY(0f, 1f);
        PlayAnimation("Jump" + Random.Range(0, 2).ToString(), true);
        jumpMeter.transform.parent.transform.DOScale(0f, .2f);
        yield return new WaitForSeconds(1f);
    }
    IEnumerator MissedJump()
    {
        canJump = false;
        jumpMeter.transform.parent.DOShakeRotation(.4f, 60, 5, 10, false);
        jumpMeter.transform.parent.DOShakeScale(.5f, .2f, 45, 15, false);
        yield return new WaitForSeconds(.5f);
        jumpMeter.transform.parent.gameObject.SetActive(false);
        jumpMeter.transform.parent.DOScale(0f, .2f);
        canJump = true;
    }

    IEnumerator PickUpBall()
    {
        guide.gameObject.SetActive(true);
        canPickUpBall = false;
        canPlayerControl = false;
        ball.GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().useGravity = false;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.transform.localEulerAngles = Vector3.zero;
        PlayAnimation("PickUp", false);
        ball.transform.DOShakePosition(.2f, .2f, 20, 45, false, true);
        ball.GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(.2f);
        ball.transform.parent = hand;
        ball.transform.DOLocalMove(Vector3.zero, .1f, false);
        yield return new WaitForSeconds(.1f);
        float timeAiming = 0f;
        while(Input.GetKey(KeyCode.Mouse0))
        {
            if(Time.timeScale > .6f)
            {
                Time.timeScale -= .00001f;
            }
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            Vector3 mousePosition = Input.mousePosition;
            timeAiming += Time.deltaTime;

            if(mousePosition.x < Screen.width / 2 )
            {
                ball.transform.eulerAngles = new Vector3(0f, 0f, -90f + ((mousePosition.x / (Screen.width / 2)) * 90f));
            }
            else
            {
                ball.transform.eulerAngles = new Vector3(0f, 0f, ((mousePosition.x - (Screen.width / 2)) / (Screen.width / 2)) * 90f);
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(ThrowBall());
        Time.timeScale = 1f;

    }
    IEnumerator ThrowBall()
    {
        //guide.gameObject.SetActive(false);
        PlayAnimation("Throw", false);
        HasBall = false;
        ball.transform.parent = null;
        ball.GetComponent<SphereCollider>().enabled = true;
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<Rigidbody>().AddForce(ball.transform.TransformDirection(Vector3.up) * _GameData.Strength, ForceMode.Impulse);
        canPlayerControl = true;
        canPickUpBall = true;
        HasBall = false;
        Time.timeScale = 1f;
        yield return new WaitForSeconds(.3f);
        PlayAnimation("Idle", true);

    }

    public void PlayAnimation(string name, bool isPremade)
    {
        if(isPremade)
        {
            animator.avatar = unityChanAvatar;
        }
        else
        {
            animator.avatar = null;
        }
        animator.Play(name);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
            IsGrounded = true;
            canJump = true;
        }
        if (other.gameObject.tag == "Ball")
        {
            canJump = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
            IsGrounded = false;
        }
    }
}
