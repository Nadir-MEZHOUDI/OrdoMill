using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model;

[Serializable]
[AddINotifyPropertyChangedInterface]
public  class Medicament : EntityBase
{    
    public string CnasId { get; set; }

    public string Nom { get; set; }

    public string Dci { get; set; }
    public string Dose { get; set; }
    public string Boite { get; set; }

    [DefaultValue("0.00")]
    public string Tr { get; set; } = "0.00";
    public string Unite { get; set; }

    [DefaultValue(true)]
    public bool Remboursable { get; set; } = true;

    [DefaultValue(false)]
    public bool Controle { get; set; }


    public int? FormeId { get; set; }
    [ForeignKey(nameof(FormeId))]
    public virtual Forme Forme { get; set; }

    public virtual ICollection<MedOrd> Ordonnances { get; set; } = new Collection<MedOrd>();
    public string FullName => $"{Nom?.Trim()} {Dose?.Trim()} {Boite?.Trim()} {Forme?.Abrg?.Trim()}";
}