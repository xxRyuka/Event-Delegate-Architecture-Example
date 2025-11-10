namespace EventDelegateArchitecture;

/// Kimlik doğrulama işlemlerini yürüten ve ilgili olayları
/// (UserLoggedIn, LoginFailed) yayınlayan sınıftır (Publisher).
public class AuthService
{
    // 3.1. OLAYLARIN (Events) TANIMLANMASI
    
    /// <summary>
        /// Kullanıcı başarıyla giriş yaptığında tetiklenecek olay.
        /// Neden 'public event AuthEventHandler ...'?
        /// 
        /// 'public': Dışarıdan (Program.cs gibi) erişilip abone (+=) olunabilsin diye.
        /// 'AuthEventHandler': Bu olaya SADECE 'AuthEventHandler' sözleşmesine
        ///                   uyan metotların abone olabileceğini zorunlu kılar.
        ///                   (Yani 'void Metot(object s, AuthEventArgs e)' imzasına uyanlar).
        /// 'event' (EN ÖNEMLİSİ): Bu, 'UserLoggedIn' değişkenini KORUMA ALTINA ALIR.
        ///       Bu sayede dışarıdaki sınıflar (örn: Program.cs):
        ///       1. SADECE '+=' (abone ol) ve '-=' (abonelikten çık) yapabilir.
        ///       2. ASLA 'authService.UserLoggedIn = null;' diyerek tüm abone listesini silemez.
        ///       3. ASLA 'authService.UserLoggedIn(this, args);' diyerek olayı YETKİSİZCE tetikleyemez.
        ///       Olayı tetikleme hakkı SADECE bu sınıfın (AuthService) kendisindedir. => Güvenlik önlemi
    /// </summary>
    
    
    /// tanımladıgımız eventleri aşağıda Login fonksiyonuna bağlı OnUserLogin ve LoginFailed fonksiyonlarının en altında tetikliyoruz
    
    
    // tetikleme anında olan durum =>
                        // authService.<EventName> += funcName
                        // => ama bu fonksiyonun default parametreleriyle EvendHandler parametrelerinin ayni olmasi gerekiyor 
                // bu sekilde Evente (olaya) abone olur ve Evetin Method listesine eklenir (method refereansiyla) 
                // Olay tetiklendiğinde mantiken calisacak olan kod 
    
                /// <summary>
                ///  foreach (var methodReference in <EventName>MethodList)
                /// {
                ///     'this' : sender
                ///     'args' : e parametresine gonderilir ve
                ///
                ///     methodReference(sender,args) ile methodlarin hepsi calıstırılır
                /// }
                /// </summary>
    public event AuthEventHandler UserLoggedIn;

    /// <summary>
      /// Kullanıcı giriş denemesi başarısız olduğunda tetiklenecek olay.
    /// </summary>
    public event AuthEventHandler LoginFailed;

    // Basit bir kullanıcı/şifre veritabanı simülasyonu
    private Dictionary<string, string> users = new Dictionary<string, string>
    {
        { "admin", "12345" },
        { "sena", "abc" }
    };

    // 3.2. İŞ MANTIĞI (Business Logic)

    /// Kullanıcının giriş yapmasını sağlayan ana metot.
    public void Login(string username, string password)
    {
        Console.WriteLine($"\nİŞLEM: '{username}' için giriş denemesi yapılıyor...");
        
        // Simülasyon: Parola kontrolü
        if (users.TryGetValue(username, out string storedPassword) && storedPassword == password)
        {
            // Başarılıysa, ilgili olayı tetiklemesi için 'Raiser' metodu çağır
            OnUserLoggedIn(username);
        }
        else
        {
            // Başarısızsa, diğer olayı tetikle
            OnLoginFailed(username);
        }
    }

    // 3.3. OLAY TETİKLEYİCİ METOTLAR (Raiser Methods)

    /// <summary>
    /// UserLoggedIn olayını güvenli bir şekilde tetikleyen metot.
    /// Neden 'protected virtual'?
    /// 'protected': Bu sınıftan miras (inheritance) alan alt sınıfların
    ///              bu olayı tetikleyebilmesine izin verir.
    /// 'virtual': Alt sınıfların bu tetikleme davranışını 'override'
    ///            edebilmesine (ezmesine/değiştirmesine) izin verir.
    /// Bu, .NET'te standart bir tasarım desenidir ve esneklik sağlar.
    /// </summary>
    protected virtual void OnUserLoggedIn(string username)
    {
        // Neden 'if (UserLoggedIn != null)' KONTROLÜ VAR?
        // ÇOK ÖNEMLİ: Eğer bu olaya HİÇ KİMSE abone olmadıysa (+= yapmadıysa),
        // 'UserLoggedIn' temsilcisi 'null' olur. 'null' bir şeyi
        // metot gibi çağırmaya çalışmak 'NullReferenceException' hatasına neden olur.
        // Bu kontrol "En az bir abone var mı?" diye bakar.
        if (UserLoggedIn != null)
        {
            // 1. Veri paketini (EventArgs) oluştur
            var args = new AuthEventArgs 
            { 
                Username = username, 
                Timestamp = DateTime.Now 
            };
            
            // 2. Olayı tetikle! (Tüm aboneleri çağır)
            // 'this': Olayı kimin başlattığını ('sender') belirtir. (Bu sınıfın kendisi)
            // 'args': Olayla birlikte gönderilen veri paketi.
            UserLoggedIn(this, args);
        }
    }

    /// <summary>
    /// LoginFailed olayını güvenli bir şekilde tetikleyen metot.
    /// </summary>
    protected virtual void OnLoginFailed(string username)
    {
        // Aynı null kontrolü burada da geçerli.
        if (LoginFailed != null)
        {
            var args = new AuthEventArgs 
            { 
                Username = username, 
                Timestamp = DateTime.Now 
            };
            LoginFailed(this, args);
        }
    }
}