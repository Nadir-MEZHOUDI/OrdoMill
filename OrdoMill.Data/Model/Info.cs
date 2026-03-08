using System;
using PropertyChanged;
using SmartApp.Helpers.Bases;

namespace OrdoMill.Data.Model
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Info : EntityBase
    {
        public string Pharmacie { get; set; }

        public string Adresse { get; set; }


        public string Tel { get; set; }

        public string Fax { get; set; }


        public string CodeFiscal { get; set; }


        public string Article { get; set; }


        public string Rc { get; set; }


        public string Ccp { get; set; }


        public string Doit { get; set; }


        public string Code { get; set; }

        public string Region { get; set; }
    }
}