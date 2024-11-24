using Editor;
using Editor.Assets;
using Editor.Audio;
using Editor.Inspectors;
using Editor.MapEditor;
using Editor.MeshEditor;
using Editor.ShaderGraph.Nodes;
using Sandbox;
using Sandbox.Audio;
using Sandbox.UI;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Editor.Inspectors.AssetInspector;
using static Editor.TreeNode;

//TODO: Ignore 1 grid axis when normal surface is offgrid;
//Prefab selection

namespace Editor.PrefabPlacer;

[CanEdit(typeof(PlaceObj))] // Asset file extension, can do typeof(Class) for non-assets
public class PlacerInspector : InspectorWidget
{
	PlaceObj placer;
	private static Material LastMaterial = Material.Load( "materials/dev/reflectivity_30.vmat" );
	public static Label header;

    // If this isn't an Asset Inspector, use CharacterInspector(SerializedObject so) : base(so)
    public PlacerInspector( SerializedObject so ) : base( so )
    {
		if ( so.Targets.FirstOrDefault() is not PlaceObj tool )
			return;
		placer = tool;
        Layout = Layout.Column();
        Layout.Margin = 4;
        Layout.Spacing = 4;

        Rebuild();
    }

    // Rebuild the UI every hotload, so we catch changes to the Asset
    [EditorEvent.Hotload]
    void Rebuild()
    {

        Layout?.Clear( true );

        // Create a header
        header = Layout.Add( new Label( "Select an object", this ) );
        header.SetStyles( "font-size: 38px; font-weight: 500; font-family: Poppins" );



		var button = Layout.Add( new Button.Primary( "Open all", "list_alt", this )
		{
			Clicked = () => { placer.OpenAllFunctions(); }
		} );


		//button.Pressed = () => PlaceObj.OpenAllFunctions(button.ScreenPosition);
		//currentPrefabText = new Label() { Text = "none" };
		//window.Layout.Add( currentPrefabText );
		Layout.AddSpacingCell( 8 );

		var surfaceCheckbox = Layout.Add( new Checkbox( "Align to surface", this ) );
		surfaceCheckbox.StateChanged += ( CheckState state ) =>
		{
			PlaceObj.useSurfaceNormal = state == CheckState.On;
		};
		var selectCheckbox = Layout.Add( new Checkbox( "Select when placed", this ) );
		selectCheckbox.StateChanged += ( CheckState state ) =>
		{
			PlaceObj.selectOnPlace = state == CheckState.On;
		};

		var customSwitch = Layout.Add( new Checkbox( "Show only custom prefabs (BROKEN)", this ));
		Layout.AddSpacingCell( 8 );

		var deltaFloat = Layout.Add( new IntProperty( this ) { HighlightColor = Theme.Yellow, Icon = "height", ToolTip = "Distance from surface", Value = 8 } );
		deltaFloat.OnChildValuesChanged += ( Widget w ) => PlaceObj.placeDeltaDistance = deltaFloat.Value;
		 Layout.AddStretchCell();  
    }


}





[EditorTool("tools.ketal.prefab-placer")] // this class is an editor tool
[Title( "Object placement tool" )] // title of your tool
[Icon( "dashboard_customize" )]
[Group( "-1" )]
public partial class PlaceObj : EditorTool
{

	Vector3 placeOffsetPos;


	Rotation normalRot;

	public GameObject prefabTemplate;

	public static float placeDeltaDistance;
	public static bool useSurfaceNormal = false;
	public static bool selectOnPlace = false;
	public bool showOnlyCustom;


	PrefabFile prefabItem;

	Widget window;


	private Layout ControlLayout { get; set; }

	[Shortcut( "tools.ketal.prefab-placer", "Shift+E", typeof( SceneViewportWidget ) )]
	public static void ActivateTool()
	{
		EditorToolManager.SetTool( nameof( PlaceObj ) );
	}

	public override void OnDisabled()
	{
		Selection.Clear();
	}

	public override void OnEnabled()
	{
		useSurfaceNormal = false;
		placeDeltaDistance = 8f;
		selectOnPlace = false;

		AllowGameObjectSelection = false;
		Selection.Clear();
		Selection.Set( this );
	}





	public void OpenAllFunctions()
	{

		var menu = new Menu();
		
	
		var prefabs = AssetSystem.All.Where( x => x.AssetType.FileExtension == "prefab" )
					.Where( x => x.RelativePath.StartsWith( "templates/gameobject/" ) )

					.Select( x => x.LoadResource<PrefabFile>() )
					.Where( x => x.ShowInMenu )
					.OrderByDescending( x => x.MenuPath.Count( x => x == '/' ) )
					.ThenBy( x => x.MenuPath )
					.ToArray();

		foreach ( var entry in prefabs )
		{
			menu.AddOption( entry.MenuPath.Split( '/' ), entry.MenuIcon, () =>
			{
				using var scope = SceneEditorSession.Scope();
				prefabItem = entry;

				PlacerInspector.header.Text = entry.ResourceName;
				PlacerInspector.header.SetStyles( "font-size: 38px; font-weight: 500; font-family: Poppins; color:#B0E24D" );
			} );
		}


		menu.OpenAtCursor();

	}
	
	public override void OnUpdate()
	{


		var tr = Trace
				.UseRenderMeshes( true )
				.UsePhysicsWorld( true )
				.Run();

		if ( !tr.Hit )
		{
			var plane =  new Plane( Vector3.Up, 0f );
			if ( plane.TryTrace( new Ray( tr.StartPosition, tr.Direction ), out tr.EndPosition, true ) )
			{
				tr.Hit = true;
				tr.Normal = plane.Normal;
			}
		}

		if ( tr.Hit )
		{
			if ( EditorScene.GizmoSettings.SnapToGrid )
			{
				tr.EndPosition = tr.EndPosition.SnapToGrid( EditorScene.GizmoSettings.GridSpacing, true, true, true );
			}

			using ( Gizmo.Scope( "tool", new Transform( tr.EndPosition, Rotation.LookAt( tr.Normal ) ) ) )
			{

				Gizmo.Draw.Color = Color.Red;
				Gizmo.Draw.LineCircle( 0, 0.5f );
				Gizmo.Draw.Color = Color.White.WithAlpha( 0.5f );
				Gizmo.Draw.LineCircle( 0, 3 );
				Gizmo.Draw.Color = Color.White.WithAlpha( 0.3f );
				Gizmo.Draw.LineCircle( 0, 6 );
				Gizmo.Draw.Color = Color.White.WithAlpha( 0.2f );
				Gizmo.Draw.LineCircle( 0, 12 );
				Gizmo.Draw.Color = Gizmo.Colors.Blue;

			
				Gizmo.Transform = new Transform( tr.EndPosition + tr.Normal * placeDeltaDistance, Rotation.LookAt( tr.Normal ) );
				Gizmo.Draw.LineBBox( BBox.FromPositionAndSize( 0, 4f ) );
				if ( Gizmo.HasClicked ) AddObject(prefabItem, tr.EndPosition + tr.Normal * placeDeltaDistance, tr);


			}
		}


		
	}

		//adds an empty gameobject at location, should replace it with library later
	void AddObject(PrefabFile entry, Vector3 pos, SceneTraceResult tr)
	{

		using ( Gizmo.Scope( "tool" ) )
		{
			using var scope = SceneEditorSession.Scope();

			var go = GameObject.Clone( "prefabs/plantedcat.prefab" );
			go.Name = "plantedcat";
			go.Parent = Scene.Directory.FindByName( "Harvestable Cats" ).FirstOrDefault();
			
			if ( useSurfaceNormal )
			{
				go.Transform.Local = new Transform( pos, Rotation.LookAt( tr.Normal ));
				Log.Info( tr.Normal );
			}
			else
			{
				go.Transform.Local = new Transform( pos, normalRot );
			}
			if ( selectOnPlace )
			{
				
				//EditorToolManager.SetTool( nameof( ObjectEditorTool ) );
				Selection.Set( go );
	
			}


		}
	}


}

