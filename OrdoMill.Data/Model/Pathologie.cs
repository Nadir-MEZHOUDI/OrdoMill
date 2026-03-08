using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;
using SmartApp.Helpers.Bases;
namespace OrdoMill.Data.Model
{
	[Serializable]
	[ImplementPropertyChanged]
	public  class Pathologie : EntityBase
	{
	    public string Code { get; set; }	
		public string Nom { get; set; }
		public virtual ICollection<Ordonnance> Ordonnances { get; set; } = new Collection<Ordonnance>();
	}
}