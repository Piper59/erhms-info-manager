using Epi;
using System.IO;

namespace ERHMS.EpiInfo.Analytics
{
    public abstract class Asset
    {
        public View View { get; }

        public Asset(View view)
        {
            View = view;
        }

        public abstract void Save(Stream stream);
    }
}
