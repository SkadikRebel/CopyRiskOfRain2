using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotate : MonoBehaviour
{
    public float mouseSensitivity = 2f; // ���������������� �����

    public Camera cam;
    public Transform playerBody; // ������, ������� ����� �������������� �� ����������� (������ �������� ������)

    private void Awake()
    {
    }

    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
     
    }

    private void Start()
    {
    }

    private void Update()
    {
        playerBody.rotation = cam.transform.rotation;
    }
}
