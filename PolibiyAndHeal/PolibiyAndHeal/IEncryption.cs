namespace PolybiiAndHill
{
    public interface IEncryption
    {
        public string Decrypt(string text, string pass);

        public string Encrypt(string text, string pass);
    }
}