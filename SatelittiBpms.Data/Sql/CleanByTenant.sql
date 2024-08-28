--UTILIZAR PARA LIMPAR O BANCO DE DADOS DE APENAS 1 TENANT--

SET @tenantId = <TENANT_ID>;

delete from bpms.FieldValues where TenantId = @tenantId;
delete from bpms.FlowPaths where TenantId = @tenantId;
delete from bpms.Flows where TenantId = @tenantId;
delete from bpms.ActivityNotifications where TenantId = @tenantId;
delete from bpms.ActivityUsersOptions where TenantId = @tenantId;
delete from bpms.ActivityUsers where TenantId = @tenantId;
delete from bpms.ActivityFields where TenantId = @tenantId;
delete from bpms.Fields where TenantId = @tenantId;
delete from bpms.Activities where TenantId = @tenantId;
delete from bpms.TaskHistories where TenantId = @tenantId;
delete from bpms.Tasks where TenantId = @tenantId;
delete from bpms.ProcessRoles where TenantId = @tenantId;
delete from bpms.RoleUsers where TenantId = @tenantId;
delete from bpms.Roles where TenantId = @tenantId;
delete from bpms.Users where TenantId = @tenantId;
delete from bpms.ProcessVersions where TenantId = @tenantId;
delete from bpms.Processes where TenantId = @tenantId;
delete from bpms.Tenants where Id = @tenantId;