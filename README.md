-----

# C\# Event ve Delegate Mimarisi: Detaylı Bir Örnek

Bu proje, C\# dilindeki **Event (Olay)** ve **Delegate (Temsilci)** mimarisini, "Publisher-Subscriber" (Yayıncı-Abone) desenini kullanarak açıklamak için hazırlanmış öğretici bir konsol uygulamasıdır.

Senaryo olarak, sorumlulukların net bir şekilde ayrıldığı (Separation of Concerns) basit bir **Kullanıcı Doğrulama Sistemi (`AuthService`)** kullanılmıştır.

-----

##  Temel Amaç: Ayrıştırma (Decoupling)

Bu mimarinin temel amacı **Ayrıştırma**'dır. Yani, birbiriyle iletişim kurması gereken sınıfların, birbirleri hakkında doğrudan bilgi sahibi olmasını (sıkı sıkıya bağlılık - *tight coupling*) engellemektir.

**Kötü Tasarım (Sıkı Sıkıya Bağlı - Tight Coupling):**
`AuthService` sınıfı, bir kullanıcı giriş yaptığında `AuditLogger` ve `StatisticsService` sınıflarını *doğrudan tanımak* ve onların metotlarını *doğrudan çağırmak* zorunda kalırdı.

```csharp
// KÖTÜ TASARIM ÖRNEĞİ
public class AuthService
{
    private AuditLogger _logger = new();
    private StatisticsService _stats = new();
    
    public void Login()
    {
        // ...giriş başarılı...
        _logger.OnUserLoggedIn(...); // AuthService, Logger'ı tanımak zorunda
        _stats.OnUserLoggedIn(...); // AuthService, Stats'ı tanımak zorunda
    }
}
```

Bu tasarımda, yeni bir `EmailService` eklemek isteseydik, `AuthService`'in kodunu açıp *değiştirmek* zorunda kalırdık. Bu, **Open/Closed Principle** (Genişlemeye Açık, Değişime Kapalı Prensibi) ihlalidir.

**İyi Tasarım (Ayrıştırılmış - Decoupled - Event Mimarisi):**
`AuthService` (Yayıncı), sadece "Biri giriş yaptı\!" diye bir sinyal yayınlar. Kimin dinlediğini, hatta birinin dinleyip dinlemediğini bile bilmez. `AuditLogger` ve `StatisticsService` (Aboneler) bu sinyali dinler ve kendi işlerini yaparlar.

-----

## 🛠️ Mimarinin 3 Temel Bileşeni

Bu "Yayıncı-Abone" desenini C\#'ta kurmak için 3 temel yapıya ihtiyacımız vardır:

1.  **Delegate (Temsilci) - "SÖZLEŞME"**

      * **Nedir?** Bir metot imzası için "tip tanımıdır". Yani, "Bana abone olacak metotların uyması gereken kural nedir?" sorusunun cevabıdır.
      * **Projedeki Dosya:** `AuthEventHandler.cs`
      * **Kuralımız:** `void Isim(object sender, AuthEventArgs e)` imzasına uyan herhangi bir metot.

2.  **EventArgs (Olay Verisi) - "VERİ PAKETİ"**

      * **Nedir?** Olay tetiklendiğinde, yayıncıdan abonelere gönderilecek veriyi (payload) taşıyan basit bir sınıftır.
      * **Projedeki Dosya:** `AuthEventArgs.cs`
      * **Verimiz:** `Username` (Kim giriş yaptı?) ve `Timestamp` (Ne zaman giriş yaptı?).

3.  **Event (Olay) - "GÜVENLİ YAYIN MEKANİZMASI"**

      * **Nedir?** Yayıncı sınıfın (`AuthService`) içinde, tanımladığımız `delegate` tipini kullanan özel bir değişkendir.
      * **Güvenliği:** `event` anahtar kelimesi, bu değişkene dışarıdan sadece `+=` (Abone Ol) ve `-=` (Abonelikten Çık) yapılmasına izin verir. Dışarıdan bir sınıfın olayı tetiklemesini (`()`) veya tüm abone listesini sıfırlamasını (`= null`) engeller.

-----

##  Proje Dosya Yapısı

Proje, sorumlulukları ayırmak için birden fazla dosyaya bölünmüştür:

  * **`AuthEventArgs.cs`**: Olay tetiklendiğinde taşınacak veri paketini tanımlar.
  * **`AuthEventHandler.cs`**: Abonelerin uyması gereken metot imzası sözleşmesini (delegate) tanımlar.
  * **`AuthService.cs`**: **Yayıncı (Publisher)**. Asıl işi (kimlik doğrulama) yapar ve `UserLoggedIn` / `LoginFailed` olaylarını yayınlar. Abonelerin kim olduğundan haberi yoktur.
  * **`AuditLogger.cs`**: **Abone (Subscriber)**. Her iki olayı da (`UserLoggedIn`, `LoginFailed`) dinler ve konsola log yazar.
  * **`StatisticsService.cs`**: **Abone (Subscriber)**. Sadece `UserLoggedIn` olayını dinler ve toplam giriş sayısını günceller.
  * **`Program.cs`**: **Orkestratör**. Tüm nesneleri (Yayıncı ve Aboneler) oluşturan ve `+=` operatörü ile abonelik bağlantılarını kuran yerdir.

-----

##  Nasıl Çalışır? "Sihrin" Arkasındaki Mekanizma

"Sihirli" gibi görünen bu bağlantının arkasında, yönetilen basit bir liste mekanizması yatar.

### 1\. Adım: Abone Olma (Bağlantı Anı `+=`)

`Program.cs` içinde şu satırı yazdığımızda:

```csharp
// Program.cs
authService.UserLoggedIn += logger.OnUserLoggedIn;
```

**Arka Planda Olanlar:**
`event` (olay) aslında arkada bir **metot referansları listesi** tutar.

1.  Bu satır, `logger.OnUserLoggedIn` metodunun **hafızadaki referansını** (adresini) alır.
2.  Bu referansı, `authService` nesnesinin içindeki `UserLoggedIn` olayının "çağrı listesine" (invocation list) **ekler**.
3.  Aynı olaya `stats.OnUserLoggedIn` için de `+=` yaptığımızda, o referans da listenin sonuna eklenir.

**Listenin Son Durumu:**
`UserLoggedIn_Listesi = [ logger.OnUserLoggedIn'in adresi , stats.OnUserLoggedIn'in adresi ]`

### 2\. Adım: Tetikleme Anı (Sinyal Verme `(this, args)`)

`AuthService.cs` içinde `Login` metodu başarılı olduğunda, `OnUserLoggedIn` metodu çalışır ve şu satıra gelir:

```csharp
// AuthService.cs -> OnUserLoggedIn() metodu içinde
var args = new AuthEventArgs { Username = username, Timestamp = DateTime.Now };
UserLoggedIn(this, args); // <-- TETİKLEME ANI
```

**Arka Planda Olanlar (Sizin de Harika Not Ettiğiniz Gibi):**
Bu `UserLoggedIn(this, args);` satırı, C\# derleyicisi için "O listedeki herkesi çağır\!" komutudur.

Derleyicinin bu satırı gördüğünde yaptığı (kavramsal) iş tam olarak budur:

```csharp
// UserLoggedIn(this, args) çağrısının
// arka planda yaptığı kavramsal işlem:

// 1. "Liste boş mu?" diye kontrol et (Null check)
if (UserLoggedIn_Listesi != null)
{
    // 2. Liste doluysa, listedeki HER BİR metot referansı için döngü kur.
    foreach (var methodReference in UserLoggedIn_Listesi)
    {
        // 3. O metodu, 'this' ve 'args' parametrelerini
        //    aktararak sırayla çalıştır.
        
        // 'this' (AuthService) -> 'sender' parametresine
        // 'args' (AuthEventArgs) -> 'e' parametresine
        
        methodReference(this, args);
    }
}
```

**Sonuç:**

  * Önce `logger.OnUserLoggedIn(this, args)` çalışır ve log atılır.
  * Sonra `stats.OnUserLoggedIn(this, args)` çalışır ve istatistik güncellenir.

İşte "sihir" budur. `+=` ile bir listeye metot eklemek ve olayı tetiklediğinizde o listedeki tüm metotları bir döngüyle çağırmak.

-----

##  Nasıl Çalıştırılır?

1.  Bu projeyi klonlayın veya indirin.
2.  Bir terminalde veya komut isteminde proje klasörüne gidin.
3.  Projeyi çalıştırmak için aşağıdaki komutu girin:

<!-- end list -->

```bash
dotnet run
```

-----

##  Öğrenilen Temel İlkeler

  * **Ayrıştırma (Decoupling):** Yayıncı ve Aboneler birbirini tanımaz.
  * **Sorumlulukların Ayrılması (Separation of Concerns):** `AuthService` (İş Mantığı), `AuditLogger` (Loglama), `StatisticsService` (İstatistik) sınıflarının her biri tek bir iş yapar.
  * **Genişlemeye Açık, Değişime Kapalı (Open/Closed Principle):** `AuthService`'in kodunu *değiştirmeden*, `Program.cs`'e `+=` ile yeni aboneler (örn: `EmailService`) ekleyerek sistemi *genişletebiliriz*.
