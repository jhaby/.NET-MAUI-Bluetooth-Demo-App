using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Newtonsoft.Json;
using SmartAsthmaAssistane.Models;
using System.Diagnostics;

namespace SmartAsthmaAssistane.Services;

public class FirebaseService
{
    readonly FirebaseAuthClient authClient;
    private readonly string apiKey = "AIzaSyApCMZNqwEMEYXnLvdIb_YFlDbAliI1ikg";
    private readonly string domain = "smartasthmaassistant.firebaseapp.com";
    public FirebaseService()
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = apiKey,
            AuthDomain = domain,
            Providers =
            [
                new GoogleProvider().AddScopes("email"),
                new EmailProvider()
            ],
            UserRepository = new FileUserRepository("SmartAsthmaUsers")
        };
        authClient = new FirebaseAuthClient(config);
    }
    public async Task<bool> SigninWithEmail(string email, string password)
    {
        try
        {
            var credentials = EmailProvider.GetCredential(email, password);
            var result = await authClient.SignInWithCredentialAsync(credentials);
            if (!string.IsNullOrEmpty(await result.User.GetIdTokenAsync()))
            {
                Preferences.Set(Constants.firebaseUser, JsonConvert.SerializeObject(result.User.Info));
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }
    public async Task<bool> SigninWithGoogle()
    {
        try
        {
            var credentials = GoogleProvider.GetCredential("sukalicare");
            var result = await authClient.SignInWithCredentialAsync(credentials);
            if (!string.IsNullOrEmpty(result.User.Uid))
            {
                Preferences.Set("user", JsonConvert.SerializeObject(result.User.Info));
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> CreateFirebaseAccount(string email, string password)
    {
        try
        {
            var result = await authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            if (!string.IsNullOrEmpty(await result.User.GetIdTokenAsync()))
            {
                return await SigninWithEmail(email, password);
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }
}
