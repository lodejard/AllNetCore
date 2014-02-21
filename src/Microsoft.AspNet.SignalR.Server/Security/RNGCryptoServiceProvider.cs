#if K10 // TODO: Crypto
namespace System.Security.Cryptography
{
    public class RNGCryptoServiceProvider : IDisposable
    {
        public void Dispose()
        {
            
        }

        public void GetBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
#endif