using System.Collections.Generic;
using System.Data.Entity;
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
		// GET /api/games
		public async Task<IEnumerable<Game>> Get()
		{
			using (var context = new GameCountContext())
			{
				var events = await context.Events
					.GroupBy(e => e.Game)
					.Select(eg => new Game
					{
						Id = eg.Key,
						Players = eg.Select(e => e.Player).Distinct().Count()
					})
					.ToListAsync();

				return events;
			}
		}

		// GET /api/games/gameId
		public async Task<IEnumerable<PlayerScore>> Get(string gameId)
		{
			using (var context = new GameCountContext())
			{
				var events = await context.Events
					.Where(e => e.Game == gameId)
					.GroupBy(e => e.Player)
					.Select(eg => new PlayerScore
					{
						Player = eg.Key,
						Score = eg.Sum(e => e.Count)
					})
					.OrderByDescending(s => s.Score)
					.ToListAsync();

				return events;
			}
		}

		// POST /api/games/gameId
		public async Task<IHttpActionResult> Post(string gameId, [FromBody]CountEvent countEvent)
		{
			using (var context = new GameCountContext())
			{
				var gameCountEvent = new GameCountEvent
				{
					Game = gameId,
					Player = countEvent.player,
					Count = countEvent.count
				};

				context.Events.Add(gameCountEvent);
				await context.SaveChangesAsync();
				return Ok();
			}
		}
	}

	public class Game
	{
		public string Id { get; set; }
		public int Players { get; set; }
	}

	public class PlayerScore
	{
		public string Player { get; set; }
		public int Score { get; set; }
	}

	public class CountEvent
	{
		public string player { get; set; }
		public int count { get; set; }
	}















	public class GameCountEvent
	{
		public long Id { get; set; }
		public string Game { get; set; }
		public string Player { get; set; }
		public int Count { get; set; }
	}

	public class GameCountContext : DbContext
	{
		public virtual DbSet<GameCountEvent> Events { get; set; }
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
