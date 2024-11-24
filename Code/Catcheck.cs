/// <summary>
/// This is for placing down cats, enable it in the "Harvestable Cats" GameObject
/// when you want to see the current Cat Count
/// </summary>
public sealed class Catcheck : Component, Component.ExecuteInEditor
{
	private List<GameObject> cats;
	protected override void OnUpdate()
	{
		if ( !Scene.IsEditor ) return;
		var allObjects = Scene.Directory.FindByName( "plantedcat" );
		
		cats = allObjects.Where(obj => obj.Name == "plantedcat").ToList();
		Log.Info($"Found {cats.Count} cats.");
	}
}
