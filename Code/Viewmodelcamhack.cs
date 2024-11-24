using Sandbox;

public sealed class Viewmodelcamhack : Component
{
	protected override void OnStart()
	{
		GetComponent<CameraComponent>().ClearFlags = ClearFlags.Stencil | ClearFlags.Depth | ClearFlags.Color;
		_ = Task.DelaySeconds(0.03f).ContinueWith(_ => { GetComponent<CameraComponent>().ClearFlags = ClearFlags.Stencil | ClearFlags.Depth; });
	}

	protected override void OnUpdate()
	{

	}
}
