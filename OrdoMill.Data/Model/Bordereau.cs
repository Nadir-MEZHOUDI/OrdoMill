using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Bordereau : EntityBase
    {
        public Bordereau()
        {
            Montant = Factures.Sum(f => f.Ordonnances.Sum(o => o.Medicaments.Sum(m => m.Ppa*m.Quantite)));
        }

        public DateTime? Date { get; set; } = DateTime.Now;
        public virtual ICollection<Facture> Factures { get; set; } = new Collection<Facture>();
        public int Number { get; set; }

        [NotMapped]
        public double? Montant { get; set; }
  
        public string Name => $"{Number}-{Date?.ToString("yyyy")}";
     //   public double? Montant => Factures.Sum(x => x.Montant);
        public double? OrdosCount => Factures.Sum(x => x.Ordonnances.Count);

        public bool IsFinished { get; set; }

        // public double? OrdonsTotal => Factures.Sum(x => x.Ordonnances.Sum(y => y.Montant));

    }
}