using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(ManuelsUno.Startup))]

namespace ManuelsUno
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR();
			app.UseWebApi(ConfigureWebApi());
		}

		private HttpConfiguration ConfigureWebApi()
		{
			var config = new HttpConfiguration();

			config.Routes.MapHttpRoute(
				"DefaultApi",
				"api/{controller}/{gameId}",
				new { gameId = RouteParameter.Optional });

			return config;
		}
	}





























	public class GamesController : ApiController
	{
		private static Dictionary<string, Dictionary<string, int>> games = new Dictionary<string, Dictionary<string, int>>();

		// GET /api/games
		public async Task<IEnumerable<string>> Get()
		{
			var result = games.Keys;

			return await Task.FromResult(result);
		}

		// GET /api/games/gameId
		public async Task<IEnumerable<string>> Get(string gameId)
		{
			var result = games[gameId].Select(kvp => kvp.Key + " has " + kvp.Value + " points");

			return await Task.FromResult(result);
		}

		// POST /api/games/gameId
		public IHttpActionResult Post(string gameId, [FromBody]CountEvent countEvent)
		{
			if (!games.ContainsKey(gameId))
			{
				games.Add(gameId, new Dictionary<string, int>());
			}
			if (!games[gameId].ContainsKey(countEvent.player))
			{
				games[gameId].Add(countEvent.player, 0);
			}
			games[gameId][countEvent.player] += countEvent.count;

			return Ok();
		}
	}

	public class CountEvent
	{
		public string player { get; set; }
		public int count { get; set; }
	}



























	public class MyHub : Hub
	{
		public void Send(string userName, string roomName, string message)
		{
			Clients.Group(roomName).addMessage(userName, message);
		}
		public async Task JoinRoom(string userName, string roomName)
		{
			await Groups.Add(Context.ConnectionId, roomName);
			Clients.Group(roomName).addMessage("", userName + " joined.");
		}
		public Task LeaveRoom(string roomName)
		{
			return Groups.Remove(Context.ConnectionId, roomName);
		}
		public override Task OnConnected()
		{
			return base.OnConnected();
		}
		public override Task OnDisconnected(bool stopCalled)
		{
			return base.OnDisconnected(stopCalled);
		}
	}
}
