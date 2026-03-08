using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using SmartApp.Helpers.Bases;

namespace OrdoMill.Data.Model
{
    [Serializable]
    [ImplementPropertyChanged]
    public class MedOrd : EntityBase
    {
        public int OrdonnanceId { get; set; }

        [ForeignKey(nameof(OrdonnanceId))]
        public virtual Ordonnance Ordonnance { get; set; }

        public int MedicamentId { get; set; }

        [ForeignKey(nameof(MedicamentId))]
        public virtual Medicament Medicament { get; set; }

        public int Duree { get; set; }


        [DefaultValue(1)]
        public int Quantite { get; set; } = 1;


        [DefaultValue(0)]
        public double Ppa { get; set; }

        public double Montant => Ppa * Quantite;
    }
}