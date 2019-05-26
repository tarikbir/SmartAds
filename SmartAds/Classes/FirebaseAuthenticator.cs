using Firebase.Auth;
using System.Threading.Tasks;

namespace SmartAds
{
    public class FirebaseAuthenticator
    {
        public async Task<FirebaseUser> LoginWithEmailPassword(string email, string password)
        {
            FirebaseUser user;
            try
            {
                var authResult = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                user = authResult.User;
            }
            catch
            {
                user = null;
            }
            return user;
        }

        public async Task<FirebaseUser> CreateUserWithEmailPassword(string email, string password)
        {
            FirebaseUser user;
            try
            {
                var authResult = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                user = authResult.User;
            }
            catch
            {
                user = null;
            }
            return user;
        }

        public async Task<bool> SendPasswordReset(string email)
        {
            try
            {
                await FirebaseAuth.Instance.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}