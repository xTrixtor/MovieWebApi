using Org.BouncyCastle.Asn1;

namespace MyAwesomeWebApi.Models.Reset
{
    public class ResetUserPassword
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string ResetCode { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
