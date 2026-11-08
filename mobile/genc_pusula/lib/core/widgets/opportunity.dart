import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../shared/models/firsat_ozeti.dart';
import '../theme/colors.dart';

class OpportunityCard extends StatelessWidget {
  const OpportunityCard({super.key, required this.firsat});

  final FirsatOzeti firsat;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        leading: CircleAvatar(
          backgroundColor: AppColors.primary.withValues(alpha: 0.10),
          child: Icon(
            _kategoriIkonu(firsat.firsatTuru),
            color: AppColors.primary,
          ),
        ),
        title: Text(
          firsat.baslik,
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
          style: Theme.of(context).textTheme.titleMedium,
        ),
        subtitle: Row(
          children: [
            const Icon(Icons.place_outlined, size: 14, color: AppColors.muted),
            const SizedBox(width: 3),
            Expanded(
              child: Text(
                firsat.sehir,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodySmall
                    ?.copyWith(color: AppColors.muted),
              ),
            ),
            Text(
              firsat.sonTarih,
              style: Theme.of(context).textTheme.bodySmall
                  ?.copyWith(color: AppColors.muted),
            ),
          ],
        ),
        onTap: () => context.push('/firsat/${firsat.id}'),
      ),
    );
  }

  IconData _kategoriIkonu(String kategori) {
    return switch (kategori) {
      'Burs' => Icons.school_outlined,
      'Staj' => Icons.work_outline,
      'Etkinlik' => Icons.event_outlined,
      'Yarisma' || 'Yarışma' => Icons.emoji_events_outlined,
      _ => Icons.work_outline,
    };
  }
}
