﻿using Dapper;
using ERHMS.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Test.Dapper
{
    public class PersonRepository : Repository<Person, string>
    {
        public PersonRepository(IDatabase database)
            : base(database) { }

        public override IEnumerable<Person> Select(string sql = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                string format = @"
                    SELECT Person.*, NULL AS Separator, Gender.*
                    FROM Person
                    INNER JOIN Gender ON Person.GenderId = Gender.GenderId
                    {0}";
                sql = string.Format(format, sql);
                Func<Person, Gender, Person> map = (person, gender) =>
                {
                    person.Gender = gender;
                    return person;
                };
                return connection.Query(sql, map, parameters, transaction, splitOn: "Separator");
            });
        }

        public override Person SelectById(string id)
        {
            string sql = string.Format("WHERE Person.PersonId = @Id");
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(sql, parameters).SingleOrDefault();
        }
    }
}
