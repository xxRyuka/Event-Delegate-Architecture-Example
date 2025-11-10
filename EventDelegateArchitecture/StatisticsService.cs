namespace EventDelegateArchitecture;

/// <summary>
    /// Sadece BAŞARILI girişleri dinleyip sayan bir abone (Subscriber) sınıfıdır.
/// </summary>
public class StatisticsService
{
    private int _totalLogins = 0;

    /// <summary>
    /// UserLoggedIn olayına abone olacak olan metot.
    /// Bu da 'AuthEventHandler' sözleşmesine (imzasına) uymak zorundadır.
    /// </summary>
    public void OnUserLoggedIn(object sender, AuthEventArgs e)
    {
        _totalLogins++;
        Console.WriteLine($"[İSTATİSTİK]: Toplam başarılı giriş sayısı güncellendi: {_totalLogins} ");
    }

    // NOT: Bu sınıf 'LoginFailed' olayını umursamıyor.
    // Bu yüzden 'OnLoginFailed' diye bir metodu yok ve o olaya abone olmayacak.
    // Mimari buna mükemmel şekilde izin verir (Decoupling).
}