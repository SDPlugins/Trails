using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Trails
{
	public class TrailCommand : IRocketCommand
	{
		public AllowedCaller AllowedCaller =>  AllowedCaller.Both;

		public string Name => "trail";

		public List<string> Aliases => new List<string> () { };

		public string Help => "";

		public string Syntax => "";

		public void Execute (IRocketPlayer caller, string [] command)
		{
			UnturnedPlayer player = (UnturnedPlayer)caller;
			if (command.Length > 0)
			{
				var trail = Trails.Instance.Configuration.Instance.customTrails.Where (t => t.name.ToLower ().Contains (command [1].ToLower ())).FirstOrDefault ();

				if (trail == null)
				{
					UnturnedChat.Say (player, Trails.Instance.Translate ("trail_not_found", command [1]), Color.red);
					return;
				}

				switch (command [0].ToLower ())
				{
					case "add":
					case "a":
						if (command.Length == 3)
						{
							if (player.HasPermission ("trail.player.force"))
							{
								UnturnedPlayer setPlayer = UnturnedPlayer.FromName (command [2]);
								if (setPlayer != null)
								{
									if (Trails.trails.ContainsKey ((ulong)setPlayer.CSteamID))
										Trails.trails [(ulong)setPlayer.CSteamID].Add (trail.id);
									else
										Trails.trails.Add ((ulong)setPlayer.CSteamID, new List<ushort> () { trail.id });

									Trails.Instance.database.addToSQL (setPlayer, Trails.trails [(ulong)setPlayer.CSteamID]);
									UnturnedChat.Say (setPlayer, Trails.Instance.Translate ("set_trail_by_admin", trail.name, player.DisplayName), Color.green);
									UnturnedChat.Say (player, Trails.Instance.Translate ("set_trail_admin", setPlayer.DisplayName, trail.name), Color.green);
								}
								else
									UnturnedChat.Say (player, Trails.Instance.Translate ("player_not_found", command [1]), Color.red);
							}
							else
								UnturnedChat.Say (player, Trails.Instance.Translate ("no_force_permission"), Color.red);
						}
						else
						{
							if (player.HasPermission (trail.permission.ToLower ()))
							{
								if (Trails.trails.ContainsKey ((ulong)player.CSteamID))
									Trails.trails [(ulong)player.CSteamID].Add (trail.id);
								else
									Trails.trails.Add ((ulong)player.CSteamID, new List<ushort> () { trail.id });

								Trails.Instance.database.addToSQL (player, Trails.trails [(ulong)player.CSteamID]);
								UnturnedChat.Say (player, Trails.Instance.Translate ("set_trail", trail.name), Color.green);
							}
							else
								UnturnedChat.Say (player, Trails.Instance.Translate ("no_permission", trail.name), Color.red);
						}
						break;
					case "remove":
					case "r":
						if (command.Length == 2)
						{
							if (command [1] == "*")
							{
								if (Trails.trails.ContainsKey ((ulong)player.CSteamID))
									Trails.trails.Remove ((ulong)player.CSteamID);

								Trails.Instance.database.removeAll (player);
								UnturnedChat.Say (player, Trails.Instance.Translate ("removed_all_trails"), Color.green);
							}
							else
							{
								if (Trails.trails.ContainsKey ((ulong)player.CSteamID))
									if (Trails.trails [(ulong)player.CSteamID].Contains (trail.id))
										Trails.trails.Remove (trail.id);

								if (Trails.trails [(ulong)player.CSteamID].Count > 0)
									Trails.Instance.database.addToSQL (player, Trails.trails [(ulong)player.CSteamID]);
								else
									Trails.Instance.database.removeAll (player);
								UnturnedChat.Say (player, Trails.Instance.Translate ("removed_trail"), Color.green);
							}
						}
						break;
					case "help":
					case "h":
						if (command.Length >= 2)
						{
							switch (command [1].ToLower ())
							{
								case "add":
								case "a":
									UnturnedChat.Say (player, "/trail add <trail name> [player name]", Color.green);
									break;
								case "remove":
								case "r":
									UnturnedChat.Say (player, "/trail remove <* | trail name>", Color.green);
									break;
								default:
									UnturnedChat.Say (player, "Invalid Format: /trail help <add | remove>", Color.red);
									break;
							}
						}
						else
							UnturnedChat.Say (player, "Invalid Format: /trail help <add | remove>", Color.red);
						break;
					default:
						UnturnedChat.Say (player, "Invalid Format: /trail <add | remove | help>", Color.red);
						break;
				}
			}
			else
			{
				if (Trails.trails.ContainsKey ((ulong)player.CSteamID))
					Trails.trails.Remove ((ulong)player.CSteamID);

				Trails.Instance.database.removeAll (player);
				UnturnedChat.Say (player, Trails.Instance.Translate ("removed_all_trails"), Color.green);
			}
		}

		public List<string> Permissions => new List<string> () { "trail" };
	}
}
