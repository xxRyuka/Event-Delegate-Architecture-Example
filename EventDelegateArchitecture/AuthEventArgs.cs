namespace EventDelegateArchitecture;

/// Bu sınıf, bir kimlik doğrulama olayı (örn: UserLoggedIn) tetiklendiğinde
/// abonelere gönderilecek veriyi taşımak için kullanılır.
/// 
/// Neden "EventArgs" sınıfından miras (inherit) alıyor?
/// Bu, .NET ekosisteminde bir kuraldır (convention). Bir sınıfın adının
/// "EventArgs" ile bitmesi ve bu sınıftan türemesi, onun bir olayla
/// ilgili veri paketi olduğunu gösterir. Zorunlu değildir ama "best practice"dir

// Evente 10* parametre vermek yerine Event Args ile paketliyoruz ve payloadi tasiyoruz 
public class AuthEventArgs : EventArgs
{
    // 'init' kullanıyoruz, çünkü bu veri oluşturulduktan sonra değişmemelidir (immutability).
    
    
        
        /// Olaya dahil olan kullanıcının adı.
        /// 
        /// Neden 'init;' kullanıldı?
        /// 'init;', bu özelliğin (property) sadece nesne oluşturulurken
        /// (yani "new AuthEventArgs { ... }") atanabileceği anlamına gelir.
        /// Oluşturulduktan sonra değiştirilemez (immutable).
        /// Bu, verinin aboneler tarafından yanlışlıkla değiştirilmesini engeller
        /// ve sistemi daha güvenli hale getirir.
    public string Username { get; init; }
    
        /// Olayın gerçekleştiği zaman damgası.
    public DateTime Timestamp { get; init; }
}

