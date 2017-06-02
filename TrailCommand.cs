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
		public AllowedCaller AllowedCaller
		{
			get
			{
				return AllowedCaller.Both;
			}
		}

		public string Name
		{
			get
			{
				return "trail";
			}
		}

		public List<string> Aliases
		{
			get
			{
				return new List<string> () { };
			}
		}

		public string Help
		{
			get
			{
				return "";
			}
		}

		public string Syntax
		{
			get
			{
				return "";
			}
		}

		public void Execute (IRocketPlayer caller, string [] command)
		{
			UnturnedPlayer player = (UnturnedPlayer)caller;
			if (command.Length > 0)
			{
				var trail = Trails.Instance.Configuration.Instance.customTrails.Where (t => t.name.ToLower ().Contains (command [0].ToLower ())).FirstOrDefault ();

				if (command.Length == 2)
				{
					if (player.HasPermission ("trail.player.force"))
					{
						UnturnedPlayer setPlayer = UnturnedPlayer.FromName (command [1]);
						if (setPlayer != null)
						{
							Trails.Instance.database.addToSQL (setPlayer, trail.id);
							if (Trails.trails.ContainsKey (setPlayer.CSteamID))
								Trails.trails [setPlayer.CSteamID] = trail.id;
							else
								Trails.trails.Add (setPlayer.CSteamID, trail.id);

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
					if (trail != null)
					{
						if (player.HasPermission (trail.permission.ToLower ()))
						{
							Trails.Instance.database.addToSQL (player, trail.id);
							if (Trails.trails.ContainsKey (player.CSteamID))
								Trails.trails [player.CSteamID] = trail.id;
							else
								Trails.trails.Add (player.CSteamID, trail.id);
							UnturnedChat.Say (player, Trails.Instance.Translate ("set_trail", trail.name), Color.green);
						}
						else
							UnturnedChat.Say (player, Trails.Instance.Translate ("no_permission", trail.name), Color.red);
					}
					else
						UnturnedChat.Say (player, Trails.Instance.Translate ("trail_not_found", command [0]), Color.red);
				}
			}
			else
			{
				Trails.Instance.database.remove (player);
				if (Trails.trails.ContainsKey (player.CSteamID))
					Trails.trails.Remove (player.CSteamID);
				UnturnedChat.Say (player, Trails.Instance.Translate ("removed_trail"), Color.green);
			}
		}

		public List<string> Permissions
		{
			get
			{
				return new List<string> () { "trail" };
			}
		}

	}
}
