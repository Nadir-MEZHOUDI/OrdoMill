using System.ComponentModel;
using System.Xml.Serialization;

namespace OrdoMill.Serializables;

[Serializable]
public sealed class SerializableAssure
{
    public string Matrecul { get; set; }

    public string Nom { get; set; }

    public string Prenom { get; set; }

    public string Adresse { get; set; }

    public string DateNaissance { get; set; }

    public string Jusquea { get; set; }

    public string Lieu { get; set; }

    public string Tel { get; set; }

    public string Grade { get; set; }

    [DefaultValue(false)]
    public bool IsContractial { get; set; }

    [DefaultValue("0")]
    public string Ccp { get; set; }

    public string ArNom { get; set; }

    public string ArPrenom { get; set; }

    public string Note { get; set; }

    [DefaultValue(false)]
    public bool Suspende { get; set; }

    [XmlArray(nameof(Patients))]
    public List<SerializablePatient> Patients { get; set; } = new List<SerializablePatient>();
}