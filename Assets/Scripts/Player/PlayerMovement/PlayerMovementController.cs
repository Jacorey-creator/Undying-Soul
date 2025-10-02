using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float move_speed = 5;
    [SerializeField] private float turn_speed = 360;
    private Vector3 _input;

    private void Update()
    {
        GatherInput();
        Look();
    }
    private void FixedUpdate()
    {
        Move();
    }
    void Look() 
    {
        if (_input != Vector3.zero) 
        {
            var relative = (transform.position + _input.ToIso()) - transform.position;
            var rotate = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, turn_speed * Time.deltaTime);
        }

    }
    void GatherInput() 
    {
        Vector3 raw_input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        _input = Vector3.ClampMagnitude(raw_input, 1f);
    }

    void Move() 
    {
        Vector3 moveDir = _input.ToIso();

        rb.MovePosition(transform.position + moveDir * move_speed * Time.deltaTime);
    }
}
