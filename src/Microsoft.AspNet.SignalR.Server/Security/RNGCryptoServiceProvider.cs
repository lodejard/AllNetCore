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
            var random = new Random((int)DateTime.UtcNow.Ticks);
            random.NextBytes(bytes);
        }
    }
}
#endif