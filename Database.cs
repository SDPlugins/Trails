using MySql.Data.MySqlClient;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trails
{
	public class Database
	{

		public Database ()
		{
			new I18N.West.CP1250 ();
			CheckTrailsTable ();
		}

		private MySqlConnection createConnection ()
		{
			MySqlConnection connection = null;
			try
			{
				if (Trails.Instance.Configuration.Instance.port == 0) Trails.Instance.Configuration.Instance.port = 3306;
				connection = new MySqlConnection (String.Format ("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", Trails.Instance.Configuration.Instance.address, Trails.Instance.Configuration.Instance.name, Trails.Instance.Configuration.Instance.username, Trails.Instance.Configuration.Instance.password, Trails.Instance.Configuration.Instance.port));
			}
			catch (Exception ex)
			{
				Logger.LogException (ex);
			}
			return connection;
		}
		public void addToSQL (UnturnedPlayer player, List <ushort> trails)
		{
			if (TableLoaded)
			{
				MySqlConnection connection = createConnection ();
				MySqlCommand command = connection.CreateCommand ();
				command.Parameters.AddWithValue ("@SteamID", player.CSteamID);
				command.Parameters.AddWithValue ("@Trail", String.Join (",", trails.Select (t => t.ToString ()).ToArray ()));
				command.CommandText = "SELECT * FROM `"+ Trails.Instance.Configuration.Instance.tablename +"` WHERE SteamID=@SteamID;";
				connection.Open ();
				object result = command.ExecuteScalar ();
				if (result == null)
					command.CommandText = "INSERT INTO `"+ Trails.Instance.Configuration.Instance.tablename +"` (SteamID, Trail) VALUES (@SteamID, @Trail);";
				else
					command.CommandText = "UPDATE `"+ Trails.Instance.Configuration.Instance.tablename +"` SET Trail=@Trail WHERE SteamID=@SteamID LIMIT 1;";
				command.ExecuteNonQuery ();
				connection.Close ();
			}
		}
		public void removeAll (UnturnedPlayer player)
		{
			if (TableLoaded)
			{
				MySqlConnection connection = createConnection ();
				MySqlCommand command = connection.CreateCommand ();
				command.Parameters.AddWithValue ("@SteamID", player.CSteamID);
				command.CommandText = "DELETE FROM `" + Trails.Instance.Configuration.Instance.tablename + "` WHERE SteamID=@SteamID;";
				connection.Open ();
				command.ExecuteNonQuery ();
				connection.Close ();
			}
		}
		public List <ushort> getTrails (UnturnedPlayer player)
		{
			if (TableLoaded)
			{
				MySqlConnection connection = createConnection ();
				MySqlCommand command = connection.CreateCommand ();
				command.Parameters.AddWithValue ("@SteamID", player.CSteamID);
				command.CommandText = "SELECT Trail FROM `" + Trails.Instance.Configuration.Instance.tablename + "` WHERE SteamID=@SteamID;";
				connection.Open ();
				object result = command.ExecuteScalar ();
				connection.Close ();
				if (result != null)
					return result.ToString ().Split (',').Select (t => ushort.Parse (t)).ToList ();
			}
			return null;
		}
		#region Check Schemas

		private bool TableLoaded = false;
		public void CheckTrailsTable ()
		{
			try
			{
				Logger.Log ("Checking the " + Trails.Instance.Configuration.Instance.tablename + " Table...");
				MySqlConnection connection = createConnection ();
				MySqlCommand command = connection.CreateCommand ();
				command.CommandText = "show tables like '" + Trails.Instance.Configuration.Instance.tablename + "'";
				connection.Open ();
				object test = command.ExecuteScalar ();

				if (test == null)
				{
					Logger.Log ("Creating " + Trails.Instance.Configuration.Instance.tablename + " Table...");
					command.CommandText = "CREATE TABLE `" + Trails.Instance.Configuration.Instance.tablename + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`SteamID` VARCHAR(18) NOT NULL,`Trail` VARCHAR(3000) NOT NULL,PRIMARY KEY (`id`));";
					command.ExecuteNonQuery ();
					Logger.Log ("" + Trails.Instance.Configuration.Instance.tablename + " Table Successfully created...");
				}
				connection.Close ();
				Logger.Log ("" + Trails.Instance.Configuration.Instance.tablename + " Table Schema successfully validated...");
				TableLoaded = true;
			}
			catch (Exception ex)
			{
				Logger.LogException (ex);
			}
		}
		#endregion
	}
}
