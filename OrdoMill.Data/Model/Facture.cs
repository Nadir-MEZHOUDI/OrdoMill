using PropertyChanged;
using SmartApp.Helpers.Bases;
using SmartApp.Helpers.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OrdoMill.Data.Model
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Facture : EntityBase
    {
        public Facture()
        {
            Montant =  Ordonnances.Sum(o => o.Medicaments.Sum(m => m.Ppa * m.Quantite));
        }

        public int Number { get; set; }

        public bool Chronic { get; set; }

        public bool Out { get; set; }

        public double? Montant { get; set; }

        public string Name => GetFactureName(this);

        public int? BordereauId { get; set; }

        [ForeignKey(nameof(BordereauId))]
        public virtual Bordereau Bordereau { get; set; }

        public virtual ICollection<Ordonnance> Ordonnances { get; set; } = new Collection<Ordonnance>();

        public string MinName => Number.ToString("D3") + (Bordereau?.Date ?? DateTime.Now).ToString("yy");

        [NotMapped]
        public string Region { get; set; }

        public static string GetFactureName(Facture instance)
        {
            var name = "FACTURE N°: " + instance.Number.ToString("D2");
            name += "-" + (instance.Bordereau?.Date ?? DateTime.Now).Year;
            if (instance.Chronic)
            {
                name += "-CH";

                if (instance.Out)
                    name += "HR";
                else
                    if (instance.Region.IsNotNullOrEmpty())
                    name += "-" + instance.Region;
            }
            return name;
        }
    }
}