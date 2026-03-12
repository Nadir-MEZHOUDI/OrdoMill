using PropertyChanged;

namespace OrdoMill.Dto;

[AddINotifyPropertyChangedInterface]
public class AssureDto
{
    public int Id { get; set; }
    public string Matricule { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Adresse { get; set; }
    public DateTime? DateNaissance { get; set; }
    public DateTime? Jusquea { get; set; }
    public string Lieu { get; set; }
    public string Tel { get; set; }
    public string Grade { get; set; }
    public bool IsContractial { get; set; }
    public string Ccp { get; set; }
    public string ArNom { get; set; }
    public string ArPrenom { get; set; }
    public string Note { get; set; }
    public bool Suspende { get; set; }
    public string FullName => $"{Nom} {Prenom}";
    public bool Activé => !Suspende;
    public List<PatientDto> Patients { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class PatientDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public DateTime? DateNaissance { get; set; }
    public int Alliance { get; set; }
    public string FullName => $"{Nom} {Prenom}";
    public DateTime? Du { get; set; }
    public DateTime? Au { get; set; }
    public int AssureId { get; set; }
    public AssureDto Assure { get; set; }
    public bool Suspende { get; set; }
    public bool Activé => !Suspende;
}

[AddINotifyPropertyChangedInterface]
public class OrdonnanceDto
{
    public int Id { get; set; }
    public DateTime ServiDate { get; set; }
    public DateTime SoineDate { get; set; }
    public int? MedecinId { get; set; }
    public MedecinDto Medecin { get; set; }
    public int PatientId { get; set; }
    public PatientDto Patient { get; set; }
    public int? FactureId { get; set; }
    public FactureDto Facture { get; set; }
    public bool IsChronique { get; set; }
    public bool IsHorsRegion { get; set; }
    public string PathologieCode { get; set; }
    public double? Montant { get; set; }
    public List<MedOrdDto> Medicaments { get; set; }
    public int? Vignettes => Medicaments?.Sum(med => med.Quantite);
    public List<string> Problems { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class FactureDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public bool Chronic { get; set; }
    public bool Out { get; set; }
    public int? BordereauId { get; set; }
    public BordereauDto Bordereau { get; set; }
    public int OrdonancesCount { get; set; }
    public List<OrdonnanceDto> Ordonnances { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class BordereauDto
{
    public int Id { get; set; }
    public DateTime? Date { get; set; }
    public int Number { get; set; }
    public bool IsFinished { get; set; }
    public int OrdonancesCount { get; set; }
    public int FacturesCount { get; set; }
    public double? MontantGlobal { get; set; }
    public List<FactureDto> Factures { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class MedecinDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Tel { get; set; }
    public string Adresse { get; set; }
    public int Type { get; set; }
    public string FullName => Nom;
}

[AddINotifyPropertyChangedInterface]
public class MedicamentDto
{
    public int Id { get; set; }
    public string CnasId { get; set; }
    public string Nom { get; set; }
    public string Dci { get; set; }
    public string Dose { get; set; }
    public string Boite { get; set; }
    public string Tr { get; set; }
    public string Unite { get; set; }
    public bool Remboursable { get; set; }
    public bool Controle { get; set; }
    public int? FormeId { get; set; }
    public FormeDto Forme { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class MedOrdDto
{
    public int Id { get; set; }
    public int OrdonnanceId { get; set; }
    public int MedicamentId { get; set; }
    public int Duree { get; set; }
    public int Quantite { get; set; }
    public double Ppa { get; set; }
    public MedicamentDto Medicament { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class FormeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abrg { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class PathologieDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Nom { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsWork { get; set; }
    public bool AllowAdd { get; set; }
    public bool AllowUpdate { get; set; }
    public bool AllowPrintDocs { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class InfoDto
{
    public int Id { get; set; }
    public string Pharmacie { get; set; }
    public string Adresse { get; set; }
    public string Tel { get; set; }
    public string Fax { get; set; }
    public string CodeFiscal { get; set; }
    public string Article { get; set; }
    public string Rc { get; set; }
    public string Ccp { get; set; }
    public string Doit { get; set; }
    public string Code { get; set; }
    public string Region { get; set; }
}

[AddINotifyPropertyChangedInterface]
public class HistoriqueDto
{
    public int Id { get; set; }
    public int? PatientId { get; set; }
    public DateTime DateTime { get; set; }
    public string Note { get; set; }
    public int? OperationId { get; set; }
    public int? UserId { get; set; }
    public PatientDto Patient { get; set; }
    public UserDto User { get; set; }
}
