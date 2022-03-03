using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xbim.Flex.Client;
using Xbim.Flex.DI.Extensions.Services;

namespace Xbim.Demo.FexillonTwin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpatialController : ControllerBase
    {
        private readonly ILogger<SpatialController> _logger;
        private readonly IFlexClientsProvider flexClientsProvider;
        private readonly IFlexAdmin admin;
        private readonly IFlexAdminClientsProvider flexAdminClientsProvider;

        public SpatialController(ILogger<SpatialController> logger, IFlexClientsProvider flexClientsProvider, IFlexAdmin admin, IFlexAdminClientsProvider flexAdminClientsProvider)
        {
            _logger = logger;
            this.flexClientsProvider = flexClientsProvider;
            this.admin = admin;
            this.flexAdminClientsProvider = flexAdminClientsProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<Space>> Get()
        {

            var flexIdentityApi = await flexAdminClientsProvider.GetFlexIdentityAPIAsync();

            var tenants = await flexIdentityApi.Tenants_GetAsync();


            var flexApi = await flexAdminClientsProvider.GetFlexAIMAPIAsync();


            var result = await flexApi.Spaces_GetAsync("microsoft", Region.Sandbox);

            return result.Value;
            
        }
    }
}
