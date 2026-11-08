import 'package:flutter/material.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';
import '../../core/widgets/button.dart';
import '../../core/widgets/empty_state.dart';
import '../../core/widgets/opportunity.dart';
import '../../shared/models/firsat_ozeti.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _arama = TextEditingController();

  List<FirsatOzeti> _firsatlar = [];
  _AramaTuru _aramaTuru = _AramaTuru.arama;
  int _sayfa = 1;
  bool _devamVarMi = false;
  int _toplamKayit = 0;
  bool _yukleniyor = false;
  String? _hata;

  @override
  void initState() {
    super.initState();
    _sayfaYukle(yenile: true);
  }

  @override
  void dispose() {
    _arama.dispose();
    super.dispose();
  }

  Future<void> _sayfaYukle({required bool yenile}) async {
    if (_yukleniyor) return;

    setState(() {
      _yukleniyor = true;
      if (yenile) {
        _hata = null;
        _sayfa = 1;
        _firsatlar = [];
      }
    });

    final metin = _arama.text.trim();

    try {
      final cevap = await appState.firsatlariAra(
        kategori: _aramaTuru == _AramaTuru.kategori ? metin : null,
        sehir: _aramaTuru == _AramaTuru.sehir ? metin : null,
        arama: _aramaTuru == _AramaTuru.arama ? metin : null,
        sayfa: yenile ? 1 : _sayfa + 1,
        sayfaBoyutu: 10,
      );

      if (!mounted) return;
      setState(() {
        _firsatlar = yenile
            ? cevap.kayitlar
            : [..._firsatlar, ...cevap.kayitlar];
        _sayfa = cevap.sayfa;
        _devamVarMi = cevap.devamVarMi;
        _toplamKayit = cevap.toplamKayit;
        _hata = null;
      });
    } catch (e) {
      if (!mounted) return;
      final mesaj = e is ApiHatasi
          ? e.mesaj
          : yenile
          ? 'Fırsatlar yüklenemedi. Tekrar dene.'
          : 'Daha fazla fırsat yüklenemedi.';
      if (yenile) {
        setState(() => _hata = mesaj);
      } else {
        ScaffoldMessenger.of(context)
            .showSnackBar(SnackBar(content: Text(mesaj)));
      }
    } finally {
      if (mounted) setState(() => _yukleniyor = false);
    }
  }

  void _filtrele() {
    FocusScope.of(context).unfocus();
    _sayfaYukle(yenile: true);
  }

  void _temizle() {
    FocusScope.of(context).unfocus();
    _arama.clear();
    _sayfaYukle(yenile: true);
  }

  String get _ipucu {
    return switch (_aramaTuru) {
      _AramaTuru.kategori => 'Kategori (örn. Burs)',
      _AramaTuru.sehir => 'Şehir (örn. Ankara)',
      _AramaTuru.arama => 'Başlık, açıklama veya şehir ara...',
    };
  }

  IconData get _ikon {
    return switch (_aramaTuru) {
      _AramaTuru.kategori => Icons.category_outlined,
      _AramaTuru.sehir => Icons.place_outlined,
      _AramaTuru.arama => Icons.search,
    };
  }

  @override
  Widget build(BuildContext context) {
    if (_yukleniyor && _firsatlar.isEmpty) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    if (_hata != null && _firsatlar.isEmpty) {
      return Scaffold(
        body: SafeArea(
          child: EmptyState(
            icon: Icons.cloud_off_rounded,
            title: 'Fırsatlar yüklenemedi',
            message: _hata!,
            actionLabel: 'Tekrar dene',
            onAction: () => _sayfaYukle(yenile: true),
          ),
        ),
      );
    }

    return Scaffold(
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: () => _sayfaYukle(yenile: true),
          child: _icerik(),
        ),
      ),
    );
  }

  Widget _icerik() {
    final textTheme = Theme.of(context).textTheme;
    final ad = appState.email.split('@').first;

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                ad.isEmpty ? 'Merhaba 👋' : 'Merhaba, $ad 👋',
                style: textTheme.headlineSmall,
              ),
            ),
            IconButton(
              onPressed: () => _sayfaYukle(yenile: true),
              tooltip: 'Yenile',
              icon: const Icon(Icons.refresh),
            ),
            CircleAvatar(
              radius: 20,
              backgroundColor: AppColors.primary,
              child: Text(
                ad.isEmpty ? 'GP' : ad[0].toUpperCase(),
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Text(
          'Bugün hangi fırsatı yakalıyorsun?',
          style: textTheme.bodyMedium?.copyWith(color: AppColors.muted),
        ),
        const SizedBox(height: 16),
        TextField(
          controller: _arama,
          textInputAction: TextInputAction.search,
          onSubmitted: (_) => _filtrele(),
          decoration: InputDecoration(
            hintText: _ipucu,
            prefixIcon: Icon(_ikon),
            isDense: true,
            suffixIcon: PopupMenuButton<_AramaTuru>(
              icon: const Icon(Icons.tune),
              tooltip: 'Arama türü',
              initialValue: _aramaTuru,
              onSelected: (tur) {
                setState(() {
                  _aramaTuru = tur;
                  _arama.clear();
                });
                _filtrele();
              },
              itemBuilder: (context) => const [
                PopupMenuItem(
                  value: _AramaTuru.kategori,
                  child: Text('Kategori'),
                ),
                PopupMenuItem(value: _AramaTuru.sehir, child: Text('Şehir')),
                PopupMenuItem(value: _AramaTuru.arama, child: Text('Arama')),
              ],
            ),
          ),
        ),
        const SizedBox(height: 20),
        Row(
          children: [
            Expanded(child: Text('Fırsatlar', style: textTheme.titleLarge)),
            Text(
              '$_toplamKayit fırsat',
              style: textTheme.bodySmall?.copyWith(color: AppColors.muted),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (_firsatlar.isEmpty)
          SizedBox(
            height: 320,
            child: EmptyState(
              icon: Icons.search_off_rounded,
              title: 'Sonuç bulunamadı',
              message: 'Arama türünü veya metni değiştirip tekrar dene.',
              actionLabel: 'Temizle',
              onAction: _temizle,
            ),
          )
        else ...[
          for (final firsat in _firsatlar) ...[
            OpportunityCard(firsat: firsat),
            const SizedBox(height: 12),
          ],
          if (_devamVarMi)
            AppButton(
              label: 'Daha fazla yükle',
              isLoading: _yukleniyor,
              onPressed: () => _sayfaYukle(yenile: false),
            ),
        ],
      ],
    );
  }
}

enum _AramaTuru { kategori, sehir, arama }
