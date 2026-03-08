using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using SmartApp.Helpers.Bases;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Data.Model
{
    [ImplementPropertyChanged]
    public class Assure : EntityBase
    {
        public string Matricule { get; set; }

        public string Nom { get; set; }

        public string Prenom { get; set; }

        public string Adresse { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateNaissance { get; set; }

        public string DateNaissanceString => DateNaissance?.ToString("dd-MM-yyyy");

        [DataType(DataType.Date)]
        public DateTime? Jusquea { get; set; }

        public string JusqueaString => Jusquea?.ToString("dd-MM-yyyy");

        public string Lieu { get; set; }

        public string Tel { get; set; }

        public string Grade { get; set; }

        [DefaultValue(false)]
        public bool IsContractial { get; set; }

        [DefaultValue("0")]
        public string Ccp { get; set; } = "0";

        public string ArNom { get; set; }

        public string ArPrenom { get; set; }

        public string Note { get; set; }

        [DefaultValue(false)]
        public bool Suspende { get; set; }

        public virtual ICollection<Patient> Patients { get; set; } = new Collection<Patient>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullName
        {
            get { return $"{Nom}   {Prenom}"; }
            private set { }
        }

        public string GetCcp => Ccp?.IsNotNullOrEmpty() == true ? Ccp : "0";

        public bool Activé => !Suspende;
    }
}