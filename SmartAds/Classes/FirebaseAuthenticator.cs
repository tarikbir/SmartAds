using Firebase.Auth;
using System.Threading.Tasks;

namespace SmartAds
{
    public class FirebaseAuthenticator
    {
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }

        public async Task<string> CreateUserWithEmailPassword(string email, string password)
        {
            var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }

        public async Task SendPasswordReset(string email)
        {
            await FirebaseAuth.Instance.SendPasswordResetEmailAsync(email);
        }
    }
}