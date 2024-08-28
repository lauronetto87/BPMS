using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class RoleController : BpmsApiControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(
            ILogger<RoleController> logger,
            IRoleService roleService) : base(logger)
        {
            _roleService = roleService;
        }

        [HttpPost]
        [Route("verifyRoleNameExists")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> VerifyRoleNameExists([FromBody] JObject body)
        {
            int? roleId = null;
            if (body.ContainsKey("roleId"))
                roleId = Convert.ToInt32(body["roleId"]);
            return await HandleExceptionAsync(async () => await _roleService.VerifyRoleNameExists(body["roleName"].ToString(), roleId: roleId));
        }

        [HttpPost]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Insert([FromBody] RoleDTO role)
        {
            return await HandleExceptionAsync(async () => await _roleService.InsertWithRelationship(role));
        }

        [HttpGet]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> List()
        {
            return await HandleExceptionAsync(async () => await _roleService.ListByTenant());
        }

        [HttpGet]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        [Route("listConfig")]
        public async Task<ActionResult> ListConfig()
        {
            return await HandleExceptionAsync(async () => await _roleService.ListByTenantToConfig());
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Get(int id)
        {
            return await HandleExceptionAsync(async () => await _roleService.GetByTenant(id));
        }

        [HttpPut]
        [Route("delete")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Delete([FromBody] List<int> rolesToDelete)
        {
            return await HandleExceptionAsync(async () => await _roleService.DeleteMany(rolesToDelete));
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Insert([FromRoute] int id, [FromBody] RoleDTO role)
        {
            return await HandleExceptionAsync(async () => await _roleService.UpdateWithRelationship(id, role));
        }
    }
}
