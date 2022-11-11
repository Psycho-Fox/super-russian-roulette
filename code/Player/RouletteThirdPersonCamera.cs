using Sandbox;

namespace Roulette;
// Inspired by classic Torque engine games.
// References used: 
// https://asset.party/api/Sandbox.ThirdPersonCamera
// base/code/Camera/ThirdPersonCamera.cs
// base/code/Camera/FirstPersonCamera.cs
// Facepunch/sbox-forsaken/code/player/TopDownCamera.cs
public partial class RouletteThirdPersonCamera : CameraMode
{
	Vector3 lastPos;

	public override void Activated() // POLISH-TODO: Sliding in/out of head animation.
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Position = pawn.EyePosition;
		Rotation = pawn.EyeRotation;

		lastPos = Position;
	}

	public override void Update()
	{
		if ( Local.Pawn is not AnimatedEntity pawn )
			return;

		Position = pawn.Position;
		Vector3 targetPos;
		
		var center = pawn.Position + Vector3.Up * 64;

		Position = center;
		Rotation = Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

		float distance = 160.0f * pawn.Scale;
		targetPos = Position + Input.Rotation.Right * pawn.Scale;
		targetPos += Input.Rotation.Forward * -distance;

		var eyePos = pawn.EyePosition;

		//if ( thirdperson_collision )
		//{
			var tr = Trace.Ray( Position, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( pawn )
				.Radius( 8 )
				.Run();

			Position = tr.EndPosition;
		/*}
		else
		{
			Position = targetPos;
		}*/

		Rotation = pawn.EyeRotation;

		Viewer = null;
		lastPos = Position;
	}
}
