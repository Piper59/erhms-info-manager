using Dapper;
using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using NUnit.Framework;
using System.Data;
using System.Reflection;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public abstract class EntityRepositoryTest
    {
        private class Gender : GuidEntity
        {
            protected override string Guid
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
                : base(@new) { }

            public Gender()
                : this(false) { }
        }

        private class GenderRepository : EntityRepository<Gender>
        {
            public static void Configure()
            {
                TypeMap typeMap = new TypeMap(typeof(Gender));
                typeMap.Get(nameof(Gender.GenderId)).SetId();
                typeMap.Get(nameof(Gender.New)).SetComputed();
                SqlMapper.SetTypeMap(typeof(Gender), typeMap);
            }

            public GenderRepository(IDataContext context)
                : base(context) { }
        }

        private class DataContext : IDataContext
        {
            public static void Configure()
            {
                GenderRepository.Configure();
            }

            public IDatabase Database { get; private set; }

            public Project Project
            {
                get { return null; }
            }

            public EntityRepository<Gender> Genders { get; private set; }

            public DataContext(IDatabase database)
            {
                Database = database;
                Genders = new EntityRepository<Gender>(this);
            }
        }

        private IDatabaseCreator creator;
        private DataContext context;

        protected abstract IDatabaseCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DataContext.Configure();
            creator = GetCreator();
            creator.SetUp();
            using (IDbConnection connection = creator.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.People.sql")));
            }
            context = new DataContext(creator.GetDatabase());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
        }

        [Test]
        public void SaveTest()
        {
            int count = context.Genders.Count();
            Gender gender = new Gender(true)
            {
                Name = "Neuter",
                Pronouns = ""
            };
            Assert.IsTrue(gender.New);
            context.Genders.Save(gender);
            count++;
            Assert.AreEqual(count, context.Genders.Count());
            Assert.IsFalse(gender.New);
            gender.Pronouns = "it;it;its;its";
            context.Genders.Save(gender);
            Assert.AreEqual(count, context.Genders.Count());
            Assert.IsFalse(gender.New);
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
