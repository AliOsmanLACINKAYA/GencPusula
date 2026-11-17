import 'package:flutter/material.dart';

import '../app_state.dart';
import '../theme/colors.dart';

class KaydetIkonu extends StatefulWidget {
  const KaydetIkonu({
    super.key,
    required this.firsatId,
    this.iconSize = 20,
    this.color,
    this.kaydetmeMesaji,
    this.kaldirmaMesaji,
    this.zatenKayitliMesajiniGoster = false,
    this.profilAksiyonu,
    this.kompakt = true,
  });

  final int firsatId;

  final double iconSize;

  final Color? color;

  final String? kaydetmeMesaji;

  final String? kaldirmaMesaji;

  final bool zatenKayitliMesajiniGoster;

  final VoidCallback? profilAksiyonu;

  final bool kompakt;

  @override
  State<KaydetIkonu> createState() => _KaydetIkonuState();
}

class _KaydetIkonuState extends State<KaydetIkonu> {
  bool _isleniyor = false;

  Future<void> _degistir() async {
    if (_isleniyor) return;

    setState(() => _isleniyor = true);
    try {
      if (appState.kayitliMi(widget.firsatId)) {
        final kayitId = appState.kayitliKayitId(widget.firsatId);
        if (kayitId != null) {
          await appState.firsatKaydiniKaldir(kayitId);
          if (!mounted) return;
          _mesaj(widget.kaldirmaMesaji);
        }
      } else {
        await appState.firsatKaydet(widget.firsatId);
        if (!mounted) return;
        _mesaj(widget.kaydetmeMesaji);
      }
    } catch (e) {
      if (!mounted) return;
      if (e is ApiHatasi && e.kod == 'ZatenKayitli') {
        if (widget.zatenKayitliMesajiniGoster) _mesaj(e.mesaj);
        await appState.kayitliFirsatlariTazele();
      } else if (e is ApiHatasi && e.kod == 'ProfilGerekli') {
        _mesaj(
          e.mesaj,
          aksiyon: widget.profilAksiyonu == null
              ? null
              : SnackBarAction(
                  label: 'Profil',
                  onPressed: widget.profilAksiyonu!,
                ),
        );
      } else {
        _mesaj(e is ApiHatasi ? e.mesaj : 'İşlem tamamlanamadı. Tekrar dene.');
      }
    } finally {
      if (mounted) setState(() => _isleniyor = false);
    }
  }

  void _mesaj(String? metin, {SnackBarAction? aksiyon}) {
    if (metin == null) return;
    ScaffoldMessenger.of(context)
        .showSnackBar(SnackBar(content: Text(metin), action: aksiyon));
  }

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: appState,
      builder: (context, _) {
        final kayitli = appState.kayitliMi(widget.firsatId);

        return IconButton(
          onPressed: _isleniyor ? null : _degistir,
          tooltip: kayitli ? 'Kaydedilenlerden kaldır' : 'Kaydet',
          iconSize: widget.iconSize,
          padding: widget.kompakt ? EdgeInsets.zero : null,
          style: widget.kompakt
              ? IconButton.styleFrom(
                  minimumSize: const Size(28, 24),
                  tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                )
              : null,
          color:
              widget.color ?? (kayitli ? AppColors.primary : AppColors.muted),
          icon: _isleniyor
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Icon(kayitli ? Icons.bookmark : Icons.bookmark_border),
        );
      },
    );
  }
}
