using Sandbox;

public sealed class Catcheck : Component, Component.ExecuteInEditor
{
	private List<GameObject> cats;
	protected override void OnUpdate()
	{
		if ( !Scene.IsEditor ) return;
		var allObjects = Scene.Directory.FindByName( "plantedcat" );
		
		cats = allObjects.Where(obj => obj.Name == "plantedcat").ToList();
		// Log.Info($"Found {cats.Count} cats.");
	}
}
