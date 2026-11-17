import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../shared/models/firsat_ozeti.dart';
import '../theme/colors.dart';
import 'kategori_avatar.dart';
import 'kaydet_ikonu.dart';

class OpportunityCard extends StatelessWidget {
  const OpportunityCard({super.key, required this.firsat});

  final FirsatOzeti firsat;

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Card(
      clipBehavior: Clip.antiAlias,
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        leading: KategoriAvatar(kategori: firsat.firsatTuru),
        title: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Text(
                firsat.baslik,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: textTheme.titleMedium,
              ),
            ),
            const SizedBox(width: 4),
            KaydetIkonu(firsatId: firsat.id),
          ],
        ),
        subtitle: Row(
          children: [
            const Icon(
              Icons.place_outlined,
              size: 14,
              color: AppColors.muted,
            ),
            const SizedBox(width: 3),
            Expanded(
              child: Text(
                firsat.sehir,
                overflow: TextOverflow.ellipsis,
                style: textTheme.bodySmall?.copyWith(
                  color: AppColors.muted,
                ),
              ),
            ),
            Text(
              firsat.sonTarih,
              style: textTheme.bodySmall?.copyWith(
                color: AppColors.muted,
              ),
            ),
          ],
        ),
        onTap: () => context.push('/firsat/${firsat.id}'),
      ),
    );
  }
}
