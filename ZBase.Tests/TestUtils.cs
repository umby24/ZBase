using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ZBase.Tests {
    public class TestUtils {
        public static string LoadResourceFile(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            var resourceName = "ZBase.Tests.Common." + name;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }

            throw new Exception("An error occured trying to read the resource");
        }
    }
}
