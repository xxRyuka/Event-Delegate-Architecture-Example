using EventDelegateArchitecture; // Diğer dosyalarımızı (sınıflarımızı) kullanabilmek için

// 'namespace' gerekmez, en üst seviye (top-level) ifadeler kullanıyoruz.

Console.WriteLine("Event/Delegate Mimarisi - Kimlik Doğrulama Sistemi Başlatıldı.");

// ---------------------------------
// 1. ADIM: PARÇALARI OLUŞTUR (NESNE YARATMA)
// ---------------------------------

// Yayıncıyı oluştur
var authService = new AuthService();

// Aboneleri oluştur
var logger = new AuditLogger();
var stats = new StatisticsService();

// Şu anda bu 3 nesne tamamen bağımsızdır ve birbirini tanımaz.

// ---------------------------------
// 2. ADIM: ABONE OLMA (WIRING / BAĞLAMA)
// ---------------------------------

// Neden '+= logger.OnUserLoggedIn'?
// Bu satır, 'authService'e der ki: "Senin 'UserLoggedIn' olayın
// tetiklendiğinde, benim 'logger.OnUserLoggedIn' METODUMU da çağır."


// DİKKAT: Buraya 'logger.OnUserLoggedIn' YAZIYORUZ (Metodun kendisi).
//         Buraya 'new AuthEventArgs()' YAZMIYORUZ (Veri paketi).
// 'event' (tipi AuthEventHandler), METOT bekler.
authService.UserLoggedIn += logger.OnUserLoggedIn;

// Aynı 'UserLoggedIn' olayına ikinci bir abone daha ekliyoruz.
// 'AuthService'in haberi bile yok, o sadece listeye bir metot daha ekler.
authService.UserLoggedIn += stats.OnUserLoggedIn;

// 'LoginFailed' olayına ise SADECE 'logger'ı abone ediyoruz.
authService.LoginFailed += logger.OnLoginFailed;

authService.LoginFailed += logger.LOG;
authService.LoginFailed -= logger.LOG; // Burda Method listesindne cikartiyoruz ve loginFailed eventi tetiklendiğinde LOG methodu calismiyor artık

authService.UserLoggedIn += logger.LOG;
authService.UserLoggedIn += logger.LOG; // metohddu değil referansini tuttugu için 2 kere ekleyebiliyoruz


// eventler tetiklendiğinde += ile listesine eklenen metodları bir for döngüsüyle çalsıtırır (bu yüzden parametrelerin ayni olmasi gerekiyor )

// 3. ADIM: SİSTEMİ TETİKLE (SİMÜLASYON)

// 'authService.Login' metodunu çağırdığımızda, 'authService'
// işini yapacak ve (içeride) 'OnUserLoggedIn' veya 'OnLoginFailed'
// metotlarını çağıracak. Bu metotlar da 'if (Event != null)' kontrolünden
// geçip, abone listesindeki (logger ve stats) metotları tetikleyecek.
// Zincirleme reaksiyon budur.

char manage = 'a';

while (manage != 'q')
{
    Console.WriteLine("çıkmak için bir 'q' tusuna bas ");
    
    manage = Console.ReadKey().KeyChar;
    
    Console.WriteLine("username : ");
    string username = Console.ReadLine();

    Console.WriteLine("password : ");
    string password = Console.ReadLine();
    
    authService.Login(username, password);
}

Console.WriteLine("\nSimülasyon tamamlandı.");