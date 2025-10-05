using UnityEngine;

[RequireComponent (typeof (PlayerController))] 
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private PlayerController player_controller;
    [SerializeField] private PlayerSpriteDirectionController spriteController;


    private Vector3 _input;

    private void Update()
    {
        GatherInput();
        Look();
        if (spriteController != null ) spriteController.UpdateDirection(_input);
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, player_controller.GetTurnSpeed() * Time.deltaTime);
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

        player_controller.GetRigidBody().linearVelocity = Vector3.zero; // cancel leftover push velocity
        player_controller.GetRigidBody().MovePosition(transform.position + moveDir * player_controller.GetSpeed() * Time.deltaTime);
    }
    public void SetController(PlayerController controller)
    {
        player_controller = controller;
    }
}
