using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Xbim.Demo.FexillonTwin.Helpers
{
    public static class FlexServiceExtensions
    {
		/// <summary>
		/// Adds log-in services based on the configuration. It will either be
		/// an interactive log-in service or non-interactive service using local directory or ldap identity
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
				// spoof IAuthorizationService
				//services.AddSingleton<IAuthorizationService, NullAuthorizationService>();
			}
			else
			{
				//// this admin service will always throw as it is only available in the SSO/directory mode
				//services.AddSingleton<IFlexAdmin, NullFlexAdminService>();

			}

			//if (string.IsNullOrWhiteSpace(config["DirectoryMode"]) || IsOneTimeFlexLogIn())
			//{
			//	// using implicit Flex flow and identities
			//	services.AddSingleton<ILogInService, DefaultLoginService>();
			//	// using DefaultLoginService fo get authenticated clients
			//	services.ReplaceTransient<IFlexClientsProvider, DefaultFlexClientsProvider>();
			//}
			//else
			//{
			//	// directory based login service
			//	services.AddSingleton<ILogInService, DirectoryLogInService>();

			//}

			//if (string.Equals(config["DirectoryMode"] ?? string.Empty, "directory", StringComparison.OrdinalIgnoreCase))
			//{
			//	// replace profile service to use directory information fot the current user
			//	services.ReplaceTransient<IFlexUserProfileProvider, DirectoryFlexUserProfileProvider>();
			//}
			//else
			//{
			//	services.ReplaceTransient<IFlexUserProfileProvider, NullFlexUserProfileProvider>();
			//}

			return services;
		}
	}
}
