public sealed class Viewmodelcamhack : Component
{
	protected override void OnStart()
	{
		var camera = GetComponent<CameraComponent>();
		if ( !camera.IsValid() ) 
			return;
		camera.ClearFlags = ClearFlags.Stencil | ClearFlags.Depth | ClearFlags.Color;
		_ = Task.DelaySeconds(0.03f).ContinueWith(_ => { camera.ClearFlags = ClearFlags.Stencil | ClearFlags.Depth; });
	}
}
