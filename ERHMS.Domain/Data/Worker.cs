using ERHMS.Common.Text;
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
            get { return (string)GetProperty(nameof(FirstName)); }
            set { SetProperty(nameof(FirstName), value); }
        }

        public string PreferredName
        {
            get { return (string)GetProperty(nameof(PreferredName)); }
            set { SetProperty(nameof(PreferredName), value); }
        }

        public string MiddleInitial
        {
            get { return (string)GetProperty(nameof(MiddleInitial)); }
            set { SetProperty(nameof(MiddleInitial), value); }
        }

        public string LastName
        {
            get { return (string)GetProperty(nameof(LastName)); }
            set { SetProperty(nameof(LastName), value); }
        }

        public string NameSuffix
        {
            get { return (string)GetProperty(nameof(NameSuffix)); }
            set { SetProperty(nameof(NameSuffix), value); }
        }

        public string FullName
        {
            get
            {
                ICollection<string> components = new List<string>();
                if (string.IsNullOrEmpty(FirstName))
                {
                    if (!string.IsNullOrEmpty(PreferredName))
                    {
                        components.Add(PreferredName);
                    }
                }
                else
                {
                    components.Add(FirstName);
                    if (!string.IsNullOrEmpty(PreferredName))
                    {
                        components.Add($"({PreferredName})");
                    }
                }
                if (!string.IsNullOrEmpty(MiddleInitial))
                {
                    components.Add(MiddleInitial);
                }
                if (!string.IsNullOrEmpty(LastName))
                {
                    components.Add(LastName);
                }
                if (!string.IsNullOrEmpty(NameSuffix))
                {
                    components.Add(NameSuffix);
                }
                return string.Join(" ", components);
            }
        }

        public string EmailAddress
        {
            get { return (string)GetProperty(nameof(EmailAddress)); }
            set { SetProperty(nameof(EmailAddress), value); }
        }

        private double similarity;
        public double Similarity
        {
            get
            {
                return similarity;
            }
            private set
            {
                if (similarity != value)
                {
                    similarity = value;
                    OnPropertyChanged(nameof(Similarity));
                }
            }
        }

        public void SetSimilarity(string firstName, string lastName, string emailAddress)
        {
            ICollection<double> similarities = new List<double>();
            if (!string.IsNullOrEmpty(firstName))
            {
                similarities.Add(Math.Max(
                    StringDistanceCalculator.GetSimilarity(firstName, FirstName ?? ""),
                    StringDistanceCalculator.GetSimilarity(firstName, PreferredName ?? "")));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                similarities.Add(StringDistanceCalculator.GetSimilarity(lastName, LastName ?? ""));
            }
            if (!string.IsNullOrEmpty(emailAddress))
            {
                similarities.Add(StringDistanceCalculator.GetSimilarity(emailAddress, EmailAddress ?? ""));
            }
            Similarity = similarities.Count == 0 ? 0.0 : similarities.Average();
        }
    }
}
