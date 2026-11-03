import 'package:flutter_test/flutter_test.dart';
import 'package:genc_pusula/shared/models/firsat_ozeti.dart';

void main() {
  test('fırsat JSON alanları doğru okunur', () {
    final firsat = FirsatOzeti.fromJson({
      'id': 2,
      'baslik': 'Staj',
      'firsatTuru': 'Staj',
      'sehir': 'İstanbul',
      'sonBasvuruTarihi': '2026-12-15T00:00:00',
      'kaynakBaglantisi': 'https://ornek.com',
    });

    expect(firsat.id, 2);
    expect(firsat.baslik, 'Staj');
    expect(firsat.sonTarih, '15.12.2026');
  });
}
