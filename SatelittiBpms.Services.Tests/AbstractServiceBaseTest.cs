using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Satelitti.Model;
using SatelittiBpms.Data;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Repository;
using SatelittiBpms.Services.Tests.Infos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class AbstractServiceBaseTest
    {
        private BaseInfoDTO DTO { get; set; }
        private BaseInfo info { get; set; }

        private DbContextOptions<BpmsContext> options = new DbContextOptionsBuilder<BpmsContext>()
                .UseInMemoryDatabase(databaseName: "AbstractTest")
                .Options;

        private BpmsContext context;

        private Mock<IMapper> mapperMock;

        private Mock<AbstractRepositoryBase<BaseInfo>> mockRepository;

        [SetUp]
        public void Setup()
        {
            info = new BpmsBaseInfo()
            {
                Id = 1,
                TenantId = 55
            };
            DTO = new BaseInfoDTO();
            context = new BpmsContext(options);
            mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<BaseInfoDTO, BaseInfo>(It.IsAny<BaseInfoDTO>())).Returns(info);
            mockRepository = new Mock<AbstractRepositoryBase<BaseInfo>>(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task MustInsertAbstract()
        {
            mockRepository.Setup(m => m.Insert(It.IsAny<BaseInfo>())).ReturnsAsync(info.Id);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            var result = await baseAbstract.Insert(DTO);
            Assert.AreEqual(1, result.Value);
            mapperMock.Verify(x => x.Map<BaseInfo>(DTO), Times.Once());
        }

        [Test]
        public async Task MustDeleteByDTOAbstract()
        {
            mockRepository.Setup(m => m.Delete(It.IsAny<BaseInfo>()));
            mapperMock.Setup(m => m.Map<BaseInfo>(It.IsAny<BaseInfoDTO>())).Returns(info);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            var result = await baseAbstract.Delete(DTO);
            Assert.IsTrue(result.Success);
            mapperMock.Verify(x => x.Map<BaseInfo>(DTO), Times.Once());
            mockRepository.Verify(x => x.Delete(info), Times.Once());
        }

        [Test]
        public async Task MustDeleteByIdAbstract()
        {
            mockRepository.Setup(m => m.Delete(It.IsAny<BaseInfo>()));
            mockRepository.Setup(m => m.Get(It.IsAny<int>())).ReturnsAsync(info);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            var result = await baseAbstract.Delete(info.Id);
            Assert.IsTrue(result.Success);
            mockRepository.Verify(x => x.Delete(info), Times.Once());
            mockRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task MustDeleteByIdAbstractThrowsWhenInfoNotExists()
        {
            mockRepository.Setup(m => m.Delete(It.IsAny<BaseInfo>()));
            mockRepository.Setup(m => m.Get(It.IsAny<int>())).ReturnsAsync(() => null);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            var result = await baseAbstract.Delete(info.Id);
            Assert.IsFalse(result.Success);
            mockRepository.Verify(x => x.Delete(info), Times.Never());
            mockRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task MustGetByIdAbstract()
        {
            mockRepository.Setup(m => m.Get(It.IsAny<int>()));
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            var result = await baseAbstract.Get(info.Id);
            Assert.IsTrue(result.Success);
            mockRepository.Verify(x => x.Get(info.Id), Times.Once());
        }

        [Test]
        public async Task MustUpdateAbstractNotFoundBaseInfo()
        {
            mockRepository.Setup(m => m.Update(It.IsAny<BaseInfo>()));
            mockRepository.Setup(m => m.Get(It.IsAny<int>())).ReturnsAsync((BaseInfo)null);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            var result = await baseAbstract.Update(info.Id, DTO);
            Assert.IsFalse(result.Success);
            mockRepository.Verify(x => x.Get(info.Id), Times.Once());
            mapperMock.Verify(x => x.Map(info, info), Times.Never());
            mapperMock.Verify(x => x.Map<BaseInfo>(DTO), Times.Never());
            mockRepository.Verify(x => x.Update(info), Times.Never());
        }

        [Test]
        public async Task MustUpdateAbstractFoundBaseInfo()
        {
            DTO = new BaseInfoDTO();
            mockRepository.Setup(m => m.Update(It.IsAny<BaseInfo>()));
            mockRepository.Setup(m => m.Get(It.IsAny<int>())).ReturnsAsync(info);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            mapperMock.Setup(m => m.Map(It.IsAny<BaseInfo>(), It.IsAny<BaseInfo>())).Returns(info);
            mapperMock.Setup(m => m.Map<BaseInfo>(It.IsAny<BaseInfoDTO>())).Returns(info);
            var result = await baseAbstract.Update(info.Id, DTO);
            Assert.IsTrue(result.Success);
            mockRepository.Verify(x => x.Get(info.Id), Times.Once());
            mapperMock.Verify(x => x.Map<BaseInfo>(DTO), Times.Once());
            mapperMock.Verify(x => x.Map(info, info), Times.Once());
            mockRepository.Verify(x => x.Update(info), Times.Once());
        }

        [Test]
        public async Task MustListAbstractEmptyFilter()
        {
            var list = new List<BaseInfo>();
            list.Add(info);
            list.Add(new BpmsBaseInfo { Id = 2, TenantId = 56 });
            mockRepository.Setup(m => m.ListAsync()).ReturnsAsync(list);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            List<BaseInfo> listR = await baseAbstract.ListAsync();
            mockRepository.Verify(x => x.ListAsync(), Times.Once());
            Assert.AreEqual(2, listR.Count);
        }

        [Test]
        public async Task MustListAbstractFilledFilter()
        {
            var list = new List<BaseInfo>();
            list.Add(info);
            list.Add(new BpmsBaseInfo { Id = 2, TenantId = 56 });
            mockRepository.Setup(m => m.ListAsync()).ReturnsAsync(list);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            Dictionary<string, string> filterList = new Dictionary<string, string>();
            filterList.Add("Id", "1");
            List<BaseInfo> listR = await baseAbstract.ListAsync(filterList);
            mockRepository.Verify(x => x.ListAsync(), Times.Once());
            Assert.AreEqual(1, listR.Count);
        }

        [Test]
        public void MustThrowWhenFilterListHaveNotKnowPropertyName()
        {
            var list = new List<BaseInfo>();
            list.Add(info);
            list.Add(new BpmsBaseInfo { Id = 2, TenantId = 56 });
            mockRepository.Setup(m => m.ListAsync()).ReturnsAsync(list);
            BaseServiceAbstract baseAbstract = new BaseServiceAbstract(mockRepository.Object, mapperMock.Object);
            Dictionary<string, string> filterList = new Dictionary<string, string>();
            filterList.Add("anyProperty", "1");
            Assert.ThrowsAsync(Is.TypeOf<Exception>().And.Message.EqualTo("Property 'anyProperty' is not a known value to filter 'BaseInfo' class"),
                    async () => await baseAbstract.ListAsync(filterList));
            mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }
    }
}
