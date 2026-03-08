using System;
using System.Windows.Markup;

namespace OrdoMill.GlobalResources.Res
{
    public class Res : MarkupExtension
    {
        public Res(ResEnum a)
        {
            Key = a;
        }

        private ResEnum Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Properties.Resources.ResourceManager.GetString(Key.ToString());
        }


    }

    public enum ResEnum
    {
    }
}