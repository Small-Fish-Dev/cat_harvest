using Editor;
using Editor.Inspectors;
using Editor.MapEditor;
using Sandbox;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[EditorTool] // this class is an editor tool
[Title( "Object placement tool" )] // title of your tool
[Icon( "add_circle" )]
[Description( "Shortcut : Shift+E" )]
[Group( "0" )]
[Shortcut( "editortool.placeobj", "Shift+E" )] // keyboard shortcut
public class PlaceObj : EditorTool
{
	IntProperty deltaFloat;

	Vector3 placePos;
	Vector3 placeOffsetPos;

	Rotation normalRot;

	public GameObject prefabTemplate;

	public float placeDeltaDistance;
	public bool useSurfaceNormal;
	public bool showOnlyCustom;

	public Checkbox surfSwitch;

	public Checkbox customSwitch;

	PrefabFile prefabItem;

	WidgetWindow window;

	Label currentPrefabText;
	public override void OnEnabled()
	{
		AllowGameObjectSelection = false;
		Selection.Clear();

		// create a widget window. This is a window that  
		// can be dragged around in the scene view
		window = new WidgetWindow( SceneOverlay );
		window.Layout = Layout.Column();
		window.WindowTitle = "Select object";
		window.Layout.Margin = 4;
		window.AcceptDrops = false;
		window.FixedWidth = 210f;
		window.Icon = "add_circle";
		window.IsDraggable = false;



		surfSwitch = new Checkbox( "Align to surface", window );
		window.Layout.Add( surfSwitch );
		window.Layout.AddSpacingCell( 8 );

		customSwitch = new Checkbox( "Show only custom prefabs", window );
		window.Layout.Add( customSwitch );
		window.Layout.AddSpacingCell( 8 );

		//Create float for surface distance offset
		deltaFloat = window.Layout.Add( new IntProperty( window ) { HighlightColor = Theme.Yellow, Icon = "height", ToolTip = "Distance from surface", Value = 8 } );
		window.Layout.AddSpacingCell( 8 );

		


		var button = new Button( "Open all" );
		button.Pressed = () => OpenAllFunctions(button.ScreenPosition);
		currentPrefabText = new Label() { Text = "none" };
		window.Layout.Add( currentPrefabText );

		
		// Add the button to the window's layout

		window.Layout.Add( button );


		// Calling this function means that when your tool is deleted,
		// ui will get properly deleted too. If you don't call this and
		// you don't delete your UI in OnDisabled, it'll hang around forever.
		AddOverlay( window, TextFlag.LeftTop, 10 );

	}



	//adds an empty gameobject at location, should replace it with library later
	void AddObject(PrefabFile entry)
	{
		if ( entry != null )
		{
			using var scope = SceneEditorSession.Scope();



			var go = SceneUtility.GetPrefabScene( entry )?.Clone();
			go.BreakFromPrefab();
			go.Name = entry.MenuPath.Split( '/' ).Last();
			go.Transform.Local = new Transform( placeOffsetPos, normalRot );
			Selection.Set( go );
		}


	

	}

	void OpenAllFunctions(Vector2 pos)
	{
		showOnlyCustom = customSwitch.Value;
		var menu = new Menu();
		if ( customSwitch.Value == false )
		{

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
					currentPrefabText.Text = entry.ToString();
				} );
			}
		}
		var CustomPrefabs = AssetSystem.All.Where( x => x.AssetType.FileExtension == "prefab" )
						.Where( x => x.RelativePath.StartsWith( "prefabs/templates" ) )
						.Select( x => x.LoadResource<PrefabFile>() )
						.Where( x => x.ShowInMenu )
						.OrderByDescending( x => x.MenuPath.Count( x => x == '/' ) )
						.ThenBy( x => x.MenuPath )
						.ToArray();

		foreach ( var entry in CustomPrefabs )
		{
			menu.AddOption( entry.MenuPath.Split( '/' ), entry.MenuIcon, () =>
			{
				using var scope = SceneEditorSession.Scope();
				prefabItem = entry;
				currentPrefabText.Text = entry.ToString();
			} );
		}


		menu.OpenAt( pos, false );
	}

	public override void OnUpdate()
	{
		placeDeltaDistance = deltaFloat.Value;

		useSurfaceNormal = surfSwitch.Value;

		var tr = Scene.Trace.Ray( Gizmo.CurrentRay, 5000 )
				.UseRenderMeshes( true )
				.UsePhysicsWorld( false )
				.Run();

		if ( tr.Hit )
		{
			using ( Gizmo.Scope( "cursor" ) )
			{
				placePos = tr.HitPosition;
				placePos = placePos.SnapToGrid( EditorScene.GizmoInstance.Settings.GridSpacing, true, true, true );
				Gizmo.Transform = new Transform( placePos, Rotation.LookAt( tr.Normal ) );
				Gizmo.Draw.Color = Gizmo.Colors.Blue;
				Gizmo.Draw.LineCircle( 0, 4f );
				Gizmo.Draw.Color = Gizmo.Colors.Green;

				placeOffsetPos = placePos + tr.Normal * placeDeltaDistance;
				if ( useSurfaceNormal )
				{
					normalRot = Rotation.LookAt( tr.Normal );
				}
				else
				{
					normalRot = Angles.Zero;
				}


				Gizmo.Transform = new Transform( placeOffsetPos, Rotation.LookAt( tr.Normal ) );
				Gizmo.Draw.LineBBox( BBox.FromPositionAndSize( 0, 4f ) );
				if ( Gizmo.HasClicked ) AddObject(prefabItem);

			}
		}

		
	}


}
