﻿using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownCrier.Modules
{
	[Group("servers"), Alias("s", "server")]
	public class ServersModule : CrierModuleBase
	{
		public enum Map
		{
			Town,
			Tutorial,
			TestZone
		}

		[Command(), Alias("online")]
		public async Task Online()
		{
			IEnumerable<GameServerInfo> servers = await AltaAPI.ApiClient.ServerClient.GetOnlineServersAsync();

			StringBuilder response = new StringBuilder();

			response.AppendLine("The following servers are online:");

			foreach (GameServerInfo server in servers)
			{
				response.AppendFormat("{0} - {3} - {1} player{2} online\n",
					server.Name,
					server.OnlinePlayers.Count,
					server.OnlinePlayers.Count == 1 ? "" : "s",
					(Map)server.SceneIndex);
			}

			await ReplyMentionAsync(response.ToString());
		}

		[Command("players"), Alias("player", "p")]
		public async Task Players([Remainder]string serverName)
		{
			serverName = serverName.ToLower();

			IEnumerable<GameServerInfo> servers = await AltaAPI.ApiClient.ServerClient.GetOnlineServersAsync();

			StringBuilder response = new StringBuilder();

			response.AppendLine("Did you mean one of these?");

			foreach (GameServerInfo server in servers)
			{
				response.AppendLine(server.Name);

				if (Regex.Match(server.Name, @"\b" + serverName + @"\b", RegexOptions.IgnoreCase).Success)
				{
					response.Clear();

					if (server.OnlinePlayers.Count > 1)
					{
						response.AppendFormat("These players are online on {0}\n", server.Name);

						foreach (UserInfo user in server.OnlinePlayers)
						{
							MembershipStatusResponse membershipResponse = await AltaAPI.ApiClient.UserClient.GetMembershipStatus(user.Identifier);

							response.AppendFormat("- {1}{0}\n", user.Username, membershipResponse.IsMember ? "<:Supporter:547252984481054733> " : "");
						}
					}
					else if (server.OnlinePlayers.Count == 1)
					{
						response.AppendFormat("Only {0} is on {1}", server.OnlinePlayers.First().Username, server.Name);
					}
					else
					{
						response.AppendFormat("Nobody is on {0}", server.Name);
					}

					break;
				}
			}

			await ReplyMentionAsync(response.ToString());
		}
	}
}