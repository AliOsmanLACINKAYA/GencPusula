import 'dart:async';
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../shared/models/firsat_ozeti.dart';
import 'config/config.dart';

/// Kullanıcıya gösterilebilecek sade API hatası.
class ApiHatasi implements Exception {
  const ApiHatasi(this.mesaj);

  final String mesaj;

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

  Future<dynamic> _get(String yol, {Map<String, dynamic>? queryParameters}) {
    return _istek(() => _dio.get(yol, queryParameters: queryParameters));
  }

  Future<dynamic> _istek(Future<dynamic> Function() istek) async {
    try {
      final cevap = await istek();
      return cevap.data;
    } on DioException catch (e) {
      throw ApiHatasi(_mesaj(e));
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
