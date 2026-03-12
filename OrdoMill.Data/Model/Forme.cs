using System.Collections.ObjectModel;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model;

[Serializable]
	[AddINotifyPropertyChangedInterface]
	public  class Forme : EntityBase
	{       
		public string Name { get; set; }
		public string Abrg { get; set; }
		public virtual ICollection<Medicament> Medicaments { get; set; } = new Collection<Medicament>();
	}