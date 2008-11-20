using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Schema;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    public class FeatureSchemaDesign : INotifyPropertyChanged
    {
        private FeatureSchema _schema;

        public FeatureSchemaDesign(FeatureSchema schema)
        {
            _schema = schema;
        }

        [Description("The name of the schema")]
        public string Name
        {
            get { return _schema.Name; }
            set 
            { 
                _schema.Name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        [Description("Schema description")]
        public string Description
        {
            get { return _schema.Description; }
            set { _schema.Description = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
