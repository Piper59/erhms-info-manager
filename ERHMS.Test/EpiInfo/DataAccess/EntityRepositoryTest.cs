using Dapper;
using ERHMS.Dapper;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public abstract class EntityRepositoryTest
    {
        private class Gender : GuidEntity
        {
            public override string Guid
            {
                get { return GenderId; }
                set { GenderId = value; }
            }

            public string GenderId
            {
                get { return GetProperty<string>(nameof(GenderId)); }
                set { SetProperty(nameof(GenderId), value); }
            }

            public string Name
            {
                get { return GetProperty<string>(nameof(Name)); }
                set { SetProperty(nameof(Name), value); }
            }

            public string Pronouns
            {
                get { return GetProperty<string>(nameof(Pronouns)); }
                set { SetProperty(nameof(Pronouns), value); }
            }

            public Gender(bool @new)
                : base(@new)
            {
                AddSynonym(nameof(GenderId), nameof(Guid));
            }

            public Gender()
                : this(false) { }
        }

        private class GenderRepository : EntityRepository<Gender>
        {
            public static void Configure()
            {
                TypeMap typeMap = new TypeMap(typeof(Gender));
                typeMap.Get(nameof(Gender.New)).SetComputed();
                typeMap.Get(nameof(Gender.Id)).SetComputed();
                typeMap.Get(nameof(Gender.Guid)).SetComputed();
                typeMap.Get(nameof(Gender.GenderId)).SetId();
                SqlMapper.SetTypeMap(typeof(Gender), typeMap);
            }

            public GenderRepository(IDatabase database)
                : base(database) { }
        }

        private IDatabaseCreator creator;
        private GenderRepository genders;

        protected abstract IDatabaseCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            GenderRepository.Configure();
            creator = GetCreator();
            creator.SetUp();
            using (IDbConnection connection = creator.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.People.sql")));
            }
            genders = new GenderRepository(creator.GetDatabase());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
        }

        [Test]
        public void SaveTest()
        {
            int count = genders.Count();
            Gender gender = new Gender(true)
            {
                Name = "Neuter",
                Pronouns = ""
            };
            Assert.IsTrue(gender.New);
            genders.Save(gender);
            Assert.AreEqual(++count, genders.Count());
            Assert.IsFalse(gender.New);
            gender.Pronouns = "it;it;its;its";
            genders.Save(gender);
            Assert.AreEqual(count, genders.Count());
            Assert.IsFalse(gender.New);
        }

        [Test]
        public void RefreshTest()
        {
            Gender male = new Gender
            {
                GenderId = "273c6d62-be89-48df-9e04-775125bc4f6a"
            };
            Assert.AreEqual("Male", genders.Refresh(male).Name);
            Gender female = new Gender
            {
                GenderId = "a7d96f3a-a990-4619-82d0-fcd9a9629f31"
            };
            IEnumerable<Gender> refreshed = genders.Refresh(new Gender[]
            {
                male,
                male,
                male,
                female
            });
            Assert.AreEqual(2, refreshed.Count());
        }
    }

    public class AccessEntityRepositoryTest : EntityRepositoryTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return AccessDatabaseCreator.ForName(nameof(AccessEntityRepositoryTest));
        }
    }

    public class SqlServerEntityRepositoryTest : EntityRepositoryTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new SqlServerDatabaseCreator();
        }
    }
}
