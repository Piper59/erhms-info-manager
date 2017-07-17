using Epi;
using ERHMS.EpiInfo.Domain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;

namespace ERHMS.Test.EpiInfo.Domain
{
    public class ViewEntityTest
    {
        [Test]
        public void PropertiesTest()
        {
            ViewEntity entity = new ViewEntity();
            dynamic person = entity;
            ICollection<string> actual = new List<string>();
            ICollection<string> expected = new List<string>();
            entity.PropertyChanged += (sender, e) =>
            {
                actual.Add(e.PropertyName);
            };
            {
                person.FirstName = "John";
                expected.Add("FirstName");
                person.LastName = "Doe";
                expected.Add("LastName");
                entity.Touch();
                expected.Add(nameof(ViewEntity.CreatedBy));
                expected.Add(nameof(ViewEntity.CreatedOn));
                expected.Add(nameof(ViewEntity.ModifiedBy));
                expected.Add(nameof(ViewEntity.ModifiedOn));
                expected.Add(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME);
                expected.Add(ColumnNames.RECORD_FIRST_SAVE_TIME);
                expected.Add(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME);
                expected.Add(ColumnNames.RECORD_LAST_SAVE_TIME);
                CollectionAssert.AreEquivalent(expected, actual);
            }
            actual.Clear();
            expected.Clear();
            {
                person.FirstName = "Jane";
                expected.Add("FirstName");
                person.LastName = "Doe";
                CollectionAssert.AreEquivalent(expected, actual);
            }
            actual.Clear();
            expected.Clear();
            {
                entity.Deleted = true;
                expected.Add(nameof(ViewEntity.Deleted));
                expected.Add(nameof(ViewEntity.RecordStatus));
                expected.Add(ColumnNames.REC_STATUS);
                CollectionAssert.AreEquivalent(expected, actual);
            }
            actual.Clear();
            expected.Clear();
            {
                entity.New = false;
                expected.Add(nameof(ViewEntity.New));
                Thread.Sleep(1000);
                entity.Touch();
                expected.Add(nameof(ViewEntity.ModifiedOn));
                expected.Add(ColumnNames.RECORD_LAST_SAVE_TIME);
                CollectionAssert.AreEquivalent(expected, actual);
            }
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
