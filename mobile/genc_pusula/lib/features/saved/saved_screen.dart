import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';
import '../../core/widgets/empty_state.dart';
import '../../core/widgets/kategori_avatar.dart';
import '../../shared/models/kayitli_firsat.dart';

class SavedScreen extends StatefulWidget {
  const SavedScreen({super.key});

  @override
  State<SavedScreen> createState() => _SavedScreenState();
}

class _SavedScreenState extends State<SavedScreen> {
  List<KayitliFirsat> _kayitlar = [];
  bool _yukleniyor = true;
  String? _hata;
  int _sonSurum = 0;
  int _istekNo = 0;
  final Set<int> _silinenler = <int>{};

  @override
  void initState() {
    super.initState();
    _sonSurum = appState.kayitliFirsatSurumu;
    appState.addListener(_surumDinle);
    _yukle(baslangic: true);
  }

  @override
  void dispose() {
    appState.removeListener(_surumDinle);
    super.dispose();
  }

  void _surumDinle() {
    if (appState.kayitliFirsatSurumu == _sonSurum) return;
    _sonSurum = appState.kayitliFirsatSurumu;
    _yukle();
  }

  Future<void> _yukle({bool baslangic = false}) async {
    final istekNo = ++_istekNo;
    if (!baslangic) {
      setState(() {
        _yukleniyor = true;
        _hata = null;
      });
    }

    try {
      final kayitlar = await appState.kayitliFirsatlariGetir();
      if (!mounted || istekNo != _istekNo) return;
      setState(() {
        _kayitlar = kayitlar;
        _hata = null;
      });
    } catch (e) {
      if (!mounted || istekNo != _istekNo) return;
      setState(() {
        _hata = e is ApiHatasi
            ? e.mesaj
            : 'Kaydedilenler yüklenemedi. Tekrar dene.';
      });
    } finally {
      if (mounted && istekNo == _istekNo) {
        setState(() => _yukleniyor = false);
      }
    }
  }

  Future<void> _sil(KayitliFirsat kayit) async {
    if (_silinenler.contains(kayit.id)) return;
    setState(() => _silinenler.add(kayit.id));

    try {
      await appState.firsatKaydiniKaldir(kayit.id);
      if (!mounted) return;
      setState(() => _kayitlar.removeWhere((k) => k.id == kayit.id));
      _mesaj('Kayıt kaldırıldı.');
    } catch (e) {
      if (!mounted) return;
      _mesaj(e is ApiHatasi ? e.mesaj : 'Kayıt kaldırılamadı. Tekrar dene.');
    } finally {
      if (mounted) setState(() => _silinenler.remove(kayit.id));
    }
  }

  void _mesaj(String metin) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(metin)));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Kaydedilenler')),
      body: _govde(),
    );
  }

  Widget _govde() {
    if (_yukleniyor && _kayitlar.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_hata != null && _kayitlar.isEmpty) {
      return EmptyState(
        icon: Icons.cloud_off_rounded,
        title: 'Kaydedilenler yüklenemedi',
        message: _hata!,
        actionLabel: 'Tekrar dene',
        onAction: () => _yukle(),
      );
    }

    return RefreshIndicator(
      onRefresh: () => _yukle(),
      child: _kayitlar.isEmpty
          ? ListView(
              physics: const AlwaysScrollableScrollPhysics(),
              children: [
                SizedBox(
                  height: MediaQuery.sizeOf(context).height * 0.7,
                  child: EmptyState(
                    icon: Icons.bookmark_border_rounded,
                    title: 'Henüz kayıtlı fırsat yok',
                    message:
                        'İlgilendiğin fırsatları kaydederek burada takip '
                        'edebilirsin.',
                    actionLabel: 'Fırsatları keşfet',
                    onAction: () => context.go('/home'),
                  ),
                ),
              ],
            )
          : ListView.separated(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
              itemCount: _kayitlar.length,
              separatorBuilder: (context, index) => const SizedBox(height: 12),
              itemBuilder: (context, index) => _kart(_kayitlar[index]),
            ),
    );
  }

  Widget _kart(KayitliFirsat kayit) {
    final textTheme = Theme.of(context).textTheme;
    final tarihStil = textTheme.bodyMedium?.copyWith(
      color: AppColors.muted,
      fontWeight: FontWeight.w700,
    );

    return Card(
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        leading: KategoriAvatar(kategori: kayit.firsatTuru),
        title: Text(
          kayit.baslik,
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
          style: textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
        ),
        subtitle: Padding(
          padding: const EdgeInsets.only(top: 4),
          child: Text(kayit.sonTarih, style: tarihStil),
        ),
        trailing: _silinenler.contains(kayit.id)
            ? const Padding(
                padding: EdgeInsets.all(12),
                child: SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2.2),
                ),
              )
            : IconButton(
                tooltip: 'Kaydı kaldır',
                onPressed: () => _sil(kayit),
                icon: const Icon(Icons.delete_outline),
              ),
        onTap: () => context.push('/firsat/${kayit.firsatId}'),
      ),
    );
  }
}
