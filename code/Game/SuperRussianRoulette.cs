//Disclaimer: this is my first time using C#, I'm a C programmer.
//I'm using some code from facepunch, hope that's ok. 
//Esp since the API reference is shit rn.
//Abandon hope all ye who enter here.

using Sandbox;
using Sandbox.Internal;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>

public partial class SuperRussianRoulette : Game
{
	//public static new SuperRussianRoulette Current => Game.Current as SuperRussianRoulette;

	public SuperRussianRoulette()
	{

	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{

		base.ClientJoined( client );
		// Create a player entity for each client.
		var pawn = new RoulettePlayer( client );
		client.Pawn = pawn;
		pawn.Spawn();

		// Note: Remove this, it's fucking pointless.
		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position += Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}

		client.SetInt( "myass", 0 );
		ClientLog( client );
	}

	private static void ClientLog(Client cl)
	{
		//Log.Info( $"Player connected: {cl.Name}" );
		Log.Info( $"Current GameState is: {CurrentState}" );
		Log.Info( $"Players Connected: {All.OfType<RoulettePlayer>().Count()}" );
	}
	
}
