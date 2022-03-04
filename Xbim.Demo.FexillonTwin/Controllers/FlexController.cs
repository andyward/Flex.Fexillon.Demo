using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xbim.Flex.Client;
using Xbim.Flex.DI.Extensions.Services;
using Xbim.Flex.Identity.Client;

namespace Xbim.Demo.FexillonTwin.Controllers
{

    /// <summary>
    /// Controller for accessing Flex services via the OpenAPI
    /// </summary>
    /// <remarks>This employs a 'master' admin account for the application. This gives 'root' access across the owned tenants (i.e. Fexillon)
    /// To provide more fine grained permissions we'd use the IFlexClientsProvider instead, and integrate the SSO authentication feature.
    /// Important: guard the ClientSecret appropriately to avoid security issues.
    /// </remarks>
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class FlexController : ControllerBase
    {
        private const int DefaultPageSize = 10;                 // Keeping a low page size to keep Swagger UI performant - can be up to 1000
        private const Region DefaultRegion = Region.Sandbox;    // Normally this is read dynamically from the tenant, bur hardwiring for simplicity
        private readonly ILogger<FlexController> _logger;
        private readonly IFlexAdminClientsProvider flexAdminClientsProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="flexAdminClientsProvider"></param>
        public FlexController(ILogger<FlexController> logger, IFlexAdminClientsProvider flexAdminClientsProvider)
        {
            _logger = logger;

            this.flexAdminClientsProvider = flexAdminClientsProvider;
        }

        /// <summary>
        /// Lists the tenants available to this application
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(ODataTenantList))]
        [Route("Tenants")]
        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            // Get the client for the Flex Identity API - configured with the master access token
            IIdentityAPI flexIdentityApi = await GetFlexIdentityApi();

            // Get fisrt 10 visible tenants, sorted by Name
            var tenants = await flexIdentityApi.Tenants_GetAsync(top: DefaultPageSize, count: true, orderby: "Name");

            // Here you might map to your data structure / DTOs
            return Ok(tenants);

        }


        /// <summary>
        /// List all assets in this Tenant, and their models
        /// </summary>
        /// <param name="tenantId">The tenant</param>
        /// <returns></returns>
        [Produces(typeof(ODataListResponseOfAsset))]
        [Route("Tenants/{tenantId}/Assets")]
        [HttpGet]
        public async Task<IActionResult> GetAssets(string tenantId = "chi")
        {

            IFlexAPI flexApi = await GetFlexApi();

            var result = await flexApi.Assets_GetAsync(tenantId, DefaultRegion, top: DefaultPageSize, count: true, expand: "Models", orderby:"DateCreated desc");

            return Ok(result);

        }

        /// <summary>
        /// Gets the levels in this tenant
        /// </summary>
        /// <param name="tenantId">The tenant</param>
        /// <returns></returns>
        [Produces(typeof(ODataListResponseOfLevel))]
        [Route("Tenants/{tenantId}/Levels")]
        [HttpGet]
        public async Task<IActionResult> GetLevels(string tenantId = "chi")
        {

            IFlexAPI flexApi = await GetFlexApi();

            var result = await flexApi.Levels_GetAsync(tenantId, DefaultRegion, top: DefaultPageSize, count: true, expand: "Spaces");

            return Ok(result);

        }

        /// <summary>
        /// Get the spaces in this tenant
        /// </summary>
        /// <param name="tenantId">The tenant</param>
        /// <param name="assetId">The optional assetId</param>
        /// <returns></returns>
        [Produces(typeof(ODataListResponseOfSpace))]
        [Route("Tenants/{tenantId}/Spaces")]
        [HttpGet]
        public async Task<IActionResult> GetSpaces(string tenantId = "chi", [FromQuery] int? assetId = null)
        {

            IFlexAPI flexApi = await GetFlexApi();

            // Build a simple oData filter dynamically
            var filter = assetId.HasValue ? $"Model/AssetId eq {assetId.Value}" : null;

            // Note we expand the attributes and parent model
            var result = await flexApi.Spaces_GetAsync(tenantId, DefaultRegion, filter: filter, top: DefaultPageSize, count: true, expand:"Attributes,Model");

            return Ok(result);
            
        }

        /// <summary>
        /// Get the components within the volume of a space
        /// </summary>
        /// <remarks>This works across models in the same asset</remarks>
        /// <param name="tenantId">The tenant</param>
        /// <param name="assetModelId">The assetModel Id of the space</param>
        /// <param name="entityId">The entity Id of the space</param>
        /// <returns></returns>
        [Produces(typeof(ODataListResponseOfComponent))]
        [Route("Tenants/{tenantId}/ComponentsInVolume")]
        [HttpGet]
        public async Task<IActionResult> GetCompenentsInSpace(string tenantId = "chi", [FromQuery] int assetModelId = 47040, [FromQuery] int entityId = 276)
        {
            IFlexAPI flexApi = await GetFlexApi();

            var result = await flexApi.SpacesContainedComponents_GetAsync(assetModelId, entityId, tenantId, DefaultRegion, top: DefaultPageSize, count: true, inSameAsset: true);

            return Ok(result);

        }

        // Gets the Flex Identity API with appropriate access token applied
        private async Task<IIdentityAPI> GetFlexIdentityApi()
        {
            return await flexAdminClientsProvider.GetFlexIdentityAPIAsync();
        }

        // Gets the Flex AIM API with appropriate access token applied
        private async Task<IFlexAPI> GetFlexApi()
        {
            return await flexAdminClientsProvider.GetFlexAIMAPIAsync();
        }
    }
}
