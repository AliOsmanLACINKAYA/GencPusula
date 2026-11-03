/// GET /api/firsatlar yanıtındaki tek bir fırsat.
class FirsatOzeti {
  const FirsatOzeti({
    required this.id,
    required this.baslik,
    required this.firsatTuru,
    required this.sehir,
    required this.sonBasvuruTarihi,
    required this.kaynakBaglantisi,
  });

  final int id;
  final String baslik;
  final String firsatTuru;
  final String sehir;
  final DateTime sonBasvuruTarihi;
  final String kaynakBaglantisi;

  factory FirsatOzeti.fromJson(Map<String, dynamic> json) {
    return FirsatOzeti(
      id: (json['id'] as num?)?.toInt() ?? 0,
      baslik: json['baslik']?.toString() ?? '',
      firsatTuru: json['firsatTuru']?.toString() ?? '',
      sehir: json['sehir']?.toString() ?? '',
      sonBasvuruTarihi:
          DateTime.tryParse(json['sonBasvuruTarihi']?.toString() ?? '') ??
          DateTime.now(),
      kaynakBaglantisi: json['kaynakBaglantisi']?.toString() ?? '',
    );
  }

  String get sonTarih {
    final tarih = sonBasvuruTarihi.toLocal();
    final gun = tarih.day.toString().padLeft(2, '0');
    final ay = tarih.month.toString().padLeft(2, '0');
    return '$gun.$ay.${tarih.year}';
  }
}
