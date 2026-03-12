using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model;

	[AddINotifyPropertyChangedInterface]
	public  class Historique:EntityBase
	{       
		public int? PatientId { get; set; }

		[ForeignKey(nameof(PatientId))]
		public virtual Patient Patient { get; set; }

		public DateTime DateTime { get; set; }

		public string Note { get; set; }

		public int? OperationId { get; set; }

		[ForeignKey(nameof(OperationId))]
		public virtual Operation Operation { get; set; }

		public int? UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual User User { get; set; }
	}