using NUnit.Framework;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Models.Tests
{
    public class FieldValueFileInfoTest
    {
        [Test]
        public void EnsureSignedFromAsFieldValueFileViewModel()
        {
            var info = new FieldValueFileInfo
            {
                TaskSignerFile = new TaskSignerFileInfo
                {
                    TaskSigner = new TaskSignerInfo
                    {
                        Status = Enums.TaskSignerStatusEnum.CONCLUDED,
                    },
                },
                FieldValue = new FieldValueInfo
                {
                    Field = new FieldInfo()
                }
            };
            var viewModel = info.AsFieldValueFileViewModel(null);
            Assert.AreEqual(viewModel.Signed, true);
        }

    }
}
