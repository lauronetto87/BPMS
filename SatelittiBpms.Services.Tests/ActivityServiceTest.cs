using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class ActivityServiceTest
    {
        Mock<IActivityRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IFieldService> _mockFieldService;
        Mock<IActivityFieldService> _mockActivityFieldService;
        Mock<IActivityUserService> _mockActivityUserService;
        Mock<IXmlDiagramService> _mockXmlDiagramService;
        Mock<IActivityNotificationService> _mockActivityNotificationService;


        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IActivityRepository>();
            _mockFieldService = new Mock<IFieldService>();
            _mockActivityFieldService = new Mock<IActivityFieldService>();
            _mockActivityUserService = new Mock<IActivityUserService>();
            _mockXmlDiagramService = new Mock<IXmlDiagramService>();
            _mockActivityNotificationService = new Mock<IActivityNotificationService>();
        }

        [Test]
        public async Task EnsureThatInsertManyDoNothingWhenActivitiesListIsNull()
        {
            ActivityService activityService = new ActivityService(_mockRepository.Object, _mockMapper.Object, _mockFieldService.Object, _mockActivityFieldService.Object, _mockActivityUserService.Object, _mockXmlDiagramService.Object, _mockActivityNotificationService.Object);
            await activityService.InsertMany(null, 1, 2);

            _mockRepository.Verify(x => x.Insert(It.IsAny<ActivityInfo>()), Times.Never());
        }

        [Test]
        public async Task EnsureThatInsertManySuccefullyWhenHaveOneActivityAndField()
        {
            var activitiesList = new List<ActivityDTO>()
            {
                new ActivityDTO(){
                    Fields = new List<ActivityFieldDTO>(){
                        new ActivityFieldDTO(){
                            FieldId = "field1"
                        }
                    }
                }
            };

            _mockRepository.Setup(x => x.Insert(It.IsAny<ActivityInfo>())).ReturnsAsync(3);
            _mockMapper.Setup(m => m.Map<FieldDTO>(It.IsAny<ActivityFieldDTO>())).Returns(new FieldDTO());
            _mockFieldService.Setup(x => x.Insert(It.IsAny<FieldDTO>())).ReturnsAsync(Result.Success(86));
            _mockActivityFieldService.Setup(x => x.Insert(It.IsAny<ActivityFieldDTO>()));

            ActivityService activityService = new ActivityService(_mockRepository.Object, _mockMapper.Object, _mockFieldService.Object, _mockActivityFieldService.Object, _mockActivityUserService.Object, _mockXmlDiagramService.Object, _mockActivityNotificationService.Object);
            await activityService.InsertMany(activitiesList, 1, 2);

            _mockRepository.Verify(x => x.Insert(It.IsAny<ActivityInfo>()), Times.Once());
            _mockFieldService.Verify(x => x.Insert(It.IsAny<FieldDTO>()), Times.Once());
            _mockActivityFieldService.Verify(x => x.Insert(It.IsAny<ActivityFieldDTO>()), Times.Once());
        }

        [Test]
        public async Task EnsureThatInsertManyNotDuplicateFieldWhenHaveMoreThenOneActivity()
        {
            var activitiesList = new List<ActivityDTO>()
            {
                new ActivityDTO(){
                    Fields = new List<ActivityFieldDTO>(){
                        new ActivityFieldDTO(){
                            FieldId = "field1"
                        }
                    }
                },
                new ActivityDTO(){
                    Fields = new List<ActivityFieldDTO>(){
                        new ActivityFieldDTO(){
                            FieldId = "field1"
                        }
                    }
                }
            };

            _mockRepository.Setup(x => x.Insert(It.IsAny<ActivityInfo>())).ReturnsAsync(3);
            _mockMapper.Setup(m => m.Map<FieldDTO>(It.IsAny<ActivityFieldDTO>())).Returns(new FieldDTO());
            _mockFieldService.Setup(x => x.Insert(It.IsAny<FieldDTO>())).ReturnsAsync(Result.Success(86));
            _mockActivityFieldService.Setup(x => x.Insert(It.IsAny<ActivityFieldDTO>()));

            ActivityService activityService = new ActivityService(_mockRepository.Object, _mockMapper.Object, _mockFieldService.Object, _mockActivityFieldService.Object, _mockActivityUserService.Object, _mockXmlDiagramService.Object, _mockActivityNotificationService.Object);
            await activityService.InsertMany(activitiesList, 1, 2);

            _mockRepository.Verify(x => x.Insert(It.IsAny<ActivityInfo>()), Times.Exactly(2));
            _mockFieldService.Verify(x => x.Insert(It.IsAny<FieldDTO>()), Times.Once());
            _mockActivityFieldService.Verify(x => x.Insert(It.IsAny<ActivityFieldDTO>()), Times.Exactly(2));
        }

        [Test]
        public void EnsureThatGetIdReturnsNullWhenNotHaveActivity()
        {
            _mockRepository.Setup(x => x.GetByTenant(It.IsAny<long>())).Returns(Enumerable.Empty<ActivityInfo>().AsQueryable());
            ActivityService activityService = new ActivityService(_mockRepository.Object, _mockMapper.Object, _mockFieldService.Object, _mockActivityFieldService.Object, _mockActivityUserService.Object, _mockXmlDiagramService.Object, _mockActivityNotificationService.Object);
            var result = activityService.GetId("iTest2", 2, 4);
            Assert.IsNull(result);
            _mockRepository.Verify(x => x.GetByTenant(It.IsAny<long>()), Times.Once());
        }

        [Test]
        public void EnsureThatGetIdReturnsWhenHaveActivity()
        {
            _mockRepository.Setup(x => x.GetByTenant(It.IsAny<long>())).Returns(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    ComponentInternalId = "iTest1",
                    ProcessVersionId = 2,
                    Name = "test1",
                    Id = 30
                },
                new ActivityInfo(){
                 ComponentInternalId = "iTest2",
                    ProcessVersionId = 2,
                    Name = "test2",
                    Id= 96
                }
            }.AsQueryable());

            ActivityService activityService = new ActivityService(_mockRepository.Object, _mockMapper.Object, _mockFieldService.Object, _mockActivityFieldService.Object, _mockActivityUserService.Object, _mockXmlDiagramService.Object, _mockActivityNotificationService.Object);
            var result = activityService.GetId("iTest2", 2, 4);

            Assert.AreEqual(96, result);
            _mockRepository.Verify(x => x.GetByTenant(It.IsAny<long>()), Times.Once());
        }
    }
}
