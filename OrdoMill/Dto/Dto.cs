//using System;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;

using PropertyChanged;

namespace OrdoMill.Dto
{
    [ImplementPropertyChanged]
    public class Assure : Data.Model.Assure
    {
    }

    [ImplementPropertyChanged]
    public class Bordereau : Data.Model.Bordereau
    {
        public int OrdonancesCount { get; set; }
        public int FacturesCount { get; set; }
        public double? MontantGlobal { get; set; }

    }

    [ImplementPropertyChanged]
    public class Info : Data.Model.Info
    {
    }

    [ImplementPropertyChanged]
    public class Facture : Data.Model.Facture
    {
        public int OrdonancesCount { get; set; }
    //    public double? MontantGlobal { get; set; }

    }

    [ImplementPropertyChanged]
    public class Forme : Data.Model.Forme
    {
    }

    [ImplementPropertyChanged]
    public class Historique : Data.Model.Historique
    {
    }

    [ImplementPropertyChanged]
    public class Medecin : Data.Model.Medecin
    {
    }

    [ImplementPropertyChanged]
    public class Medicament : Data.Model.Medicament
    {
    }

    [ImplementPropertyChanged]
    public class Pathologie : Data.Model.Pathologie
    {
    }

    [ImplementPropertyChanged]
    public class MedOrd : Data.Model.MedOrd
    {
    }

    [ImplementPropertyChanged]
    public class Ordonnance : Data.Model.Ordonnance
    {
    }

    [ImplementPropertyChanged]
    public class Patient : Data.Model.Patient
    {
    }

    [ImplementPropertyChanged]
    public class User : Data.Model.User
    {
    }

    [ImplementPropertyChanged]
    public class Operation : Data.Model.Operation
    {
    }

//    #region POCO classes

//    // Assures
//     public class Assure
//    {
//        public Assure()
//        {
//            Patients = new List<Patient>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string Matricule { get; set; } // Matricule
//        public string Nom { get; set; } // Nom
//        public string Prenom { get; set; } // Prenom
//        public string Adresse { get; set; } // Adresse
//        public DateTime? DateNaissance { get; set; } // DateNaissance
//        public DateTime? Jusquea { get; set; } // Jusquea
//        public string Lieu { get; set; } // Lieu
//        public string Tel { get; set; } // Tel
//        public string Grade { get; set; } // Grade
//        public bool IsContractial { get; set; } // IsContractial
//        public string Ccp { get; set; } // Ccp
//        public string ArNom { get; set; } // ArNom
//        public string ArPrenom { get; set; } // ArPrenom
//        public string Note { get; set; } // Note
//        public bool Suspende { get; set; } // Suspende
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt
//        public string FullName { get; set; } // FullName

//        // Reverse navigation
//        public virtual ICollection<Patient> Patients { get; set; } // Patients.FK_dbo.Patients_dbo.Assures_AssureId
//    }

//    // Bordereaux
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Bordereau
//    {
//        public Bordereau()
//        {
//            Factures = new List<Facture>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public DateTime? Date { get; set; } // Date
//        public int Number { get; set; } // Number
//        public bool IsFinished { get; set; } // IsFinished
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Facture> Factures { get; set; }
//        // Factures.FK_dbo.Factures_dbo.Bordereaux_BordereauId
//    }

//    // Factures
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Facture
//    {
//        public Facture()
//        {
//            Ordonnances = new List<Ordonnance>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public int Number { get; set; } // Number
//        public bool Chronic { get; set; } // Chronic
//        public bool Out { get; set; } // Out
//        public int? BordereauId { get; set; } // BordereauId
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Ordonnance> Ordonnances { get; set; }
//        // Ordonnances.FK_dbo.Ordonnances_dbo.Factures_FactureId

//        // Foreign keys
//        public virtual Bordereau Bordereau { get; set; } // FK_dbo.Factures_dbo.Bordereaux_BordereauId
//    }

//    // Formes
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Forme
//    {
//        public Forme()
//        {
//            Medicaments = new List<Medicament>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string Name { get; set; } // Name
//        public string Abrg { get; set; } // Abrg
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Medicament> Medicaments { get; set; }
//        // Medicaments.FK_dbo.Medicaments_dbo.Formes_FormeId
//    }

//    // Historiques
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Historique
//    {
//        public int Id { get; set; } // Id (Primary key)
//        public int? PatientId { get; set; } // PatientId
//        public DateTime DateTime { get; set; } // DateTime
//        public string Note { get; set; } // Note
//        public int? OperationId { get; set; } // OperationId
//        public int? UserId { get; set; } // UserId
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Foreign keys
//        public virtual Operation Operation { get; set; } // FK_dbo.Historiques_dbo.Operations_OperationId
//        public virtual Patient Patient { get; set; } // FK_dbo.Historiques_dbo.Patients_PatientId
//        public virtual User User { get; set; } // FK_dbo.Historiques_dbo.Users_UserId
//    }

//    // Infoes
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Info
//    {
//        public int Id { get; set; } // Id (Primary key)
//        public string Pharmacie { get; set; } // Pharmacie
//        public string Adresse { get; set; } // Adresse
//        public string Tel { get; set; } // Tel
//        public string Fax { get; set; } // Fax
//        public string CodeFiscal { get; set; } // CodeFiscal
//        public string Article { get; set; } // Article
//        public string Rc { get; set; } // Rc
//        public string Ccp { get; set; } // Ccp
//        public string Doit { get; set; } // Doit
//        public string Code { get; set; } // Code
//        public string Region { get; set; } // Region
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt
//    }

//    // Medecins
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Medecin
//    {
//        public Medecin()
//        {
//            Ordonnances = new List<Ordonnance>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string Nom { get; set; } // Nom
//        public string Tel { get; set; } // Tel
//        public string Adresse { get; set; } // Adresse
//        public int Type { get; set; } // Type
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Ordonnance> Ordonnances { get; set; }
//        // Ordonnances.FK_dbo.Ordonnances_dbo.Medecins_MedecinId
//    }

//    // Medicaments
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Medicament
//    {
//        public Medicament()
//        {
//            MedOrds = new List<MedOrd>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string CnasId { get; set; } // CnasId
//        public string Nom { get; set; } // Nom
//        public string Dci { get; set; } // Dci
//        public string Dose { get; set; } // Dose
//        public string Boite { get; set; } // Boite
//        public string Tr { get; set; } // Tr
//        public string Unite { get; set; } // Unite
//        public bool Remboursable { get; set; } // Remboursable
//        public bool Controle { get; set; } // Controle
//        public int? FormeId { get; set; } // FormeId
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<MedOrd> MedOrds { get; set; } // MedOrds.FK_dbo.MedOrds_dbo.Medicaments_MedicamentId

//        // Foreign keys
//        public virtual Forme Forme { get; set; } // FK_dbo.Medicaments_dbo.Formes_FormeId
//    }

//    // MedOrds
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class MedOrd
//    {
//        public int Id { get; set; } // Id (Primary key)
//        public int OrdonnanceId { get; set; } // OrdonnanceId
//        public int MedicamentId { get; set; } // MedicamentId
//        public int Duree { get; set; } // Duree
//        public int Quantite { get; set; } // Quantite
//        public double Ppa { get; set; } // Ppa
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Foreign keys
//        public virtual Medicament Medicament { get; set; } // FK_dbo.MedOrds_dbo.Medicaments_MedicamentId
//        public virtual Ordonnance Ordonnance { get; set; } // FK_dbo.MedOrds_dbo.Ordonnances_OrdonnanceId
//    }

//    // Operations
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Operation
//    {
//        public Operation()
//        {
//            Historiques = new List<Historique>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string Name { get; set; } // Name
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Historique> Historiques { get; set; }
//        // Historiques.FK_dbo.Historiques_dbo.Operations_OperationId
//    }

//    // Ordonnances
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Ordonnance
//    {
//        public Ordonnance()
//        {
//            MedOrds = new List<MedOrd>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public DateTime ServiDate { get; set; } // ServiDate
//        public DateTime SoineDate { get; set; } // SoineDate
//        public int? MedecinId { get; set; } // MedecinId
//        public int PatientId { get; set; } // PatientId
//        public int? FactureId { get; set; } // FactureId
//        public bool IsChronique { get; set; } // IsChronique
//        public bool IsHorsRegion { get; set; } // IsHorsRegion
//        public string PathologieCode { get; set; } // PathologieCode
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt
//        public int? PathologieId { get; set; } // Pathologie_Id

//        // Reverse navigation
//        public virtual ICollection<MedOrd> MedOrds { get; set; } // MedOrds.FK_dbo.MedOrds_dbo.Ordonnances_OrdonnanceId

//        // Foreign keys
//        public virtual Facture Facture { get; set; } // FK_dbo.Ordonnances_dbo.Factures_FactureId
//        public virtual Medecin Medecin { get; set; } // FK_dbo.Ordonnances_dbo.Medecins_MedecinId
//        public virtual Pathology Pathology { get; set; } // FK_dbo.Ordonnances_dbo.Pathologies_Pathologie_Id
//        public virtual Patient Patient { get; set; } // FK_dbo.Ordonnances_dbo.Patients_PatientId
//    }

//    // Pathologies
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Pathology
//    {
//        public Pathology()
//        {
//            Ordonnances = new List<Ordonnance>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string Code { get; set; } // Code
//        public string Nom { get; set; } // Nom
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Ordonnance> Ordonnances { get; set; }
//        // Ordonnances.FK_dbo.Ordonnances_dbo.Pathologies_Pathologie_Id
//    }

//    // Patients
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class Patient
//    {
//        public Patient()
//        {
//            Historiques = new List<Historique>();
//            Ordonnances = new List<Ordonnance>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string Nom { get; set; } // Nom
//        public string Prenom { get; set; } // Prenom
//        public DateTime? DateNaissance { get; set; } // DateNaissance
//        public int Alliance { get; set; } // Alliance
//        public DateTime? Du { get; set; } // Du
//        public DateTime? Au { get; set; } // Au
//        public int AssureId { get; set; } // AssureId
//        public bool Suspende { get; set; } // Suspende
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Historique> Historiques { get; set; }
//        // Historiques.FK_dbo.Historiques_dbo.Patients_PatientId
//        public virtual ICollection<Ordonnance> Ordonnances { get; set; }
//        // Ordonnances.FK_dbo.Ordonnances_dbo.Patients_PatientId

//        // Foreign keys
//        public virtual Assure Assure { get; set; } // FK_dbo.Patients_dbo.Assures_AssureId
//    }

//    // Users
//    [GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
//    public class User
//    {
//        public User()
//        {
//            Historiques = new List<Historique>();
//        }

//        public int Id { get; set; } // Id (Primary key)
//        public string UserName { get; set; } // UserName
//        public string Password { get; set; } // Password
//        public string FullName { get; set; } // FullName
//        public bool IsAdmin { get; set; } // IsAdmin
//        public bool IsWork { get; set; } // IsWork
//        public bool AllowAdd { get; set; } // AllowAdd
//        public bool AllowUpdate { get; set; } // AllowUpdate
//        public bool AllowPrintDocs { get; set; } // AllowPrintDocs
//        public DateTime? CreatedAt { get; set; } // CreatedAt
//        public string CreatedBy { get; set; } // CreatedBy
//        public string ModifiedBy { get; set; } // ModifiedBy
//        public DateTime? ModifiedAt { get; set; } // ModifiedAt

//        // Reverse navigation
//        public virtual ICollection<Historique> Historiques { get; set; }
//        // Historiques.FK_dbo.Historiques_dbo.Users_UserId
//    }

//    #endregion
}