using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{

	public class HarvestViewModel : BaseViewModel
	{

		public override void PostCameraSetup( ref CameraSetup camSetup )
		{
			base.PostCameraSetup( ref camSetup );

			if ( !Local.Pawn.IsValid() ) { return; }

			Position = camSetup.Position;
			Rotation = camSetup.Rotation;

			camSetup.ViewModel.FieldOfView = 30f;

		}

	}

}
