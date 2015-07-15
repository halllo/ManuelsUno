using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

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
				"api/{controller}/{id}",
				new { id = RouteParameter.Optional });

			return config;
		}
	}





























	public class ValuesController : ApiController
	{
		// GET /api/values
		public async Task<IEnumerable<string>> Get()
		{
			return await Task.FromResult(Enumerable.Range(1, 10).Select(i => "value " + i));
		}

		// GET /api/values/5
		public async Task<string> Get(int id)
		{
			return await Task.FromResult("value request for " + id);
		}

		// POST /api/values
		public void Post(string value)
		{

		}
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
