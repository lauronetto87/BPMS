using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Repository.Tests.Context;
using SatelittiBpms.Repository.Tests.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Tests
{
    public class AbstractRepositoryBaseTest
    {
        Mock<BpmsTestHelperContext> _mockBpmsTestHelperContext;
        Mock<DbSet<TestHelperInfo>> _mockDbSet;

        private int counterIdentity = 0;

        [SetUp]
        public void init()
        {
            _mockBpmsTestHelperContext = new Mock<BpmsTestHelperContext>();
            _mockDbSet = new Mock<DbSet<TestHelperInfo>>();

            _mockDbSet.Setup(x => x.AddAsync(It.IsAny<TestHelperInfo>(), It.IsAny<CancellationToken>())).Callback<TestHelperInfo, CancellationToken>(async (info, cancelationToken) =>
            {
                info.Id = Interlocked.Increment(ref counterIdentity);
            });
            _mockBpmsTestHelperContext.Setup(x => x.Set<TestHelperInfo>()).Returns(_mockDbSet.Object);
        }

        [Test]
        public async Task ensureInsert()
        {
            TestHelperInfo info = new TestHelperInfo()
            {
                TenantId = 55
            };

            AbstractRepositoryBase<TestHelperInfo> repo = new AbstractRepositoryBase<TestHelperInfo>(_mockBpmsTestHelperContext.Object);
            var result = await repo.Insert(info);
            Assert.AreEqual(1, info.Id);
            _mockDbSet.Verify(m => m.AddAsync(It.IsAny<TestHelperInfo>(), It.IsAny<CancellationToken>()), Times.Once());
            _mockBpmsTestHelperContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task ensureDelete()
        {
            TestHelperInfo info = new TestHelperInfo()
            {
                Id = 1,
                TenantId = 55
            };

            AbstractRepositoryBase<TestHelperInfo> repo = new AbstractRepositoryBase<TestHelperInfo>(_mockBpmsTestHelperContext.Object);
            await repo.Delete(info);
            _mockDbSet.Verify(m => m.Remove(It.IsAny<TestHelperInfo>()), Times.Once());
            _mockBpmsTestHelperContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
