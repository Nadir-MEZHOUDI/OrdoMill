using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PropertyChanged;
using SmartApp.Helpers.Bases;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Data.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Ordonnance : EntityBase
    {
        public Ordonnance()
        {
            Montant = Medicaments.Sum(m => m.Ppa * m.Quantite);
        }
        public DateTime ServiDate { get; set; } = DateTime.Now;
        public DateTime SoineDate { get; set; } = DateTime.Now;
        public int? MedecinId { get; set; }
        [ForeignKey(nameof(MedecinId))]
        public virtual Medecin Medecin { get; set; }
        public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; }
        public int? FactureId { get; set; }
        [ForeignKey(nameof(FactureId))]
        public virtual Facture Facture { get; set; }
        [DefaultValue(false)]
        public bool IsChronique { get; set; }
        [DefaultValue(false)]
        public bool IsHorsRegion { get; set; }
        public string PathologieCode { get; set; }
        public string VignettesString => Vignettes == null || Vignettes == 0 ? "" : Vignettes?.ToString("D");
        public string MontantString => Montant == null || Montant == 0 ? "" : Montant?.ToString("F");
        public int? Vignettes => Medicaments?.Sum(med => med.Quantite);
        public double? Montant { get; set; }
        public virtual ICollection<MedOrd> Medicaments { get; set; } = new Collection<MedOrd>();
        public string Contenu => string.Join(", ", Medicaments.Select(med => $"{med?.Medicament?.Nom} {med?.Medicament?.Forme?.Abrg} {med?.Quantite} ")).UppercaseAllFirsts();

        [NotMapped]
        [DefaultValue("")]
        public List<string> Problems { get; set; }
    }
}