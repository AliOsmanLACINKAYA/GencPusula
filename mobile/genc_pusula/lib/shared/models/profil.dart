class Profil {
  const Profil({
    this.profilVar = false,
    this.adSoyad = '',
    this.bolum = '',
    this.sinif = '',
    this.ilgiAlanlari = '',
    this.tercihEdilenTurler = '',
    this.sehir = '',
    this.genelNotOrtalamasi,
    this.yabanciDilSeviyesi,
    this.guncellenmeTarihi,
    this.profilTamamlandi = false,
    this.eksikAlanlar = const [],
  });

  final bool profilVar;
  final String adSoyad;
  final String bolum;
  final String sinif;
  final String ilgiAlanlari;
  final String tercihEdilenTurler;
  final String sehir;
  final double? genelNotOrtalamasi;
  final String? yabanciDilSeviyesi;
  final DateTime? guncellenmeTarihi;
  final bool profilTamamlandi;
  final List<String> eksikAlanlar;

  factory Profil.fromJson(Map<String, dynamic> json) {
    return Profil(
      profilVar: json['profilVar'] == true,
      adSoyad: json['adSoyad']?.toString() ?? '',
      bolum: json['bolum']?.toString() ?? '',
      sinif: json['sinif']?.toString() ?? '',
      ilgiAlanlari: json['ilgiAlanlari']?.toString() ?? '',
      tercihEdilenTurler: json['tercihEdilenTurler']?.toString() ?? '',
      sehir: json['sehir']?.toString() ?? '',
      genelNotOrtalamasi: (json['genelNotOrtalamasi'] as num?)?.toDouble(),
      yabanciDilSeviyesi: _metinVeyaNull(
        json['yabanciDilSeviyesi']?.toString(),
      ),
      guncellenmeTarihi: DateTime.tryParse(
        json['guncellenmeTarihi']?.toString() ?? '',
      ),
      profilTamamlandi: json['profilTamamlandi'] == true,
      eksikAlanlar: (json['eksikAlanlar'] as List? ?? const [])
          .map((e) => e.toString())
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'adSoyad': adSoyad.trim(),
      'bolum': bolum.trim(),
      'sinif': sinif.trim(),
      'ilgiAlanlari': _metinVeyaNull(ilgiAlanlari),
      'tercihEdilenTurler': _metinVeyaNull(tercihEdilenTurler),
      'sehir': _metinVeyaNull(sehir),
      'genelNotOrtalamasi': genelNotOrtalamasi,
      'yabanciDilSeviyesi': _metinVeyaNull(yabanciDilSeviyesi),
    };
  }

  static String? _metinVeyaNull(String? deger) {
    final metin = deger?.trim() ?? '';
    return metin.isEmpty ? null : metin;
  }
}
