namespace EventDelegateArchitecture;

/// Bu bir "delegate" (temsilci) tanımıdır.
/// 
/// Neden 'delegate' kullanıyoruz?
/// Bu, bir "metot imzası" için (yani metotların uyması gereken kural için)
/// YENİ BİR TİP (type) oluşturur. Tıpkı 'string', 'int' gibi.
/// 
/// Bu satırın okuması:
/// "Adı 'AuthEventHandler' olan yeni bir TİP oluşturuyorum.
///  Bu TİP'e uyan metotlar:
///  1. 'void' döndürmelidir (yani bir cevap vermemelidir).
///  2. İlk parametre olarak 'object sender' almalıdır (Olayı kimin başlattığı).
///  3. İkinci parametre olarak 'AuthEventArgs e' almalıdır (Taşınan veri).
public delegate void AuthEventHandler(object sender, AuthEventArgs e);