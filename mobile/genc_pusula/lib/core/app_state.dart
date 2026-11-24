import 'dart:async';
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../shared/models/firsat_ozeti.dart';
import '../shared/models/kayitli_firsat.dart';
import '../shared/models/profil.dart';
import 'config/config.dart';

/// Kullanıcıya gösterilebilecek sade API hatası.
class ApiHatasi implements Exception {
  const ApiHatasi(this.mesaj, {this.kod});

  final String mesaj;

  final String? kod;

  @override
  String toString() => mesaj;
}

/// Uygulamanın tek durum sınıfı: token, kullanıcı ve API istekleri.
class AppState extends ChangeNotifier {
  AppState({Dio? dio, FlutterSecureStorage? storage})
    : _storage = storage ?? const FlutterSecureStorage(),
      _dio =
          dio ??
          Dio(
            BaseOptions(
              baseUrl: AppConfig.apiBaseUrl,
              connectTimeout: const Duration(seconds: 10),
              receiveTimeout: const Duration(seconds: 10),
              contentType: Headers.jsonContentType,
            ),
          ) {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          if (_token != null) {
            options.headers['Authorization'] = 'Bearer $_token';
          }
          handler.next(options);
        },
        onError: (error, handler) {
          // Token varken 401 gelirse oturum bitmiştir.
          if (error.response?.statusCode == 401 && _token != null) {
            unawaited(cikisYap());
          }
          handler.next(error);
        },
      ),
    );
  }

  static const _tokenAnahtari = 'genc_pusula_jwt';

  final Dio _dio;
  final FlutterSecureStorage _storage;

  String? _token;
  String email = '';
  List<String> roller = const [];
  bool hazir = false;

  bool get girisYapildi => _token != null;
  bool get yoneticiMi => roller.contains('Yonetici');

  /// Saklanan tokenı okur. Splash bu bitince yönlendirilir.
  Future<void> baslat() async {
    try {
      final token = await _storage.read(key: _tokenAnahtari);
      if (token != null) {
        if (_suresiGecerli(token)) {
          _token = token;
          _kimligiCoz(token);
        } else {
          await _storage.delete(key: _tokenAnahtari);
        }
      }
    } catch (_) {
      _token = null;
    } finally {
      hazir = true;
      notifyListeners();
    }
  }

  Future<void> girisYap(String email, String parola) async {
    await _oturumAc(
      await _post('/api/auth/login', {'email': email, 'parola': parola}),
    );
  }

  Future<void> kayitOl(String email, String parola) async {
    await _oturumAc(
      await _post('/api/auth/register', {'email': email, 'parola': parola}),
    );
  }

  Future<void> cikisYap() async {
    _token = null;
    email = '';
    roller = const [];
    _kayitliFirsatKayitIdleri.clear();
    notifyListeners();

    try {
      await _storage.delete(key: _tokenAnahtari);
    } catch (_) {
      // Depo silinemese bile oturum kapandı.
    }
  }

  Future<FirsatSayfasi> firsatlariAra({
    String? kategori,
    String? sehir,
    String? arama,
    int sayfa = 1,
    int sayfaBoyutu = 10,
  }) async {
    final parametreler = <String, dynamic>{
      'sayfa': sayfa,
      'sayfaBoyutu': sayfaBoyutu,
    };

    if (kategori != null && kategori.isNotEmpty) {
      parametreler['kategori'] = kategori;
    }
    if (sehir != null && sehir.trim().isNotEmpty) {
      parametreler['sehir'] = sehir.trim();
    }
    if (arama != null && arama.trim().isNotEmpty) {
      parametreler['arama'] = arama.trim();
    }

    final veri = await _get(
      '/api/firsatlar/ara',
      queryParameters: parametreler,
    );
    return FirsatSayfasi.fromJson(veri as Map<String, dynamic>);
  }

  Future<FirsatOzeti> firsatDetayGetir(int id) async {
    final veri = await _get('/api/firsatlar/$id');
    return FirsatOzeti.fromJson(veri as Map<String, dynamic>);
  }

  Future<List<FirsatOzeti>> firsatlariGetir() async {
    final veri = await _get('/api/firsatlar');
    if (veri is List) {
      return veri
          .map((e) => FirsatOzeti.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    throw const ApiHatasi('Fırsatlar yüklenemedi.');
  }

  Future<List<String>> kategorileriGetir() async {
    final veri = await _get('/api/kategoriler');
    if (veri is List) return veri.map((e) => e.toString()).toList();
    throw const ApiHatasi('Kategoriler yüklenemedi.');
  }

  Future<FirsatOzeti> firsatEkle(Map<String, dynamic> govde) async {
    return FirsatOzeti.fromJson(await _post('/api/firsatlar', govde));
  }

  int _kayitliFirsatSurumu = 0;
  int get kayitliFirsatSurumu => _kayitliFirsatSurumu;

  final Map<int, int> _kayitliFirsatKayitIdleri = {};

  bool kayitliMi(int firsatId) =>
      _kayitliFirsatKayitIdleri.containsKey(firsatId);

  int? kayitliKayitId(int firsatId) => _kayitliFirsatKayitIdleri[firsatId];

  Future<Profil> profilGetir() async {
    final veri = await _get('/api/profil');
    if (veri is Map<String, dynamic>) return Profil.fromJson(veri);
    throw const ApiHatasi('Sunucudan beklenmeyen bir yanıt geldi.');
  }

  Future<Profil> profilKaydet(Profil profil) async {
    final veri = await _put('/api/profil', profil.toJson());
    return Profil.fromJson(veri);
  }

  Future<List<KayitliFirsat>> kayitliFirsatlariGetir() async {
    final veri = await _get('/api/kayitli-firsatlar');
    if (veri is List) {
      final kayitlar = veri
          .map((e) => KayitliFirsat.fromJson(e as Map<String, dynamic>))
          .toList();
      _kayitliFirsatKayitIdleri
        ..clear()
        ..addEntries(kayitlar.map((k) => MapEntry(k.firsatId, k.id)));
      notifyListeners();
      return kayitlar;
    }
    throw const ApiHatasi('Sunucudan beklenmeyen bir yanıt geldi.');
  }

  Future<void> kayitliFirsatlariTazele() async {
    try {
      await kayitliFirsatlariGetir();
    } catch (_) {
      return;
    }
  }

  Future<KayitliFirsat> firsatKaydet(int firsatId) async {
    final veri = await _post('/api/kayitli-firsatlar', {'firsatId': firsatId});
    final kayit = KayitliFirsat.fromJson(veri);
    _kayitliFirsatKayitIdleri[kayit.firsatId] = kayit.id;
    _kayitliFirsatlarDegisti();
    return kayit;
  }

  Future<void> firsatKaydiniKaldir(int kayitId) async {
    await _delete('/api/kayitli-firsatlar/$kayitId');
    _kayitliFirsatKayitIdleri.removeWhere((_, id) => id == kayitId);
    _kayitliFirsatlarDegisti();
  }

  void _kayitliFirsatlarDegisti() {
    _kayitliFirsatSurumu++;
    notifyListeners();
  }

  Future<void> _oturumAc(Map<String, dynamic> cevap) async {
    final token = cevap['token']?.toString() ?? '';
    if (token.isEmpty) {
      throw const ApiHatasi('Giriş yanıtında token bulunamadı.');
    }

    await _storage.write(key: _tokenAnahtari, value: token);

    _token = token;
    email = cevap['email']?.toString() ?? '';
    roller = (cevap['roller'] as List? ?? const [])
        .map((e) => e.toString())
        .toList();
    notifyListeners();
  }

  Future<Map<String, dynamic>> _post(
    String yol,
    Map<String, dynamic> govde,
  ) async {
    final veri = await _istek(() => _dio.post(yol, data: govde));
    if (veri is Map<String, dynamic>) return veri;
    throw const ApiHatasi('Sunucudan beklenmeyen bir yanıt geldi.');
  }

  Future<Map<String, dynamic>> _put(
    String yol,
    Map<String, dynamic> govde,
  ) async {
    final veri = await _istek(() => _dio.put(yol, data: govde));
    if (veri is Map<String, dynamic>) return veri;
    throw const ApiHatasi('Sunucudan beklenmeyen bir yanıt geldi.');
  }

  Future<void> _delete(String yol) async {
    await _istek(() => _dio.delete(yol));
  }

  Future<dynamic> _get(String yol, {Map<String, dynamic>? queryParameters}) {
    return _istek(() => _dio.get(yol, queryParameters: queryParameters));
  }

  Future<dynamic> _istek(Future<dynamic> Function() istek) async {
    try {
      final cevap = await istek();
      return cevap.data;
    } on DioException catch (e) {
      throw ApiHatasi(_mesaj(e), kod: _kod(e));
    } catch (_) {
      throw const ApiHatasi('Sunucudan beklenmeyen bir yanıt geldi.');
    }
  }

  String _mesaj(DioException e) {
    final veri = e.response?.data;

    if (veri is Map) {
      final mesaj = veri['mesaj'];
      if (mesaj is String && mesaj.isNotEmpty) return mesaj;

      final hatalar = veri['errors'];
      if (hatalar is Map && hatalar.isNotEmpty) {
        final ilk = hatalar.values.first;
        if (ilk is List && ilk.isNotEmpty) return ilk.first.toString();
      }
    }

    if (e.type == DioExceptionType.connectionError) {
      return 'Sunucuya bağlanılamadı. API açık mı?';
    }
    if (e.type == DioExceptionType.connectionTimeout ||
        e.type == DioExceptionType.receiveTimeout) {
      return 'Sunucu yanıt vermedi. Tekrar dene.';
    }
    return 'Bir hata oluştu. Tekrar dene.';
  }

  String? _kod(DioException e) {
    final veri = e.response?.data;
    if (veri is Map) {
      final kod = veri['kod'];
      if (kod is String && kod.isNotEmpty) return kod;
    }
    return null;
  }

  Map<String, dynamic>? _payload(String token) {
    try {
      final parcalar = token.split('.');
      if (parcalar.length != 3) return null;
      return jsonDecode(
        utf8.decode(base64Url.decode(base64Url.normalize(parcalar[1]))),
      ) as Map<String, dynamic>;
    } catch (_) {
      return null;
    }
  }

  bool _suresiGecerli(String token) {
    final exp = _payload(token)?['exp'];
    return exp is num &&
        DateTime.now().millisecondsSinceEpoch < exp.toInt() * 1000;
  }

  void _kimligiCoz(String token) {
    final veri = _payload(token);
    if (veri == null) return;

    email = veri['email']?.toString() ?? '';
    final rol = veri['role'];
    roller = rol is List
        ? rol.map((e) => e.toString()).toList()
        : rol is String
        ? [rol]
        : const [];
  }
}

/// Uygulama genelinde tek örnek.
final appState = AppState();
