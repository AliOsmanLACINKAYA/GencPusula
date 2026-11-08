import 'package:flutter_test/flutter_test.dart';
import 'package:genc_pusula/shared/models/firsat_ozeti.dart';

void main() {
  test('sayfalama JSON alanları doğru okunur', () {
    final sayfa = FirsatSayfasi.fromJson({
      'toplamKayit': 16,
      'sayfa': 1,
      'toplamSayfa': 2,
      'kayitlar': [
        {'id': 2, 'baslik': 'Staj', 'sonBasvuruTarihi': '2026-12-15T00:00:00'},
      ],
    });

    expect(sayfa.toplamKayit, 16);
    expect(sayfa.sayfa, 1);
    expect(sayfa.toplamSayfa, 2);
    expect(sayfa.devamVarMi, isTrue);
    expect(sayfa.kayitlar.single.baslik, 'Staj');
  });
}
