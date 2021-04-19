using ERHMS.Common;
using ERHMS.EpiInfo.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Domain.Data
{
    public class Worker : Record
    {
        public string FirstName
        {
            get { return (string)GetPropertyCore(); }
            set { SetPropertyCore(value); }
        }

        public string PreferredName
        {
            get { return (string)GetPropertyCore(); }
            set { SetPropertyCore(value); }
        }

        public string MiddleInitial
        {
            get { return (string)GetPropertyCore(); }
            set { SetPropertyCore(value); }
        }

        public string LastName
        {
            get { return (string)GetPropertyCore(); }
            set { SetPropertyCore(value); }
        }

        public string NameSuffix
        {
            get { return (string)GetPropertyCore(); }
            set { SetPropertyCore(value); }
        }

        public string FullName
        {
            get
            {
                ICollection<string> components = new List<string>();
                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    if (!string.IsNullOrWhiteSpace(PreferredName))
                    {
                        components.Add(PreferredName);
                    }
                }
                else
                {
                    components.Add(FirstName);
                    if (!string.IsNullOrWhiteSpace(PreferredName))
                    {
                        components.Add($"({PreferredName})");
                    }
                }
                if (!string.IsNullOrWhiteSpace(MiddleInitial))
                {
                    components.Add(MiddleInitial);
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    components.Add(LastName);
                }
                if (!string.IsNullOrWhiteSpace(NameSuffix))
                {
                    components.Add(NameSuffix);
                }
                return string.Join(" ", components);
            }
        }

        public string EmailAddress
        {
            get { return (string)GetPropertyCore(); }
            set { SetPropertyCore(value); }
        }

        public Worker()
        {
            FirstName = null;
            PreferredName = null;
            MiddleInitial = null;
            LastName = null;
            NameSuffix = null;
            EmailAddress = null;
        }

        public double GetSimilarity(string firstName, string lastName, string emailAddress)
        {
            ICollection<double> similarities = new List<double>();
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                similarities.Add(Math.Max(
                    StringDistanceCalculator.GetSimilarity(firstName, FirstName ?? ""),
                    StringDistanceCalculator.GetSimilarity(firstName, PreferredName ?? "")));
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                similarities.Add(StringDistanceCalculator.GetSimilarity(lastName, LastName ?? ""));
            }
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                similarities.Add(StringDistanceCalculator.GetSimilarity(emailAddress, EmailAddress ?? ""));
            }
            if (similarities.Count == 0)
            {
                return 0.0;
            }
            else
            {
                return similarities.Average();
            }
        }
    }
}
