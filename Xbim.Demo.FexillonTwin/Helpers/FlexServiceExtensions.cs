using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Xbim.Demo.FexillonTwin.Helpers
{
    public static class FlexServiceExtensions
    {
		/// <summary>
		/// Adds log-in services based on the configuration. It will either be
		/// an interactive log-in service or non-interactive service using local directory or AD identity
		/// </summary>
		/// <param name="config">Configuration</param>
		/// <param name="services">Services</param>
		public static IServiceCollection AddFlexLogInServices(this IServiceCollection services, IConfiguration config)
		{
			
			var flexSection = config.GetSection("Flex");
			if (flexSection != null)
			{
				services.AddXbimFlex(config)
					.AddAimAPI()
					.AddIdentityAPI();
				// spoof IAuthorizationService for SSO
				//services.AddSingleton<IAuthorizationService, NullAuthorizationService>();
			}
			else
			{
				// TODO revert to interactive Flex Flow
			}

			return services;
		}
	}
}
