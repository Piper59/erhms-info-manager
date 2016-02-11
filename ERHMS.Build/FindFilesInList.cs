using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.Build
{
    public class FindFilesInList : Task
    {
        [Required]
        public ITaskItem[] Items { get; set; }

        [Required]
        public ITaskItem[] ItemsToFind { get; set; }

        [Output]
        public ITaskItem[] FoundItems { get; set; }

        public override bool Execute()
        {
            ICollection<string> pathsToFind = ItemsToFind.Select(item => new FileInfo(item.ItemSpec).FullName).ToList();
            ICollection<ITaskItem> foundItemsList = new List<ITaskItem>();
            foreach (ITaskItem item in Items)
            {
                FileInfo file = new FileInfo(item.ItemSpec);
                if (pathsToFind.Contains(file.FullName))
                {
                    foundItemsList.Add(item);
                }
            }
            FoundItems = foundItemsList.ToArray();
            return true;
        }
    }
}
