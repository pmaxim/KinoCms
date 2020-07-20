using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace KinoCMS.Server.Infrastructure
{
    public static class Certificate
    {
        public static X509Certificate2 Get()
        {
            var assembly = typeof(Certificate).GetTypeInfo().Assembly;
            //file properties - build action: Embedded Resource
            //var names = assembly.GetManifestResourceNames();
            /***********************************************************************************************
             *  Please note that here we are using a local certificate only for testing purposes. In a 
             *  real environment the certificate should be created and stored in a secure way, which is out
             *  of the scope of this project.
             **********************************************************************************************/
            using var stream = assembly.GetManifestResourceStream("KinoCMS.Server.Infrastructure.token_signing.pfx");
            return new X509Certificate2(ReadStream(stream), "Max1967!",
                X509KeyStorageFlags.MachineKeySet
                | X509KeyStorageFlags.PersistKeySet
                | X509KeyStorageFlags.Exportable);
        }

        private static byte[] ReadStream(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using var ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}
