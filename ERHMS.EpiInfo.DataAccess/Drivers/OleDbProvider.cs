using System;
using System.ComponentModel;

namespace ERHMS.EpiInfo.DataAccess
{
    public enum OleDbProvider
    {
        Jet,
        Ace
    }

    public static class OleDbProviderExtensions
    {
        public static string GetName(this OleDbProvider @this)
        {
            switch (@this)
            {
                case OleDbProvider.Jet:
                    return "Microsoft.Jet.OLEDB.4.0";
                case OleDbProvider.Ace:
                    return "Microsoft.ACE.OLEDB.12.0";
                default:
                    throw new InvalidEnumArgumentException("this", Convert.ToInt32(@this), typeof(OleDbProvider));
            }
        }
    }
}
