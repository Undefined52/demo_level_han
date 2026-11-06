using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    
    [Header("Camera")]
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [Header("UI")] 
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject clueBook;
    
    [Header("Clues")]
    [SerializeField] private float clueDistance;
    [SerializeField] private Transform clueCheckOrigin;

    private Camera _boardCam;
    private bool _inBoard;
    
    private CharacterController _controller;
    private Transform _camPivot;
    private Transform _character;
    private Animator _animator;
    
    // input
    private Vector3 _moveDirection = Vector3.zero;
    private bool _looking;
    private bool _pauseMenu;

    private Vector2 _lookVector;
    private float characterAngle;
    private Vector2 velocity;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camPivot = transform.Find("CameraPivot");
        _character = transform.Find("Character");
        _animator = _character.GetComponent<Animator>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _controller.SimpleMove(_character.rotation * _moveDirection);
        
        velocity.y += (_moveDirection.z - velocity.y) * Time.deltaTime * 10;
        velocity.x += (_moveDirection.x - velocity.x) * Time.deltaTime * 10;
        
        _animator.SetFloat("speed", velocity.y);
        _animator.SetFloat("Strafe", velocity.x);

        UpdateRotation();
    }

    public void OnPickup(InputValue value) {
        if(_inBoard) {
            _inBoard = false;
            _boardCam.gameObject.SetActive(false);
            GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
            UpdateTimeScale();
        } else {
            _animator.SetTrigger("pickup");
        }
    }

    public void OnPause() {
        _pauseMenu = !_pauseMenu;
        pauseMenu.SetActive(_pauseMenu);
        UpdateTimeScale();
    }

    private void UpdateTimeScale() {
        Time.timeScale = _inBoard | _pauseMenu ? 0 : 1;
        Cursor.lockState = _inBoard | _pauseMenu ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Quit() {
        Application.Quit();
    }

    public void CheckForClues() {
        Collider[] clueChecks = Physics.OverlapSphere(clueCheckOrigin.position, clueDistance);
        foreach(Collider clue in clueChecks) {
            if(clue.gameObject.name == "GuessBoard") {
                print("Guessboard");
                _boardCam = clue.gameObject.GetComponentInChildren<Camera>(true);
                _boardCam.gameObject.SetActive(true);
                GetComponentInChildren<Camera>(true).gameObject.SetActive(false);
                _inBoard = true;
                UpdateTimeScale();
            }
            if(!clue.CompareTag("Clue")) {
                continue;
            }

            AddClue(clue.gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.rebeccaPurple;
        Gizmos.DrawWireSphere(clueCheckOrigin.position, clueDistance);
        
    }

    private void AddClue(GameObject clue) {
        RawImage[] images = clueBook.GetComponentsInChildren<RawImage>(true);
        foreach(RawImage image in images) {
            string bookClueName = image.gameObject.transform.parent.parent.gameObject.name;
            if(image.gameObject.transform.parent.gameObject.activeSelf) {
                continue;
            }
            if(bookClueName == "Weapon" && clue.name == "shovel") {
                clueBook.SetActive(true);
                image.gameObject.transform.parent.gameObject.SetActive(true);
            } else if(bookClueName == "Wool" && clue.name == "Wool") {
                clueBook.SetActive(true);
                image.gameObject.transform.parent.gameObject.SetActive(true);
            } else if(bookClueName == "Location" && clue.name == "bodytape") {
                clueBook.SetActive(true);
                image.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    private void OnClues() {
        clueBook.SetActive(!clueBook.activeSelf);
    }

    
    private void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        
        _moveDirection.x = move.x * speed;
        _moveDirection.z = move.y * speed;
        if (_moveDirection.z < 0) _moveDirection.z *= 0.5f;
        _moveDirection.x *= 0.5f;
        _animator.SetBool("moving", move != Vector2.zero);
    }

    private void OnMouseMove(InputValue value)
    {
        _lookVector += value.Get<Vector2>() * cameraSpeed;
        if (_lookVector.y < minAngle) _lookVector.y = minAngle;
        else if (_lookVector.y > maxAngle) _lookVector.y = maxAngle;
        _camPivot.rotation = Quaternion.Euler(-_lookVector.y, _lookVector.x, 0f);
    }


    private void UpdateRotation()
    {
        float delta = _lookVector.x - characterAngle;
        delta %= 360f;
        characterAngle = _lookVector.x - delta;
        
        characterAngle += delta * Time.deltaTime * turnSpeed;
        _character.rotation = Quaternion.Euler(0, characterAngle, 0);
    }
}
