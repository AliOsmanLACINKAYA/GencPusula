# GencPusula

Üniversite öğrencileri için burs, staj, yarışma ve etkinlik fırsatlarını tek merkezde
toplayan, dijital gazete formatında bir portal. Öğrenciler profillerine ve ilgi
alanlarına uygun fırsatları görüntüler, ilgilendiklerini kaydeder ve başvuru
durumlarını takip eder; yönetici rolündeki kullanıcılar içeriği panelden yönetir.

## Depo yapısı

```
GencPusula/
├── api/                      .NET çözümü: web arayüzü + Web API
│   ├── GencPusula.slnx       çözüm dosyası (XML biçimi)
│   ├── dotnet-tools.json     yerel araç manifestosu (dotnet-ef sürümünü sabitler)
│   └── GencPusula.Web/       ASP.NET Core projesi
├── mobile/genc_pusula/       Flutter uygulaması
├── docs/                     veritabanı şeması, uç nokta listesi, karar kayıtları
├── .gitignore
├── README.md
└── LICENSE
```

`api/` .NET dünyasını, `mobile/` Flutter dünyasını barındırır. İkisi ayrı klasörlerde
durur; mobil uygulama verilere yalnızca Web API üzerinden erişir, veritabanına
doğrudan bağlanmaz.

## Kullanılan teknolojiler

| Katman | Teknoloji |
| --- | --- |
| Backend | .NET 10 (LTS), ASP.NET Core, Razor Pages, Web API |
| Kimlik | ASP.NET Core Identity + JWT (mobil istemci için) |
| Veritabanı | PostgreSQL 16 veya üzeri |
| Veri erişimi | Entity Framework Core 10, Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3 |
| API dokümantasyonu | Swagger / OpenAPI (API uç noktaları yazıldığında eklenecek) |
| Mobil | Flutter (Dart) |

## Neyi nereye yazıyoruz

### `api/GencPusula.Web/`

| Yol | Buraya ne yazılır |
| --- | --- |
| `Pages/` | Web arayüzü sayfaları (Razor Pages). Sayfa başına bir `.cshtml` ve bir `.cshtml.cs`. Kullanıcı akışı burada; iş kuralı burada yazılmaz. |
| `Pages/Admin/` | Yalnızca yönetici rolüne açık panel: fırsat ekleme, düzenleme, silme, onaylama. |
| `Pages/Firsatlar/` | Fırsat listesi, detay, kategori ve arama/filtre görünümleri. |
| `Areas/Identity/` | Identity arayüzü varsayılan olarak NuGet paketinden gelir; bir giriş/kayıt sayfasını özelleştirmek istediğinde o sayfa buraya kopyalanır (scaffold). |
| `Controllers/Api/` | Web API denetleyicileri. Uç nokta sözleşmesi buradadır; iş mantığı `Services/` katmanında durur. |
| `Data/` | `ApplicationDbContext`, `Migrations/` (EF Core migration dosyaları) ve ileride `Seed/` (örnek veri). Migration dosyaları commit edilir. |
| `Entities/` | Veritabanı tablolarının sınıf karşılıkları. Yalnızca veri ve ilişki tanımı içerir; davranış eklenmez. |
| `Dtos/` | API'nin dışarıya verdiği ve dışarıdan aldığı veri modelleri. Entity'ler doğrudan API'ye dönülmez. |
| `Services/` | İş kuralları: uygunluk değerlendirmesi, güncellik etiketi, JWT üretimi, fırsat işlemleri. |
| `wwwroot/` | Statik dosyalar (css, js, görseller). `wwwroot/lib/` altındaki Bootstrap ve jQuery şablonla birlikte gelir. |
| `Program.cs` | Uygulamanın giriş noktası: servis kayıtları, kimlik doğrulama, ara katman sırası. |
| `appsettings.json` | Ortama bağlı olmayan ayarlar. Bağlantı dizesi bilinçli olarak boştur; gerçek değer gizli depodan gelir. |

### `mobile/genc_pusula/lib/`

| Yol | Buraya ne yazılır |
| --- | --- |
| `core/` | API adresi, sabitler, tema, ortak hata tipleri. |
| `data/models/` | API'den gelen JSON'un Dart karşılıkları. |
| `data/services/` | HTTP çağrıları (API istemcisi) ve token saklama. |
| `data/repositories/` | Veriyi ekranlara hazırlayan ara katman. |
| `features/<özellik>/` | Ekran ve o ekrana ait durum yönetimi. Özellikler: `auth`, `firsatlar`, `profil`, `kayitlilar`, `admin`. |
| `shared/widgets/` | Birden fazla ekranda kullanılan ortak arayüz parçaları. |

### `docs/`

`er-diagram.md` (veritabanı şeması ve ilişkiler), `api-endpoints.md` (uç nokta listesi
ve örnek istek/yanıt), `kararlar.md` (hangi teknik kararın neden alındığı). Klasör
şimdilik yalnızca `.gitkeep` içerir.

## Yeni geliştirici kurulumu (API)

**Ön koşullar:** .NET 10 SDK, PostgreSQL 16 veya üzeri, `openssl` (JWT anahtarı
üretmek için; yalnızca Linux/macOS'ta bulunur).

Kontrol:

```
dotnet --list-sdks      # 10.x görmelisin
pg_isready              # "accepting connections" demeli
```

### 1) Depoyu al ve aracı geri yükle

```
git clone https://github.com/AliOsmanLACINKAYA/GencPusula.git
cd GencPusula/api
dotnet tool restore
dotnet tool list        # dotnet-ef 10.0.13 görünmeli
```

EF Core komut satırı aracı depodaki `dotnet-tools.json` dosyasında tanımlıdır; bu
yüzden genel kurulum gerekmez, `dotnet tool restore` yeterlidir.

### 2) Veritabanını ve uygulama kullanıcısını oluştur

Kendi güçlü parolanı seç. Parola en az 16 karakter olsun ve içinde `;` ile `'`
karakterleri bulunmasın; bu iki karakter sırasıyla bağlantı dizesini ve SQL komutunu
bozar. Rastgele parola üretmek için: `openssl rand -hex 16`

```
sudo -u postgres psql -c "CREATE DATABASE gencpusula;"
sudo -u postgres psql -c "CREATE USER gencpusula_app WITH PASSWORD 'DB_PAROLASI';"
sudo -u postgres psql -c "GRANT ALL PRIVILEGES ON DATABASE gencpusula TO gencpusula_app;"
sudo -u postgres psql -d gencpusula -c "GRANT ALL ON SCHEMA public TO gencpusula_app;"
```

Son satır zorunludur: PostgreSQL 15 ve sonrasında `public` şemasında tablo oluşturma
yetkisi normal kullanıcılara varsayılan olarak verilmez. Bu satır atlanırsa
`dotnet ef database update` şu hatayı verir: `permission denied for schema public`.

Uygulama veritabanına `postgres` süper kullanıcısıyla bağlanmaz; yalnızca bu adımda
oluşturulan `gencpusula_app` kullanıcısı kullanılır.

### 3) Gizli ayarları tanımla

Bu değerler depoya girmez; her geliştiricinin makinesinde ayrı durur.

```
cd GencPusula.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=gencpusula;Username=gencpusula_app;Password=DB_PAROLASI"
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 48)"
dotnet user-secrets set "Jwt:Issuer" "GencPusula"
dotnet user-secrets set "Jwt:Audience" "GencPusulaMobile"
dotnet user-secrets list
```

`DB_PAROLASI` bu adımda ve 2. adımda birebir aynı olmalıdır. Farklı olursa bağlantı
`28P01 password authentication failed` hatası verir. PostgreSQL parolayı geri
okunamayacak biçimde sakladığı için doğrulamanın tek yolu bağlanmayı denemektir.

`dotnet user-secrets init` projede `UserSecretsId` zaten tanımlıysa hiçbir şey
değiştirmez; güvenle çalıştırılabilir.

### 4) Şemayı oluştur

```
dotnet ef database update
```

Migration dosyaları depoda olduğu için `dotnet ef migrations add` **çalıştırılmaz**;
şema hazır migration'lardan kurulur. Bu komut Identity tablolarını ve sonraki
adımlarda eklenecek uygulama tablolarını PostgreSQL'de oluşturur.

`dotnet ef` komutlarını `api/GencPusula.Web` klasöründen çalıştır. `api/` kökünden
çalıştırırsan araç hangi projeyi kullanacağını seçemez; o durumda projeyi açıkça
belirt: `dotnet ef database update --project GencPusula.Web`.

### 5) Çalıştır ve doğrula

```
dotnet run
```

Terminalde yazan yerel adresi tarayıcıda aç. Sağ üstte Register bağlantısını
görmelisin; bu, Identity'nin çalıştığını gösterir. Bir hesap açtıktan sonra kaydın
veritabanına düştüğünü doğrulayabilirsin:

```
sudo -u postgres psql -d gencpusula -c 'select "Email" from "AspNetUsers";'
```

### Sorun giderme

| Belirti | Sebep | Çözüm |
| --- | --- | --- |
| `28P01 password authentication failed` | 2. ve 3. adımdaki parolalar farklı | `sudo -u postgres psql -c "ALTER USER gencpusula_app WITH PASSWORD 'YENI_PAROLA';"` ve ardından user-secrets değerini aynı parolayla güncelle |
| `permission denied for schema public` | 2. adımın son satırı atlanmış | `sudo -u postgres psql -d gencpusula -c "GRANT ALL ON SCHEMA public TO gencpusula_app;"` |
| `dotnet-ef does not exist` hatası | araç geri yüklenmemiş | `api/` klasöründe `dotnet tool restore` |
| Bağlantı dizesi boş görünüyor | gizli ayar yazılmamış | 3. adımı tekrar çalıştır, `dotnet user-secrets list` ile doğrula |
| `database "gencpusula" does not exist` | veritabanı oluşturulmamış | 2. adımı tekrar çalıştır |

**Windows kullanıyorsan:** `sudo`, `$(...)` ve `openssl` komutları yoktur. PostgreSQL
komutlarını pgAdmin veya SQL Shell (psql) üzerinden çalıştır; JWT anahtarı için
rastgele 64 karakterlik bir metin üretip `dotnet user-secrets set "Jwt:Key" "..."`
komutuyla yaz.

## Çalıştırma

```
cd api/GencPusula.Web
dotnet run
```

Uygulama çalışırken API uç noktaları Swagger arayüzünden denenebilir. Swagger, ilk
API denetleyicileri yazıldığında eklenecek ve `/swagger` adresinde açılacaktır.

## Mobil uygulama (Flutter)

1. `cd mobile/genc_pusula`
2. `flutter pub get`
3. API adresini ayarla: Android emülatöründe `http://10.0.2.2:<port>`, gerçek cihazda
   bilgisayarın yerel IP adresi (`http://<ip>:<port>`). `localhost` mobil cihazda
   çalışmaz, çünkü cihazın kendisini işaret eder.
4. `flutter run`

Mobil uygulama, verilere erişmek için her istekte JWT token'ını
`Authorization: Bearer <token>` başlığıyla gönderir.

## API

- Tüm uç noktalar Swagger üzerinden test edilebilir.
- Kimlik doğrulama JWT iledir: giriş uç noktasından token alınır, sonraki isteklerde
  `Authorization: Bearer <token>` başlığı gönderilir.
- Roller: `Yonetici` ve `Kullanici`. Yönetici uç noktaları yalnızca `Yonetici` rolüne
  açıktır.
- En az bir varlık için tam CRUD (listeleme, ekleme, güncelleme, silme) uç noktası
  bulunur.

## Gizli bilgi politikası

Bağlantı dizesi, veritabanı parolası, JWT gizli anahtarı ve API anahtarları **depoya
konmaz**. Geliştirme ortamında `dotnet user-secrets`, üretim ortamında ortam
değişkenleri kullanılır. `appsettings.Development.json` ve `secrets.json` dosyaları
`.gitignore` içindedir. Depo herkese açık olduğu için yanlışlıkla gönderilen bir sır
kalıcı olarak açığa çıkmış sayılır; böyle bir durumda değer derhal değiştirilir.

## Roller

| Rol | Yetki |
| --- | --- |
| `Yonetici` | Fırsat ekleme, düzenleme, silme, onaylama; paneli kullanma |
| `Kullanici` | Fırsatları görüntüleme, profil oluşturma, fırsat kaydetme, başvuru durumu tutma |
