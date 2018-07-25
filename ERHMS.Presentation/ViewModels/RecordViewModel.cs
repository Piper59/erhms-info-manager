using ERHMS.Domain;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    [TypeDescriptionProvider(typeof(RecordViewModelTypeDescriptionProvider))]
    public class RecordViewModel : ViewModelBase
    {
        private bool import;
        public bool Import
        {
            get { return import; }
            set { SetProperty(nameof(Import), ref import, value); }
        }

        private Record record;
        public Record Record
        {
            get { return record; }
            set { SetProperty(nameof(Record), ref record, value); }
        }

        private ViewEntity entity;
        public ViewEntity Entity
        {
            get { return entity; }
            set { SetProperty(nameof(Entity), ref entity, value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { SetProperty(nameof(Responder), ref responder, value); }
        }
    }

    public class RecordViewModelTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new RecordViewModelTypeDescriptor(base.GetTypeDescriptor(objectType, instance), (RecordViewModel)instance);
        }
    }

    public class RecordViewModelTypeDescriptor : CustomTypeDescriptor
    {
        public RecordViewModel Instance { get; private set; }

        public RecordViewModelTypeDescriptor(ICustomTypeDescriptor parent, RecordViewModel instance)
            : base(parent)
        {
            Instance = instance;
        }

        private IEnumerable<PropertyDescriptor> GetPropertiesInternal()
        {
            foreach (KeyValuePair<string, string> property in Instance.Record)
            {
                yield return new CustomPropertyDescriptor<RecordViewModel, string>(
                    property.Key,
                    instance => instance.Record[property.Key],
                    (instance, value) => instance.Record[property.Key] = value);
            }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(GetPropertiesInternal().ToArray());
        }
    }
}
