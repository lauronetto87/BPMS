using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockSuiteUserService : ISuiteUserService
    {
        private async Task<IList<SuiteUserViewModel>> List(SuiteUserListFilter suiteUserListFilter)
        {
            var userListMock = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel() { Id = 1, Name = "Luis", Admin = true, Tenant = 55, Timezone = -3, Mail = "lauro.netto@selbetti.com.br" },
                new SuiteUserViewModel() { Id = 2, Name = "Fernando", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 3, Name = "Bertucci", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 4, Name = "Cassio", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 5, Name = "Cadore", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 6, Name = "Benjamim", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 7, Name = "Jose", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 8, Name = "Anversa", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 9, Name = "Caio", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 10, Name = "Mayer", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 11, Name = "Domingos", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 12, Name = "Antonio", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 13, Name = "Ricardi", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 14, Name = "Neto", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 15, Name = "Rafael", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 16, Name = "Brych", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 17, Name = "Andre", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 18, Name = "Vieira", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 19, Name = "Bruno", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 20, Name = "Jonatam", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 21, Name = "Garcia", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 22, Name = "Vinicius", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 23, Name = "Medeiros", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 24, Name = "Pagung", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 25, Name = "Arthur", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 26, Name = "Joaquim", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 27, Name = "Bruna", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 28, Name = "Ricardo", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 29, Name = "João" , Admin = true, Tenant = 55, Timezone = -3},
                new SuiteUserViewModel() { Id = 30, Name = "Daniel", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 31, Name = "Drews", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 32, Name = "Henrique", Admin = true, Tenant = 55, Timezone = -3 },
                new SuiteUserViewModel() { Id = 33, Name = "Erli", Admin = true, Tenant = 55, Timezone = -3, Mail = "erli.soares@selbetti.com.br" },
            };
            if (suiteUserListFilter.InUserIds == null) return await Task.FromResult(userListMock);
            return await Task.FromResult(userListMock.Where(x => suiteUserListFilter.InUserIds.Contains(x.Id)).ToList());
        }

        public async Task<IList<SuiteUserViewModel>> ListWithContext(SuiteUserListFilter suiteUserListFilter)
        {
            return await List(suiteUserListFilter);
        }

        public async Task<IList<SuiteUserViewModel>> ListWithoutContext(SuiteUserListFilter suiteUserListFilter)
        {
            return await List(suiteUserListFilter);
        }
    }
}
