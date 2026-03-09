using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PropertyChanged;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Medecin : EntityBase
    {
        public string Nom { get; set; }
        public int Type { get; set; }

        public string Tel { get; set; }
        public string Adresse { get; set; }

        public string FullName => GetFullName(this);

        public static string GetFullName(Medecin med)
        {
            if (med?.Nom == null) return null;

            switch (med.Type)
            {
                case 0:
                    return $"Dr.{med.Nom}/G";
                case 1:
                    return $"Dr.{med.Nom}/S";
                case 2:
                    return $"Dr.{med.Nom}/D";
                case 3:
                    return $"Dr.{med.Nom}/SF";
                default:
                    return null;

            }
        }

        public virtual ICollection<Ordonnance> Ordonnances { get; set; } = new Collection<Ordonnance>();
    }
}