# Genç Pusula

**BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları — 2026–2027 Güz Yarıyılı Dönem Projesi**

## 1. Projenin Amacı ve Kapsamı
Genç Pusula; Türkiye genelindeki ön lisans, lisans, lisansüstü ve doktora öğrencilerinin burs, staj ilanları, yarışma ve etkinlik duyurularına tek bir merkezden, dijital gazete formatında ulaşmasını sağlayan web tabanlı bir portaldır. Öğrenciler kendi bölüm, sınıf ve not ortalaması bilgilerine göre fırsatlara uygunluk durumlarını görebilir, başvurularını ve kontrol listelerini takip edebilirler.

### İlk Sürüme Alınan Özellikler (Kapsam İçi)
* Fırsat (burs, staj, yarışma, etkinlik) listeleme, detay görüntüleme ve kategori/şehir filtreleme.
* Yönetici paneli üzerinden fırsat ekleme, düzenleme, silme, onaylama ve güncellik etiketi yönetimi.
* Öğrenci profili oluşturma (bölüm, sınıf, not ortalaması, ilgi alanları) ve ilan şartlarıyla otomatik uygunluk karşılaştırması.
* Fırsatları kaydetme, başvuru durumu (*İlgileniyorum*, *Hazırlanıyorum*, *Başvurdum*) ve kişisel not takibi.
* İlan gereksinimlerine göre kontrol listesi (checklist) takibi ve Swagger destekli Web API CRUD uç noktaları.

### Kapsam Dışı Bırakılan İşler
* Kurum web sitelerinden otomatik veri kazıma (web scraping) botları.
* Uygulama içi anlık mesajlaşma ve ödeme/bağış altyapısı.

## 2. Geliştirme Ekibi
* **Ali Osman LAÇİNKAYA** (`@AliOsmanLACINKAYA`)
* **Zeynep Lâl KAŞIKCI**
* **Kadirhan Topal**

## 3. Kullanılan Teknolojiler
* **Proje Türü:** ASP.NET Core (Razor Pages) & Web API
* **Veritabanı:** PostgreSQL
* **ORM:** Entity Framework Core (Code-First & Migrations)
* **Kimlik Doğrulama:** ASP.NET Core Identity (Yönetici ve Kullanıcı Rolleri)

## 4. Kurulum ve Çalıştırma Adımları

### Gereksinimler
* .NET SDK
* PostgreSQL

### 1. Depoyu Klonlayın
```bash
git clone [https://github.com/AliOsmanLACINKAYA/gencpusula.git](https://github.com/AliOsmanLACINKAYA/gencpusula.git)
cd gencpusula
```

### 2. Veritabanı Kurulumu
`scripts/` klasörü altındaki betikleri kullanarak yerel PostgreSQL veritabanı başlangıç yapılandırmasını çalıştırabilir veya `User Secrets` ile kendi bağlantı dizenizi tanımlayabilirsiniz:
```bash
cd api/GencPusula.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=GencPusulaDb;Username=postgres;Password=KENDI_SIFRENIZ"
```

### 3. Migration Adımlarını Uygulayın
Tabloları (`Firsatlar`, `OgrenciProfilleri`, `BasvuruKayitlari` ve Identity tabloları) oluşturmak için aşağıdaki komutu çalıştırın:
```bash
dotnet ef database update
```

### 4. Uygulamayı Çalıştırın
```bash
dotnet run
```
