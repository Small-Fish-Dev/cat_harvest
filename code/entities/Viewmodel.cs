using Sandbox;
using System;
using System.Linq;

namespace Sandbox.entities
{

	public class HarvestViewModel : BaseViewModel
	{

		float walkBob = 0f;

		public override void PostCameraSetup( ref CameraSetup camSetup )
		{

			base.PostCameraSetup( ref camSetup );

			if ( !Local.Pawn.IsValid() ) { return; }

			Entity ply = Local.Pawn;

			Position = ply.EyePosition - ply.EyeRotation.Forward * 20;
			Rotation = ply.EyeRotation;

			// Thank you DM98 for this piece of code
			var speed = ply.Velocity.Length / 300f;
			var left = camSetup.Rotation.Left;
			var up = camSetup.Rotation.Up;

			if ( ply.GroundEntity != null )
			{
				walkBob += Time.Delta * 25.0f * speed;
			}

			Position += up * MathF.Sin( walkBob ) * speed * -1;
			Position += left * MathF.Sin( walkBob * 0.6f ) * speed * -0.5f;

			camSetup.ViewModel.FieldOfView = 70f;

		}


	}

}
