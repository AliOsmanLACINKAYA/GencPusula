/// GET /api/firsatlar yanıtındaki tek bir fırsat.
class FirsatOzeti {
  const FirsatOzeti({
    required this.id,
    required this.baslik,
    required this.aciklama,
    required this.firsatTuru,
    required this.sehir,
    required this.sonBasvuruTarihi,
    required this.sonKontrolTarihi,
    required this.guncellikEtiketi,
    required this.kaynakBaglantisi,
  });

  final int id;
  final String baslik;
  final String aciklama;
  final String firsatTuru;
  final String sehir;
  final DateTime sonBasvuruTarihi;
  final DateTime? sonKontrolTarihi;
  final String guncellikEtiketi;
  final String kaynakBaglantisi;

  factory FirsatOzeti.fromJson(Map<String, dynamic> json) {
    return FirsatOzeti(
      id: (json['id'] as num?)?.toInt() ?? 0,
      baslik: json['baslik']?.toString() ?? '',
      aciklama: json['aciklama']?.toString() ?? '',
      firsatTuru: json['firsatTuru']?.toString() ?? '',
      sehir: json['sehir']?.toString() ?? '',
      sonBasvuruTarihi:
          DateTime.tryParse(json['sonBasvuruTarihi']?.toString() ?? '') ??
          DateTime.now(),
      sonKontrolTarihi: DateTime.tryParse(
        json['sonKontrolTarihi']?.toString() ?? '',
      ),
      guncellikEtiketi: json['guncellikEtiketi']?.toString() ?? '',
      kaynakBaglantisi: json['kaynakBaglantisi']?.toString() ?? '',
    );
  }

  String get sonTarih => _tarih(sonBasvuruTarihi);

  String get sonKontrol =>
      sonKontrolTarihi == null ? 'Belirtilmedi' : _tarih(sonKontrolTarihi!);

  static String _tarih(DateTime tarih) {
    final yerel = tarih.toLocal();
    final gun = yerel.day.toString().padLeft(2, '0');
    final ay = yerel.month.toString().padLeft(2, '0');
    return '$gun.$ay.${yerel.year}';
  }
}

/// GET /api/firsatlar/ara yanıtı.
class FirsatSayfasi {
  const FirsatSayfasi({
    required this.toplamKayit,
    required this.sayfa,
    required this.toplamSayfa,
    required this.kayitlar,
  });

  final int toplamKayit;
  final int sayfa;
  final int toplamSayfa;
  final List<FirsatOzeti> kayitlar;

  bool get devamVarMi => sayfa < toplamSayfa;

  factory FirsatSayfasi.fromJson(Map<String, dynamic> json) {
    final kayitlar = (json['kayitlar'] as List? ?? const [])
        .map((e) => FirsatOzeti.fromJson(e as Map<String, dynamic>))
        .toList();

    return FirsatSayfasi(
      toplamKayit: (json['toplamKayit'] as num?)?.toInt() ?? 0,
      sayfa: (json['sayfa'] as num?)?.toInt() ?? 1,
      toplamSayfa: (json['toplamSayfa'] as num?)?.toInt() ?? 1,
      kayitlar: kayitlar,
    );
  }
}
