using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal = 0f;
    private float speed = 20f;
    private float jumpingPower = 48f;
    private bool isFacingRight = true; 
    private bool wasGrounded = false, isGrounded = false;
    private bool isDeath = false;
    private bool leftPressed = false, rightPressed = false;
    private int walkingStep = 0, framesPerStep = 15; //20; //40;

    public Sprite jumpSprite, standSprite, stand2Sprite, walking1Sprite, walking2Sprite, walking3Sprite, deathSprite;

    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public SpriteRenderer sp;
    //public CoinManager coinManager;
    // [SerializeField] private Transform groundCheck;
    // [SerializeField] private LayerMask groundLayer;

    public void setDeath(bool death) {
        isDeath = death;
    }

    void Awake() {
        //Game.instance.player = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) leftPressed = true;
        if (Input.GetKeyUp(KeyCode.LeftArrow)) leftPressed = false;
        if (Input.GetKeyDown(KeyCode.RightArrow)) rightPressed = true;
        if (Input.GetKeyUp(KeyCode.RightArrow)) rightPressed = false;
        horizontal = 0f;
        if (leftPressed && !rightPressed) horizontal = -1;
        if (!leftPressed && rightPressed) horizontal = 1;
        isGrounded = IsGrounded();
        if (!wasGrounded && isGrounded) {
            Game.instance.playLanding();
        }
        wasGrounded = isGrounded;
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            Game.instance.playJump();
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) && rb.velocity.y > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        Flip();       
        MoveBackgrounds(); 
        Animate();
    }

    void Animate() {
        if(isDeath) {
            sp.sprite = deathSprite;
        } else {
            if(isGrounded) {
                if(horizontal == 0f) {
                    switch ((int)(walkingStep / framesPerStep)) {
                        case 0: case 1:
                            sp.sprite = standSprite;
                            break;
                        case 2: case 3:
                            sp.sprite = stand2Sprite;
                            break;
                        default:
                            walkingStep = 0;
                            break;
                    }
                } else {
                    switch ((int)(walkingStep / framesPerStep)) {
                        case 0:
                            sp.sprite = walking1Sprite;
                            break;
                        case 1: case 3:
                            sp.sprite = walking2Sprite;
                            break;
                        case 2: 
                            sp.sprite = walking3Sprite;
                            break;
                        default:
                            walkingStep = 0;
                            break;
                    }                    
                }
                walkingStep++;
                if(walkingStep >= framesPerStep * 4) walkingStep = 0;
            } else {
                sp.sprite = jumpSprite;
            }
        }        
    }

    void MoveBackgrounds() {
        Vector3 playerPosition = rb.transform.position;
        float woodsScale = 0.2f, cloudsScale = 0.1f, cloudHeight = 100;
        Game.instance.woodsBackground.transform.position = new Vector3(woodsScale * playerPosition.x, woodsScale * playerPosition.y, 3);
        Game.instance.cloudsBackground.transform.position = new Vector3(cloudsScale * playerPosition.x, cloudsScale * playerPosition.y + cloudHeight, 2);
    }

    private bool IsGrounded() {
        return Mathf.Abs(rb.velocity.y) <= 1e-3;
    }

    private void Flip() {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f) {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void OnTriggerEnter2D(Collider2D other) { 
        if(other.gameObject.CompareTag("Coin")) {
            Destroy(other.gameObject);
            CoinManager coinManager = Game.instance.coinManager;
            coinManager.coinCount += 1;
            if(coinManager.coinCount >= coinManager.totalCoins) {
                Game.instance.Win();
            } else {
                Game.instance.playCoin();
            }
        }
    }

}