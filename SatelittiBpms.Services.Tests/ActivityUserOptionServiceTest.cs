using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class ActivityUserOptionServiceTest
    {
        Mock<IActivityUserOptionService> _mockActivityUserOptionService;
        Mock<IActivityUserOptionRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IActivityUserOptionRepository>();
            _mockActivityUserOptionService = new Mock<IActivityUserOptionService>();
        }

        [Test]
        public async Task ensureThatListWithListActivitiesIDSuccess()
        {
            List<ActivityUserOptionInfo> lstOptions = new List<ActivityUserOptionInfo>();

            List<int> lstActivitiesId = new List<int>();
            lstActivitiesId.Add(1);
            lstActivitiesId.Add(2);
            lstActivitiesId.Add(3);

            ActivityUserOptionInfo option1 = new ActivityUserOptionInfo()
            {
                TenantId = 55,
                ActivityUserId = 1,
                Description = "Decisão 1",
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Atividade1"
                    }
                }
            };
            ActivityUserOptionInfo option2 = new ActivityUserOptionInfo()
            {
                TenantId = 55,
                ActivityUserId = 2,
                Description = "Decisão 2",
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Atividade2"
                    }
                }
            };
            ActivityUserOptionInfo option3 = new ActivityUserOptionInfo()
            {
                TenantId = 55,
                ActivityUserId = 3,
                Description = "Decisão 3",
                ActivityUser = new ActivityUserInfo()
                {
                    Activity = new ActivityInfo()
                    {
                        ComponentInternalId = "Atividade3"
                    }
                }
            };

            lstOptions.Add(option1);
            lstOptions.Add(option2);
            lstOptions.Add(option3);

            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(lstOptions);

            ActivityUserOptionService activityService = new ActivityUserOptionService(_mockRepository.Object, _mockMapper.Object);
            var result = await activityService.ListByUserActivityId(lstActivitiesId);

            Assert.AreEqual(3, result.Count);
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }
    }
}
