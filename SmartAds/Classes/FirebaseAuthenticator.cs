using Firebase.Auth;
using System.Threading.Tasks;

namespace SmartAds
{
    public class FirebaseAuthenticator
    {
        public async Task<LocalUser> LoginWithEmailPassword(string email, string password)
        {
            LocalUser user;
            try
            {
                var authResult = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var tokenResult = await authResult.User.GetIdTokenAsync(false);
                user = new LocalUser(authResult, tokenResult.Token);
                user.Result = true;
            }
            catch
            {
                user = new LocalUser();
                user.Result = false;
            }
            return user;
        }

        public async Task<LocalUser> CreateUserWithEmailPassword(string email, string password)
        {
            LocalUser user;
            try
            {
                var authResult = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var tokenResult = await authResult.User.GetIdTokenAsync(false);
                user = new LocalUser(authResult, tokenResult.Token);
                user.Result = true;
            }
            catch
            {
                user = new LocalUser();
                user.Result = false;
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