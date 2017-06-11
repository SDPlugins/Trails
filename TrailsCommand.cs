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
	public class TrailsCommand : IRocketCommand
	{
		public AllowedCaller AllowedCaller => AllowedCaller.Both;

		public string Name => "trails";

		public List<string> Aliases => new List<string> () { };

		public string Help => "";

		public string Syntax => "";

		public void Execute (IRocketPlayer caller, string [] command)
		{
			UnturnedPlayer player = (UnturnedPlayer)caller;

			string trails = String.Join (",", Trails.Instance.Configuration.Instance.customTrails.Where (t => player.HasPermission (t.permission.ToLower ())).Select (t => t.name).ToArray ());

			UnturnedChat.Say (player, Trails.Instance.Translate ("trails_list", Trails.Instance.Configuration.Instance.customTrails.Count, trails), Color.green);
		}

		public List<string> Permissions => new List<string> () { "trails" };
	}
}
