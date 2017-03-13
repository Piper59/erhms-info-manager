using Epi;
using ERHMS.EpiInfo.Domain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;

namespace ERHMS.Test.EpiInfo.Domain
{
    public class ViewEntityTest
    {
        [Test]
        public void PropertiesTest()
        {
            ICollection<string> actual = new List<string>();
            ViewEntity entity = new ViewEntity();
            entity.PropertyChanged += (sender, e) =>
            {
                actual.Add(e.PropertyName);
            };
            dynamic person = entity;
            person.FirstName = "John";
            person.LastName = "Doe";
            entity.SetAuditProperties(true, true);
            ICollection<string> expected = new HashSet<string>
            {
                "FirstName",
                "LastName",
                nameof(ViewEntity.FirstSaveUserName),
                nameof(ViewEntity.FirstSaveStamp),
                nameof(ViewEntity.LastSaveUserName),
                nameof(ViewEntity.LastSaveStamp),
                ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME,
                ColumnNames.RECORD_FIRST_SAVE_TIME,
                ColumnNames.RECORD_LAST_SAVE_LOGON_NAME,
                ColumnNames.RECORD_LAST_SAVE_TIME
            };
            CollectionAssert.AreEquivalent(expected, actual);
            Assert.IsFalse(entity.Deleted);
            entity.Deleted = true;
            Assert.IsTrue(entity.Deleted);
            expected.Add(nameof(ViewEntity.Deleted));
            expected.Add(nameof(ViewEntity.RecordStatus));
            expected.Add(ColumnNames.REC_STATUS);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void CloneTest()
        {
            dynamic entity = new ViewEntity();
            entity.Value = 1;
            entity.Subobject = new ExpandoObject();
            entity.Subobject.Value = 1;
            entity.Subentity = new ViewEntity();
            entity.Subentity.Value = 1;
            dynamic clone = entity.Clone();
            Assert.AreEqual(1, clone.Value);
            Assert.AreEqual(1, clone.Subobject.Value);
            Assert.AreEqual(1, clone.Subentity.Value);
            entity.Value = 2;
            entity.Subobject.Value = 2;
            entity.Subentity.Value = 2;
            Assert.AreEqual(1, clone.Value);
            Assert.AreEqual(2, clone.Subobject.Value);
            Assert.AreEqual(1, clone.Subentity.Value);
        }
    }
}
