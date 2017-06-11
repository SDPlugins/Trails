using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Serialisation;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace Trails
{
	public class Trails : RocketPlugin<TrailsConfiguration>
	{
		public static Trails Instance;
		public static Dictionary<ulong, List <ushort>> trails;
		public Database database;

		#region Translations
		public override TranslationList DefaultTranslations
		{
			get
			{
				return new TranslationList ()
				{
					{ "no_permission", "You don't have permission to set your trail to {0}" },
					{ "set_trail", "Your trail has been set to {0}" },
					{ "removed_trail", "Removed {0} from your trails" },
					{ "removed_all_trails", "Your trails has been removed" },
					{ "trail_not_found", "{0} is not a valid trail, try /trails for a list of trails" },
					{ "trails_list", "{0} trails found : {1}"},

					{ "set_trail_admin", "You set {0}'s trail to {1}"},
					{ "set_trail_by_admin", "Your trail has been set to {0} by {1}"},
					{ "player_not_found", "Player by the name {0} wasn't found"},
					{ "no_force_permission", "You don't have permission to set a players trail"},

					{ "no_permission_custom", "You don't have permission to set custom trails"}
				};
			}
		}
		#endregion

		protected override void Load ()
		{
			Instance = this;
			database = new Database ();
			trails = new Dictionary<ulong, List <ushort>> ();
			UnturnedPlayerEvents.OnPlayerUpdatePosition += PlayerMoved;

			U.Events.OnPlayerConnected += playerConnected;
			U.Events.OnPlayerDisconnected += playedDisconnected;

			Configuration.Save ();
		}

		private void playerConnected (UnturnedPlayer player)
		{
			var loadedTrails = database.getTrails (player);
			if (loadedTrails != null)
			{
				trails.Add ((ulong)player.CSteamID, loadedTrails);
			}
		}

		private void playedDisconnected (UnturnedPlayer player)
		{
			if (trails.ContainsKey ((ulong)player.CSteamID))
				trails.Remove ((ulong)player.CSteamID);
		}

		private void PlayerMoved (UnturnedPlayer player, UnityEngine.Vector3 position)
		{
			if (!trails.ContainsKey ((ulong)player.CSteamID))
				return;
			foreach (var trail in trails [(ulong)player.CSteamID])
			{
				var trailItem = Configuration.Instance.customTrails.Where (t => t.id == trail).FirstOrDefault ();

				string [] showOn = trailItem.type.Split (',');

				foreach (var show in showOn)
				{
					switch (show.Split ('.') [0].ToLower ())
					{
						case "always":
							break;
						case "grounded":
							if (!player.Player.movement.isGrounded)
								return;
							break;
						case "notgrounded":
							if (player.Player.movement.isGrounded)
								return;
							break;
						case "jump":
							if (!player.Player.movement.jump)
								return;
							break;
						case "bleeding":
							if (!player.Bleeding)
								return;
							break;
						case "brokenbones":
							if (!player.Broken)
								return;
							break;
						case "zombiefollowing":
							if (!player.Player.life.isAggressor)
								return;
							break;
						case "equipedgun":
							if (player.Player.equipment.asset == null)
								return;
							ushort [] guns = show.Split ('.').Select (g => ushort.Parse (g)).ToArray ();
							if (!guns.Contains (player.Player.equipment.asset.id))
								return;
							break;
						case "inwater":
							if (!player.Player.stance.isSubmerged)
								return;
							break;
						case "running":
							if (!player.Player.stance.sprint)
								return;
							break;
						case "walking":
							if (player.Player.stance.sprint || player.Player.stance.crouch || player.Player.stance.prone)
								return;
							break;
						case "prone":
							if (!player.Player.stance.prone)
								return;
							break;
						case "crouch":
							if (!player.Player.stance.crouch)
								return;
							break;
						case "talking":
							if (!player.Player.voice.isTalking)
								return;
							break;
						case "invehicle":
							if (!player.IsInVehicle)
								return;
							ushort [] vehicles = show.Split ('.').Select (v => ushort.Parse (v)).ToArray ();
							if (!vehicles.Contains (player.CurrentVehicle.id))
								return;
							break;
						default:
							break;
					}
				}

				EffectManager.sendEffect (trail, 80, position);
			}
		}

		protected override void Unload ()
		{
			UnturnedPlayerEvents.OnPlayerUpdatePosition -= PlayerMoved;
		}
	}
}
