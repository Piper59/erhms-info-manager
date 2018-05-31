using System;
using System.Collections.Generic;
using System.Windows;

namespace ERHMS.Presentation
{
    public class DialogTypeMap : Dictionary<Type, Type>
    {
        public static DialogTypeMap Instance
        {
            get { return (DialogTypeMap)Application.Current.Resources[typeof(DialogTypeMap)]; }
        }

        public Type GetDialogType(object model)
        {
            return this[model.GetType()];
        }
    }
}
