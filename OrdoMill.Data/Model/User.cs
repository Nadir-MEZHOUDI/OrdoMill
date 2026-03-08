using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using SmartApp.Helpers.Bases;

namespace OrdoMill.Data.Model
{
	[ImplementPropertyChanged]
	public  class User : EntityBase    
    {         
		public string UserName { get; set; }
		public string Password { get; set; }
		public string FullName { get; set; }
		[NotMapped]
		public string ConfirmationPassword { get; set; }
		public bool IsAdmin { get; set; }
		[DefaultValue(true)]
		public bool IsWork { get; set; }
		public bool AllowAdd { get; set; }
		public bool AllowUpdate { get; set; }
		public bool AllowPrintDocs { get; set; }
        public virtual ICollection<Historique> Historiques { get; set; } = new Collection<Historique>();
    }
}