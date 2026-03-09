using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OrdoMill.Data.Properties;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model
{
    [Serializable]
	[AddINotifyPropertyChangedInterface]
	public  class Forme : EntityBase
	{       
		public string Name { get; set; }
		public string Abrg { get; set; }
		public virtual ICollection<Medicament> Medicaments { get; set; } = new Collection<Medicament>();
	}
}