using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model;

[Serializable]
[AddINotifyPropertyChangedInterface]
public class Patient : EntityBase
{ 
    public string Nom { get; set; }

    public string Prenom { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateNaissance { get; set; }

    public int Alliance { get; set; }

    public string FullName => $"{Nom} {Prenom}";

    [DataType(DataType.Date)]
    public DateTime? Du { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Au { get; set; }

    public int AssureId { get; set; }

    [ForeignKey(nameof(AssureId))]
    public virtual Assure Assure { get; set; }

    [DefaultValue(false)]
    public bool Suspende { get; set; }

    public bool Activé => !Suspende;

    public virtual ICollection<Ordonnance> Ordonnances { get; set; } //= new Collection<Ordonnance>();
}