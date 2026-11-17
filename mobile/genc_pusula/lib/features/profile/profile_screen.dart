import 'package:flutter/material.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';
import '../../core/widgets/empty_state.dart';
import '../../core/widgets/kategori_avatar.dart';
import '../../core/widgets/text_field.dart';
import '../../shared/models/profil.dart';

const _siniflar = [
  'Hazırlık',
  '1. Sınıf',
  '2. Sınıf',
  '3. Sınıf',
  '4. Sınıf',
  '5. Sınıf',
  '6. Sınıf',
  'Yüksek Lisans',
  'Doktora',
  'Mezun',
];

const _tercihEdilenTurSecenekleri = [
  ('Burs', 'Burs'),
  ('Staj', 'Staj'),
  ('Etkinlik', 'Etkinlik'),
  ('Yarisma', 'Yarışma'),
];

const _dilSeviyeleri = [
  'Belirtilmedi',
  'A1 - Başlangıç',
  'A2 - Temel',
  'B1 - Orta Altı',
  'B2 - Orta Üstü',
  'C1 - İleri',
  'C2 - Yetkin',
];

String _kisaSinif(String sinif) {
  final rakam = int.tryParse(sinif.split('.').first);
  return rakam?.toString() ?? sinif;
}

String _kisaDil(String seviye) {
  if (seviye == 'Belirtilmedi') return seviye;
  return seviye.split(' ').first;
}

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  final _adSoyad = TextEditingController();
  final _bolum = TextEditingController();
  final _ilgiAlanlari = TextEditingController();
  final _sehir = TextEditingController();
  final _notOrtalamasi = TextEditingController();

  Profil? _profil;
  bool _yukleniyor = true;
  bool _kaydediliyor = false;
  String? _hata;
  String? _sinif;
  String? _dil;
  Set<String> _tercihEdilenTurler = {};

  @override
  void initState() {
    super.initState();
    _yukle(baslangic: true);
  }

  @override
  void dispose() {
    for (final controller in [
      _adSoyad,
      _bolum,
      _ilgiAlanlari,
      _sehir,
      _notOrtalamasi,
    ]) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<void> _yukle({bool baslangic = false}) async {
    if (!baslangic) {
      setState(() {
        _yukleniyor = true;
        _hata = null;
      });
    }

    try {
      final profil = await appState.profilGetir();
      if (!mounted) return;
      setState(() {
        _profil = profil;
        _formuDoldur(profil);
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _hata = e is ApiHatasi ? e.mesaj : 'Profil yüklenemedi. Tekrar dene.';
      });
    } finally {
      if (mounted) setState(() => _yukleniyor = false);
    }
  }

  void _formuDoldur(Profil profil) {
    _adSoyad.text = profil.adSoyad;
    _bolum.text = profil.bolum;
    _ilgiAlanlari.text = profil.ilgiAlanlari;
    _tercihEdilenTurler = profil.tercihEdilenTurler
        .split(',')
        .map(_turApiDegeri)
        .whereType<String>()
        .toSet();
    _sehir.text = profil.sehir;
    _notOrtalamasi.text = profil.genelNotOrtalamasi == null
        ? ''
        : profil.genelNotOrtalamasi!.toStringAsFixed(2);
    _sinif = _siniflar.contains(profil.sinif) ? profil.sinif : null;
    _dil = _dilSeviyeleri.contains(profil.yabanciDilSeviyesi)
        ? profil.yabanciDilSeviyesi
        : null;
  }

  String? _turApiDegeri(String deger) {
    final temiz = deger.trim();
    for (final (apiDegeri, gorunenAd) in _tercihEdilenTurSecenekleri) {
      if (temiz == apiDegeri || temiz == gorunenAd) return apiDegeri;
    }
    return null;
  }

  Future<void> _turSeciminiAc() async {
    final secim = {..._tercihEdilenTurler};
    final sonuc = await showModalBottomSheet<Set<String>>(
      context: context,
      isScrollControlled: true,
      builder: (context) {
        return StatefulBuilder(
          builder: (context, setSheetState) {
            return SafeArea(
              child: SingleChildScrollView(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Padding(
                      padding: const EdgeInsets.fromLTRB(20, 20, 20, 8),
                      child: Text(
                        'Tercih Edilen Türler',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                    ),
                    for (final (apiDegeri, gorunenAd)
                        in _tercihEdilenTurSecenekleri)
                      CheckboxListTile(
                        value: secim.contains(apiDegeri),
                        secondary: KategoriAvatar(kategori: apiDegeri),
                        title: Text(gorunenAd),
                        onChanged: (secili) {
                          setSheetState(() {
                            if (secili == true) {
                              secim.add(apiDegeri);
                            } else {
                              secim.remove(apiDegeri);
                            }
                          });
                        },
                      ),
                    Padding(
                      padding: const EdgeInsets.all(20),
                      child: SizedBox(
                        width: double.infinity,
                        child: FilledButton(
                          onPressed: () => Navigator.of(context).pop(secim),
                          child: const Text('Tamam'),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );

    if (sonuc != null && mounted) {
      setState(() => _tercihEdilenTurler = sonuc);
    }
  }

  Future<void> _kaydet() async {
    final notMetni = _notOrtalamasi.text.trim();
    double? notDegeri;
    if (notMetni.isNotEmpty) {
      notDegeri = double.tryParse(notMetni.replaceAll(',', '.'));
      if (notDegeri == null || !notDegeri.isFinite) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Genel not ortalaması geçerli bir sayı olmalı.'),
          ),
        );
        return;
      }
    }

    setState(() => _kaydediliyor = true);
    try {
      final profil = Profil(
        adSoyad: _adSoyad.text.trim(),
        bolum: _bolum.text.trim(),
        sinif: _sinif ?? '',
        ilgiAlanlari: _ilgiAlanlari.text.trim(),
        tercihEdilenTurler: [
          for (final (apiDegeri, _) in _tercihEdilenTurSecenekleri)
            if (_tercihEdilenTurler.contains(apiDegeri)) apiDegeri,
        ].join(', '),
        sehir: _sehir.text.trim(),
        genelNotOrtalamasi: notDegeri,
        yabanciDilSeviyesi: _dil,
      );

      final kaydedilen = await appState.profilKaydet(profil);
      if (!mounted) return;
      setState(() {
        _profil = kaydedilen;
        _formuDoldur(kaydedilen);
      });
      ScaffoldMessenger.of(context)
          .showSnackBar(const SnackBar(content: Text('Profil kaydedildi.')));
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            e is ApiHatasi ? e.mesaj : 'Profil kaydedilemedi. Tekrar dene.',
          ),
        ),
      );
    } finally {
      if (mounted) setState(() => _kaydediliyor = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Profil'),
        actions: [
          if (_kaydediliyor)
            const Padding(
              padding: EdgeInsets.symmetric(horizontal: 16),
              child: Center(
                child: SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                ),
              ),
            )
          else
            IconButton(
              icon: const Icon(Icons.save_outlined),
              tooltip: 'Kaydet',
              onPressed: _yukleniyor || _hata != null ? null : _kaydet,
            ),
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Çıkış yap',
            onPressed: () async {
              await appState.cikisYap();
            },
          ),
        ],
      ),
      body: _govde(),
    );
  }

  Widget _govde() {
    if (_yukleniyor) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_hata != null) {
      return EmptyState(
        icon: Icons.cloud_off_rounded,
        title: 'Profil yüklenemedi',
        message: _hata!,
        actionLabel: 'Tekrar dene',
        onAction: _yukle,
      );
    }

    final profil = _profil;

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        _baslik(),
        const SizedBox(height: 24),
        if (profil != null && !profil.profilVar) ...[
          _uyariKarti(
            icon: Icons.person_add_alt_1_outlined,
            title: 'Profilini tamamla',
            message:
                'Sana uygun fırsatları değerlendirebilmemiz için aşağıdaki '
                'formu doldur.',
          ),
          const SizedBox(height: 16),
        ],
        if (profil != null &&
            profil.profilVar &&
            profil.eksikAlanlar.isNotEmpty) ...[
          _uyariKarti(
            icon: Icons.warning_amber_rounded,
            title: 'Profilinde eksik bilgiler var',
            message: 'Aşağıdaki zorunlu alanları tamamlaman gerekiyor:',
            color: AppColors.accent,
            maddeler: profil.eksikAlanlar,
          ),
          const SizedBox(height: 16),
        ],
        _form(),
      ],
    );
  }

  Widget _baslik() {
    final textTheme = Theme.of(context).textTheme;
    final rol = appState.yoneticiMi ? 'Yönetici' : 'Kullanıcı';

    return Row(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        const CircleAvatar(
          radius: 44,
          backgroundColor: AppColors.primary,
          child: Icon(Icons.person, color: Colors.white, size: 44),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                appState.email,
                style: textTheme.titleMedium,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 4),
              Text(
                rol,
                style: textTheme.bodyMedium?.copyWith(color: AppColors.muted),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _uyariKarti({
    required IconData icon,
    required String title,
    required String message,
    Color color = AppColors.primary,
    List<String>? maddeler,
  }) {
    final textTheme = Theme.of(context).textTheme;

    return Card(
      color: color.withValues(alpha: 0.06),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: color),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    message,
                    style: textTheme.bodySmall?.copyWith(
                      color: AppColors.muted,
                    ),
                  ),
                  if (maddeler != null) ...[
                    const SizedBox(height: 6),
                    for (final madde in maddeler)
                      Text(
                        '• $madde',
                        style: textTheme.bodySmall?.copyWith(
                          color: AppColors.muted,
                        ),
                      ),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _form() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        AppTextField(
          controller: _adSoyad,
          label: 'Ad Soyad *',
          prefixIcon: Icons.person_outline,
          textInputAction: TextInputAction.next,
        ),
        const SizedBox(height: 14),
        AppTextField(
          controller: _bolum,
          label: 'Bölüm *',
          prefixIcon: Icons.school_outlined,
          textInputAction: TextInputAction.next,
        ),
        const SizedBox(height: 14),
        DropdownButtonFormField<String>(
          initialValue: _sinif,
          isExpanded: true,
          isDense: true,
          decoration: const InputDecoration(labelText: 'Sınıf'),
          hint: const Text('Seç'),
          items: [
            for (final sinif in _siniflar)
              DropdownMenuItem(
                value: sinif,
                child: Text(
                  _kisaSinif(sinif),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
          ],
          onChanged: (deger) => setState(() => _sinif = deger),
        ),
        const SizedBox(height: 14),
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: AppTextField(
                controller: _notOrtalamasi,
                label: 'Not Ortalaması',
                hint: '0 - 4',
                prefixIcon: Icons.percent_outlined,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                ),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: DropdownButtonFormField<String>(
                initialValue: _dil,
                isExpanded: true,
                decoration: const InputDecoration(
                  labelText: 'Dil Seviyesi',
                  prefixIcon: Icon(Icons.translate, size: 20),
                ),
                hint: const Text('Seç'),
                items: [
                  for (final seviye in _dilSeviyeleri)
                    DropdownMenuItem(
                      value: seviye,
                      child: Text(_kisaDil(seviye)),
                    ),
                ],
                onChanged: (deger) => setState(() => _dil = deger),
              ),
            ),
          ],
        ),
        const SizedBox(height: 14),
        AppTextField(
          controller: _sehir,
          label: 'Şehir',
          hint: 'örn. Ankara, İstanbul, Online',
          prefixIcon: Icons.place_outlined,
          textInputAction: TextInputAction.next,
        ),
        const SizedBox(height: 14),
        InkWell(
          borderRadius: BorderRadius.circular(12),
          onTap: _turSeciminiAc,
          child: InputDecorator(
            decoration: const InputDecoration(
              labelText: 'Tercih Edilen Türler',
              prefixIcon: Icon(Icons.category_outlined, size: 20),
            ),
            child: _tercihEdilenTurler.isEmpty
                ? const Text('Tür seçiniz')
                : Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      for (final (apiDegeri, gorunenAd)
                          in _tercihEdilenTurSecenekleri)
                        if (_tercihEdilenTurler.contains(apiDegeri))
                          Tooltip(
                            message: gorunenAd,
                            child: KategoriAvatar(kategori: apiDegeri),
                          ),
                    ],
                  ),
          ),
        ),
        const SizedBox(height: 14),
        AppTextField(
          controller: _ilgiAlanlari,
          label: 'İlgi Alanları',
          hint: 'örn. Web Geliştirme, Yapay Zeka, Veri Bilimi...',
          prefixIcon: Icons.interests_outlined,
          maxLines: 4,
        ),
      ],
    );
  }
}
