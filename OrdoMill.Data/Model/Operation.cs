using System.Collections.ObjectModel;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model;

	[AddINotifyPropertyChangedInterface]
	public class Operation:EntityBase
	{
		public string Name { get; set; }
		public virtual ICollection<Historique> Historiques { get; set; } = new Collection<Historique>();
	}