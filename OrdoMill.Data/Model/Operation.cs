using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;
using SmartApp.Helpers.Bases;

namespace OrdoMill.Data.Model
{
	[ImplementPropertyChanged]
	public class Operation:EntityBase
	{
		public string Name { get; set; }
		public virtual ICollection<Historique> Historiques { get; set; } = new Collection<Historique>();
	}
}