using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace SmartApp.Helpers.Bases
{
    [AddINotifyPropertyChangedInterface]
    public class EntityBase : IEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [NotMapped]
        [DefaultValue(true)]
        public bool IsCheck { get; set; }
    }

    public interface IEntity
    {
        [Key]
        int Id { get; set; }

        DateTime? CreatedAt { get; set; }

        string CreatedBy { get; set; }

        string ModifiedBy { get; set; }

        DateTime? ModifiedAt { get; set; }

        [NotMapped]
        bool IsCheck { get; set; }
    }
}
