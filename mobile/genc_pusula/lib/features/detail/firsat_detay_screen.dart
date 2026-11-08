import 'package:flutter/material.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';
import '../../core/widgets/empty_state.dart';
import '../../core/widgets/etiket_badge.dart';
import '../../shared/models/firsat_ozeti.dart';

class FirsatDetayScreen extends StatefulWidget {
  const FirsatDetayScreen({super.key, required this.id});

  final int id;

  @override
  State<FirsatDetayScreen> createState() => _FirsatDetayScreenState();
}

class _FirsatDetayScreenState extends State<FirsatDetayScreen> {
  late Future<FirsatOzeti> _firsat;

  @override
  void initState() {
    super.initState();
    _firsat = appState.firsatDetayGetir(widget.id);
  }

  @override
  void didUpdateWidget(covariant FirsatDetayScreen oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.id != widget.id) {
      _firsat = appState.firsatDetayGetir(widget.id);
    }
  }

  void _yenile() {
    setState(() {
      _firsat = appState.firsatDetayGetir(widget.id);
    });
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<FirsatOzeti>(
      future: _firsat,
      builder: (context, snapshot) {
        final firsat = snapshot.data;

        return Scaffold(
          appBar: AppBar(
            title: Text(
              firsat?.baslik ?? 'Fırsat Detayı',
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ),
          body: _govde(snapshot),
        );
      },
    );
  }

  Widget _govde(AsyncSnapshot<FirsatOzeti> snapshot) {
    if (snapshot.connectionState == ConnectionState.waiting) {
      return const Center(child: CircularProgressIndicator());
    }

    if (snapshot.hasError) {
      return EmptyState(
        icon: Icons.cloud_off_rounded,
        title: 'Fırsat yüklenemedi',
        message: snapshot.error.toString(),
        actionLabel: 'Tekrar dene',
        onAction: _yenile,
      );
    }

    final firsat = snapshot.data;
    if (firsat == null) {
      return EmptyState(
        icon: Icons.search_off_rounded,
        title: 'Fırsat bulunamadı',
        message: 'Bu ilan kaldırılmış veya hiç var olmamış olabilir.',
        actionLabel: 'Tekrar dene',
        onAction: _yenile,
      );
    }

    return _detay(firsat);
  }

  Widget _detay(FirsatOzeti firsat) {
    final textTheme = Theme.of(context).textTheme;

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
      children: [
        Row(
          children: [
            EtiketBadge(text: firsat.firsatTuru, color: AppColors.primary),
            if (firsat.guncellikEtiketi.isNotEmpty) ...[
              const SizedBox(width: 6),
              EtiketBadge(
                text: firsat.guncellikEtiketi,
                color: _guncellikRengi(firsat.guncellikEtiketi),
              ),
            ],
            Expanded(
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  const Icon(
                    Icons.place_outlined,
                    size: 16,
                    color: AppColors.muted,
                  ),
                  const SizedBox(width: 4),
                  Flexible(
                    child: Text(
                      firsat.sehir,
                      overflow: TextOverflow.ellipsis,
                      textAlign: TextAlign.right,
                      style: textTheme.bodyMedium?.copyWith(
                        color: AppColors.muted,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
        const SizedBox(height: 20),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                _bilgiSatiri('Son başvuru', firsat.sonTarih, textTheme),
                const Divider(height: 20),
                _bilgiSatiri('Son kontrol', firsat.sonKontrol, textTheme),
              ],
            ),
          ),
        ),
        const SizedBox(height: 20),
        Text('Açıklama', style: textTheme.titleMedium),
        const SizedBox(height: 8),
        Text(
          firsat.aciklama.isEmpty ? 'Açıklama belirtilmemiş.' : firsat.aciklama,
          style: textTheme.bodyMedium?.copyWith(height: 1.5),
        ),
        const SizedBox(height: 20),
        Text('Kaynak', style: textTheme.titleMedium),
        const SizedBox(height: 8),
        SelectableText(
          firsat.kaynakBaglantisi.isEmpty
              ? 'Kaynak bağlantısı belirtilmemiş.'
              : firsat.kaynakBaglantisi,
          style: textTheme.bodyMedium?.copyWith(color: AppColors.primary),
        ),
      ],
    );
  }

  Widget _bilgiSatiri(String etiket, String deger, TextTheme textTheme) {
    return Row(
      children: [
        Expanded(
          child: Text(
            etiket,
            style: textTheme.bodyMedium?.copyWith(color: AppColors.muted),
          ),
        ),
        Text(deger, style: textTheme.bodyMedium),
      ],
    );
  }

  Color _guncellikRengi(String etiket) {
    return switch (etiket) {
      'Güncel' => AppColors.success,
      'Kontrol Edilmeli' => AppColors.accent,
      'Eski' => AppColors.error,
      _ => AppColors.muted,
    };
  }
}
