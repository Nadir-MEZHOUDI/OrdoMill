using System;
using System.ComponentModel;

namespace OrdoMill.Serializables
{
    [Serializable]
	public class SerializablePatient
	{ 
		public string Nom { get; set; }

		public string Prenom { get; set; }

		public string DateNaissance { get; set; }

		public int Alliance { get; set; }

		public string Du { get; set; }

		public string Au { get; set; }

        [DefaultValue(false)]
		public bool Suspende { get; set; }
	}
}