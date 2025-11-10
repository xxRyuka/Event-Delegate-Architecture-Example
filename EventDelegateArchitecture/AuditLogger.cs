namespace EventDelegateArchitecture;

/// Giriş denemelerini (başarılı veya başarısız) dinleyen
/// ve loglayan bir abone (Subscriber) sınıfıdır.
public class AuditLogger
{
        /// UserLoggedIn olayına abone olacak olan metot.

        /// Neden imzası 'public void ...(object sender, AuthEventArgs e)'?
        /// ÇÜNKÜ: Bu metodun imzası, 'AuthEventHandler' delegesi (sözleşmesi)
        /// ile BİREBİR AYNI OLMALIDIR.
        /// Eğer imzası farklı olsaydı (örn: 'public void Log(string user)'),
        /// 'authService.UserLoggedIn += logger.OnUserLoggedIn;' satırı
        /// DERLEME HATASI (compile error) verirdi.
        
    public void OnUserLoggedIn(object sender, AuthEventArgs e)
    {
        Console.WriteLine($"[AUDIT LOG]: BAŞARILI GİRİŞ. Kullanıcı: {e.Username}, Zaman: {e.Timestamp}");
    }

    /// LoginFailed olayına abone olacak olan metot.
    /// Bu da aynı 'AuthEventHandler' sözleşmesine uyar.

    public void OnLoginFailed(object sender, AuthEventArgs e)
    {
        Console.WriteLine($"[AUDIT LOG]: *** BAŞARISIZ GİRİŞ DENEMESİ ***. Kullanıcı: {e.Username}, Zaman: {e.Timestamp}");
    }
    
    //
    public void LOG(object sender, AuthEventArgs e)
    {
        Console.WriteLine($"[LOG] Olayı Cagiran :   {sender.GetType().Name} detailed : {sender.ToString()}");
    }
}