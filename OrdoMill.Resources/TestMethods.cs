using System.Globalization;
using System.Resources;

namespace OrdoMill.GlobalResources;

public class TestMethods
{
    public void TestResources()
    {
        ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        foreach (string entry in resourceSet.OfType<string>())
        {
            var a = entry;
        }
    }
}
