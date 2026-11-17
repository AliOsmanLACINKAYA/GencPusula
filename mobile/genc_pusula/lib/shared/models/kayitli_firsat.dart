class KayitliFirsat {
  const KayitliFirsat({
    required this.id,
    required this.firsatId,
    required this.baslik,
    required this.firsatTuru,
    required this.sehir,
    required this.sonBasvuruTarihi,
    required this.guncellikEtiketi,
    required this.basvuruDurumu,
    this.kisiselNotlar,
    required this.kayitTarihi,
  });

  final int id;
  final int firsatId;
  final String baslik;
  final String firsatTuru;
  final String sehir;
  final DateTime sonBasvuruTarihi;
  final String guncellikEtiketi;
  final String basvuruDurumu;
  final String? kisiselNotlar;
  final DateTime kayitTarihi;

  factory KayitliFirsat.fromJson(Map<String, dynamic> json) {
    return KayitliFirsat(
      id: (json['id'] as num?)?.toInt() ?? 0,
      firsatId: (json['firsatId'] as num?)?.toInt() ?? 0,
      baslik: json['baslik']?.toString() ?? '',
      firsatTuru: json['firsatTuru']?.toString() ?? '',
      sehir: json['sehir']?.toString() ?? '',
      sonBasvuruTarihi:
          DateTime.tryParse(json['sonBasvuruTarihi']?.toString() ?? '') ??
          DateTime.now(),
      guncellikEtiketi: json['guncellikEtiketi']?.toString() ?? '',
      basvuruDurumu: json['basvuruDurumu']?.toString() ?? '',
      kisiselNotlar: json['kisiselNotlar']?.toString(),
      kayitTarihi:
          DateTime.tryParse(json['kayitTarihi']?.toString() ?? '') ??
          DateTime.now(),
    );
  }

  String get sonTarih => _tarih(sonBasvuruTarihi);

  static String _tarih(DateTime tarih) {
    final yerel = tarih.toLocal();
    final gun = yerel.day.toString().padLeft(2, '0');
    final ay = yerel.month.toString().padLeft(2, '0');
    return '$gun.$ay.${yerel.year}';
  }
}
