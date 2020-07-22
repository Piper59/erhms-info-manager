using Epi;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Infrastructure
{
    public static class FieldExtensions
    {
        private const char RelateConditionSeparator = ':';

        public static string MapChildFieldNames(string fieldNames, IDictionary<string, string> fieldNameMap)
        {
            IList<string> fieldNameList = fieldNames.Split(Constants.LIST_SEPARATOR);
            for (int index = 0; index < fieldNameList.Count; index++)
            {
                string original = fieldNameList[index];
                if (fieldNameMap.TryGetValue(original, out string modified))
                {
                    fieldNameList[index] = modified;
                }
            }
            return string.Join(Constants.LIST_SEPARATOR.ToString(), fieldNameList);

        }

        public static string MapRelateConditions(string conditions, IDictionary<int, int> fieldIdMap)
        {
            IList<string> conditionList = conditions.Split(Constants.LIST_SEPARATOR);
            for (int index = 0; index < conditionList.Count; index++)
            {
                string condition = conditionList[index];
                IList<string> chunks = condition.Split(RelateConditionSeparator);
                if (chunks.Count != 2)
                {
                    continue;
                }
                string columnName = chunks[0];
                if (!int.TryParse(chunks[1], out int fieldId))
                {
                    continue;
                }
                if (!fieldIdMap.TryGetValue(fieldId, out fieldId))
                {
                    continue;
                }
                conditionList[index] = string.Concat(columnName, RelateConditionSeparator, fieldId);
            }
            return string.Join(Constants.LIST_SEPARATOR.ToString(), conditionList);
        }
    }
}
