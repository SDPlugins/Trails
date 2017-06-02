using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Trails
{
	public class TrailsConfiguration : IRocketPluginConfiguration
	{
		[XmlElement ("DatabaseAddress")]
		public string address;
		[XmlElement ("DatabaseName")]
		public string name;
		[XmlElement ("DatabaseUsername")]
		public string username;
		[XmlElement ("DatabasePassword")]
		public string password;
		[XmlElement ("DatabaseTableName")]
		public string tablename;
		[XmlElement ("DatabasePort")]
		public int port;

		[XmlArray ("Trails")]
		[XmlArrayItem ("Trail")]
		public List<customTrail> customTrails = new List<customTrail> ();

		public void LoadDefaults ()
		{
			address = "localhost";
			name = "unturned";
			username = "unturned";
			password = "password";
			tablename = "trails";
			port = 3306;

			customTrails = new List<customTrail> ()
			{
				new customTrail ()
				{
					name = "electric",
					id = 61,
					permission = "trail.electric"
				},
				new customTrail ()
				{
					name = "fire",
					id = 139,
					permission = "trail.fire"
				},
				new customTrail ()
				{
					name = "water",
					id = 129,
					permission = "trail.water"
				},
				new customTrail ()
				{
					name = "red",
					id = 124,
					permission = "trail.red"
				},
				new customTrail ()
				{
					name = "orange",
					id = 130,
					permission= "trail.orange"
				},
				new customTrail ()
				{
					name = "purple",
					id = 132,
					permission = "trail.purple"
				},
				new customTrail ()
				{
					name = "green",
					id = 134,
					permission = "trail.green"
				}
			};
		}
	}

	public class customTrail
	{
		[XmlElement ("Name")]
		public string name;
		[XmlElement ("ID")]
		public ushort id;
		[XmlElement ("Permission")]
		public string permission;

		public customTrail () { }
	}
}
