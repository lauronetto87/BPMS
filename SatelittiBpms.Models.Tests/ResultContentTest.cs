using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Tests
{
    public class ResultContentTest
    {
        RoleDTO _roleDTO = new RoleDTO()
        {
            Name = "teste ROLE",
            UsersIds = new List<int>() { 1, 2, 3 }
        };

        [Test]
        public void ensureGetValue()
        {
            ResultContent resultContent = GetBaseResultContentWithValue();
            var result = ResultContent<RoleDTO>.GetValue(resultContent);
            Assert.IsInstanceOf(typeof(RoleDTO), result);
            Assert.AreEqual(_roleDTO.Name, result.Name);
            Assert.AreEqual(3, result.UsersIds.Count);
        }

        private ResultContent GetBaseResultContentWithValue()
        {
            return new ResultContent<RoleDTO>(_roleDTO, true, null);
        }

        [Test]
        public void ensureGetErrorJson()
        {
            ResultContent error = new ResultContent(false, "newErrorID");
            string result = error.ToErrorJson();
            JObject jObject = JObject.Parse(result);
            Assert.IsTrue(jObject.ContainsKey("errorId"));
            Assert.AreEqual("newErrorID", jObject["errorId"].ToString());
        }
    }
}
