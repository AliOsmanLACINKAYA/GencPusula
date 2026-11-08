import 'package:flutter_test/flutter_test.dart';
import 'package:genc_pusula/shared/models/firsat_ozeti.dart';

void main() {
  test('fırsat JSON alanları doğru okunur', () {
    final firsat = FirsatOzeti.fromJson({
      'id': 2,
      'baslik': 'Staj',
      'aciklama': 'Örnek staj açıklaması',
      'firsatTuru': 'Staj',
      'sehir': 'İstanbul',
      'sonBasvuruTarihi': '2026-12-15T00:00:00',
      'sonKontrolTarihi': '2026-11-05T00:00:00',
      'guncellikEtiketi': 'Güncel',
      'kaynakBaglantisi': 'https://ornek.com',
    });

    expect(firsat.id, 2);
    expect(firsat.baslik, 'Staj');
    expect(firsat.aciklama, 'Örnek staj açıklaması');
    expect(firsat.guncellikEtiketi, 'Güncel');
    expect(firsat.sonTarih, '15.12.2026');
    expect(firsat.sonKontrol, '05.11.2026');
  });

  test('son kontrol tarihi yoksa Belirtilmedi döner', () {
    final firsat = FirsatOzeti.fromJson({
      'id': 3,
      'baslik': 'Burs',
      'sonBasvuruTarihi': '2026-12-15T00:00:00',
    });

    expect(firsat.sonKontrol, 'Belirtilmedi');
  });
}
