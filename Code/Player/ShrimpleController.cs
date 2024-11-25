using Sandbox.Citizen;

public class ShrimpleWalker : Component
{
    [RequireComponent]
    public ShrimpleCharacterController.ShrimpleCharacterController Controller { get; set; }

    [RequireComponent]
    public CitizenAnimationHelper AnimationHelper { get; set; }
    public SkinnedModelRenderer Renderer { get; set; }
    [Property] 
    public GameObject Camera { get; set; }

    [Property]
    [Range(50f, 200f, 10f)]
    public float WalkSpeed { get; set; } = 100f;

    [Property]
    [Range(100f, 500f, 20f)]
    public float RunSpeed { get; set; } = 300f;

    [Property]
    [Range(200f, 500f, 20f)]
    public float JumpStrength { get; set; } = 350f;

    public Angles EyeAngles { get; set; }

    protected override void OnStart()
    {
        base.OnStart();

        Renderer = Components.Get<SkinnedModelRenderer>(FindMode.EverythingInSelfAndDescendants);
        var cameraComponent = Camera.Components.Get<CameraComponent>();
        cameraComponent.ZFar = 32768f;
        cameraComponent.FieldOfView = Preferences.FieldOfView;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        var wishDirection = Input.AnalogMove.Normal * Rotation.FromYaw(EyeAngles.yaw);
        var isRunning = Input.Down("Run");
        var wishSpeed = isRunning ? RunSpeed : WalkSpeed;

        Controller.WishVelocity = wishDirection * wishSpeed;
        Controller.Move();

        if (Input.Pressed("Jump") && Controller.IsOnGround)
        {
            Controller.Punch(Vector3.Up * JumpStrength);
            AnimationHelper?.TriggerJump();
        }

        if (!AnimationHelper.IsValid()) return;

        AnimationHelper.WithWishVelocity(Controller.WishVelocity);
        AnimationHelper.WithVelocity(Controller.Velocity);
        AnimationHelper.IsGrounded = Controller.IsOnGround;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        EyeAngles += Input.AnalogLook;
        EyeAngles = EyeAngles.WithPitch(MathX.Clamp(EyeAngles.pitch, -89f, 89f));
        Renderer.WorldRotation = Rotation.Slerp(Renderer.WorldRotation, Rotation.FromYaw(EyeAngles.yaw), Time.Delta * 5f);

        Camera.WorldRotation = EyeAngles.ToRotation();
    }
}
